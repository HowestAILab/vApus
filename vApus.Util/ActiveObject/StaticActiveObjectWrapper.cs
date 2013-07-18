/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Util {
    public static class StaticActiveObjectWrapper {
        public static readonly ActiveObject ActiveObject;

        static StaticActiveObjectWrapper() {
            ActiveObject = new ActiveObject();
        }
    }
}