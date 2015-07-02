/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Publish {
    public class JSONFormatter : IFormatter {
        private object _cached, _cachedFormatted;

        /// <summary>
        /// If it was formatted before, the cached formatted message will be returned.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public object Format(object message) {
            if (_cached != message) {
                _cached = message;
                _cachedFormatted = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            }
            return _cachedFormatted;
        }
    }
}
