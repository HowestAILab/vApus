/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, department PIH
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Util
{
    public delegate void Callback();
    public delegate void ParameterizedCallback(object arg);
    public delegate object ReturningCallback();
    public delegate object ReturningParameterizedCallback(object arg);
}
