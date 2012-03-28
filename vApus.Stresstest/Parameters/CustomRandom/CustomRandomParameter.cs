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
        private ICustomRandomParameter _customParameter;
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
            if (_customParameter == null)
                CreateInstance();

            _value = _customParameter.Generate();

            if (_unique)
            {
                if (_chosenValues.Count == int.MaxValue)
                    _chosenValues.Clear();

                while (!_chosenValues.Add(_value))
                {
                    if (_chosenValues.Count == int.MaxValue)
                        _chosenValues.Clear();

                    _value = _customParameter.Generate();
                }
            }
        }
        internal CompilerResults CreateInstance()
        {
            CompilerUnit cu = new CompilerUnit();
            CompilerResults results;
            Assembly assembly = cu.Compile(BuildCode(), false, out results);

            if (assembly != null)
                _customParameter = assembly.CreateInstance("vApus.Stresstest.CustomRandomParameter") as ICustomRandomParameter;

            return results;
        }
        private string BuildCode()
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
            _customParameter = null;
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
