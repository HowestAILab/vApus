/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Reflection;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    [DisplayName("Custom Random Parameter"), Serializable]
    public class CustomRandomParameter : BaseParameter {
        private ICustomRandomParameter _customRandomParameter;
        private string _code = @"// dllreferences:System.dll;vApus.Stresstest.dll
using System;
namespace vApus.Stresstest {
public class CustomRandomParameter : ICustomRandomParameter {
public string Generate() {
// Example:
return GetRandomDateTime(" + "\"03-05-1986\", \"03-05-2086\").ToString();" + @"

// Custom DateTime to string formats can be found here http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx.
}
private DateTime GetRandomDateTime(string start, string stop) {
return GetRandomDateTime(DateTime.Parse(start), DateTime.Parse(stop));
}
private DateTime GetRandomDateTime(DateTime start, DateTime stop) {
// Uncomment the following line to test if the returning of unique values works. This line is not needed in a test.
// System.Threading.Thread.Sleep(1);

// A random can only handle 32-bit integers and we need a 64-bit integer, therefore this workaround.
var rand = new Random();
byte[] buffer = new byte[8];

rand.NextBytes(buffer);

long longRand = BitConverter.ToInt64(buffer, 0);            
long randomTicks = Math.Abs(longRand % (stop.Ticks - start.Ticks));

return start.AddTicks(randomTicks);
}
}
}";
        private bool _unique;

        [SavableCloneable]
        public string Code {
            get { return _code; }
            set { _code = value; }
        }

        [SavableCloneable]
        public bool Unique {
            get { return _unique; }
            set { _unique = value; }
        }

        public override void Next() {
            lock (_lock)
            //For thread safety, only here, because only for this type of parameter this function can be used while testing.
            {
                try {
                    if (_customRandomParameter == null)
                        CreateInstance();

                    _value = _customRandomParameter.Generate();
                } catch {
                    throw new Exception("[" + this + "] The custom code does not compile!\nPlease check it for errors.");
                }

                if (_unique) {
                    if (_chosenValues.Count == int.MaxValue)
                        _chosenValues.Clear();

                    int loops = 0; //Preferably max 1, detecting infinite loops here.
                    int maxLoops = 10;
                    while (!_chosenValues.Add(_value)) {
                        if (_chosenValues.Count == int.MaxValue)
                            _chosenValues.Clear();

                        _value = _customRandomParameter.Generate();

                        if (++loops == maxLoops)
                            throw new Exception("[" + this + "] Your code cannot provide unique values!");
                    }
                }
            }
        }

        internal CompilerResults CreateInstance() {
            var cu = new CompilerUnit();
            CompilerResults results;
            Assembly assembly = cu.Compile(_code, false, out results);

            if (assembly != null)
                _customRandomParameter =
                    assembly.CreateInstance("vApus.Stresstest.CustomRandomParameter") as ICustomRandomParameter;

            return results;
        }

        public override void ResetValue() {
            _customRandomParameter = null;
            _chosenValues.Clear();
        }

        public override void Activate() {
            SolutionComponentViewManager.Show(this);
        }
    }

    public interface ICustomRandomParameter {
        string Generate();
    }
}