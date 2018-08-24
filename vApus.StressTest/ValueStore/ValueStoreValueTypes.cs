/*
 * 2016 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.ComponentModel;

namespace vApus.StressTest {
    [Serializable]
    public enum ValueStoreValueTypes {
        [Description("Object")]
        objectType = 0,
        [Description("String")]
        stringType,
        [Description("32-bit integer")]
        intType,
        [Description("64-bit integer")]
        longType,
        [Description("32-bit floating point")]
        floatType,
        [Description("64-bit floating point")]
        doubleType,
        [Description("Boolean")]
        boolType
    }
}
