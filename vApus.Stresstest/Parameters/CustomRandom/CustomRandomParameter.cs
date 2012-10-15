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

namespace vApus.Stresstest
{
    [DisplayName("Custom Random Parameter"), Serializable]
    public class CustomRandomParameter : BaseParameter
    {
        private ICustomRandomParameter _customRandomParameter;
        private string _generateFunction = @"public string Generate() {
//
// You can use anything from the 'System' namespace to generate a random string (e.g. DateTime).
//
}";
        private bool _unique;

        [SavableCloneable]
        public string GenerateFunction
        {
            get { return _generateFunction; }
            set { _generateFunction = value; }
        }

        [SavableCloneable]
        public bool Unique
        {
            get { return _unique; }
            set { _unique = value; }
        }
        public override void Next()
        {
            lock (_lock)//For thread safety, only here, because only for this type of parameter this function can be used while testing.
            {
                try
                {
                    if (_customRandomParameter == null)
                        CreateInstance();

                    _value = _customRandomParameter.Generate();
                }
                catch
                {
                    throw new Exception("[" + this + "] The custom code does not compile!\nPlease check it for errors.");
                }

                if (_unique)
                {
                    if (_chosenValues.Count == int.MaxValue)
                        _chosenValues.Clear();

                    int loops = 0; //Preferably max 1, detecting infinite loops here.
                    int maxLoops = 10;
                    while (!_chosenValues.Add(_value))
                    {
                        if (_chosenValues.Count == int.MaxValue)
                            _chosenValues.Clear();

                        _value = _customRandomParameter.Generate();

                        if (++loops == maxLoops)
                            throw new Exception("[" + this + "] Your code cannot provide unique values!");
                    }
                }
            }
        }
        internal CompilerResults CreateInstance()
        {
            CompilerUnit cu = new CompilerUnit();
            CompilerResults results;
            Assembly assembly = cu.Compile(BuildCode(), false, out results);

            if (assembly != null)
                _customRandomParameter = assembly.CreateInstance("vApus.Stresstest.CustomRandomParameter") as ICustomRandomParameter;

            return results;
        }
        internal string BuildCode()
        {
            return @"// dllreferences:System.dll;vApus.Stresstest.dll
using System;
namespace vApus.Stresstest
{
public class CustomRandomParameter : ICustomRandomParameter
{
public CustomRandomParameter() {}"
+ _generateFunction + "}}";
        }
        public override void ResetValue()
        {
            _customRandomParameter = null;
            _chosenValues.Clear();
        }

        public override void Activate()
        {
            SolutionComponentViewManager.Show(this);
        }
    }

    public interface ICustomRandomParameter
    {
        string Generate();
    }
}
