/*
 * Copyright 2016 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
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
