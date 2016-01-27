/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
namespace vApus.Publish {
    internal interface IDestination {
        bool AllowMultipleInstances { get; }
        void Post(object message);
    }
}
