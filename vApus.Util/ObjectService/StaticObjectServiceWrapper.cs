/*
 * Copyright 2011 (c) Sizing Servers Lab 
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
namespace vApus.Util
{
    /// <summary>
    /// A wrapper around StaticService.
    /// </summary>
    public static class StaticObjectServiceWrapper
    {
        private static ObjectService _objectService = new ObjectService();

        public static ObjectService ObjectService
        {
            get { return _objectService; }
        }
    }
}