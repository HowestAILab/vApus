using RandomUtils;
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
using System.Runtime.Serialization;
using vApus.SolutionTree;
using vApus.Util;

namespace vApus.Stresstest {
    /// <summary>
    /// You can use your own piece of code as a parameter, an example is included as the default value.
    /// </summary>
    [DisplayName("Custom Random Parameter"), Serializable]
    public class CustomRandomParameter : BaseParameter, ISerializable {

        #region Fields
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
// Uncomment the following line to test if the returning of unique values works. You can add these values to a custom list parameter.
// System.Threading.Thread.Sleep(1);

// A Random can only handle 32-bit integers and we need a 64-bit integer, therefore this workaround.
var rand = new Random();
byte[] buffer = new byte[8];

rand.NextBytes(buffer);

long longRand = BitConverter.ToInt64(buffer, 0);            
long randomTicks = Math.Abs(longRand % (stop.Ticks - start.Ticks));

return start.AddTicks(randomTicks);
}
}
}";
        #endregion

        #region Properties
        [SavableCloneable]
        public string Code {
            get { return _code; }
            set { _code = value; }
        }

        [SavableCloneable]
        public bool Unique { get; set; }
        #endregion

        #region Constructors
        public CustomRandomParameter() {
            if (Solution.ActiveSolution == null)
                Solution.ActiveSolutionChanged += Solution_ActiveSolutionChanged;
        }
        public CustomRandomParameter(SerializationInfo info, StreamingContext ctxt) {
            SerializationReader sr;
            using (sr = SerializationReader.GetReader(info)) {
                ShowInGui = false;
                Label = sr.ReadString();
                _code = sr.ReadString();
                Unique = sr.ReadBoolean();
                _tokenNumericIdentifier = sr.ReadInt32();
            }
            sr = null;
        }
        #endregion

        #region Functions
        private void Solution_ActiveSolutionChanged(object sender, ActiveSolutionChangedEventArgs e) {
            Solution.ActiveSolutionChanged -= Solution_ActiveSolutionChanged;
            if (Parent != null && Parent is CustomListParameter)
                ShowInGui = false;
        }

        public override void Next() {
            lock (_lock)
            //For thread safety, only here, because only for this type of parameter this function can be used while testing.
            {
                try {
                    if (_customRandomParameter == null)
                        CreateInstance();

                    Value = _customRandomParameter.Generate();
                } catch {
                    throw new Exception("[" + this + "] The custom code does not compile!\nPlease check it for errors.");
                }

                if (Unique) {
                    if (_chosenValues.Count == int.MaxValue)
                        _chosenValues.Clear();

                    int loops = 0; //Preferably max 1, detecting infinite loops here.
                    int maxLoops = 10;
                    while (!_chosenValues.Add(Value)) {
                        if (_chosenValues.Count == int.MaxValue)
                            _chosenValues.Clear();

                        Value = _customRandomParameter.Generate();

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
            Type t = assembly.GetType("vApus.Stresstest.CustomRandomParameter");

            if (assembly != null)
                _customRandomParameter = FastObjectCreator.CreateInstance<ICustomRandomParameter>(t);

            return results;
        }

        public override void ResetValue() {
            _customRandomParameter = null;
            _chosenValues.Clear();
        }

        public override BaseSolutionComponentView Activate() { return SolutionComponentViewManager.Show(this); }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            SerializationWriter sw;
            using (sw = SerializationWriter.GetWriter()) {
                sw.Write(Label);
                sw.Write(_code);
                sw.Write(Unique);
                sw.Write(_tokenNumericIdentifier);
                sw.AddToInfo(info);
            }
            sw = null;
        }
        #endregion
    }

    public interface ICustomRandomParameter { string Generate();  }
}