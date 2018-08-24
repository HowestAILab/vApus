/*
 * 2015 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

namespace vApus.Publish {
    /// <summary>
    /// Contains a super simple one-entry cache.
    /// </summary>
    public class JSONFormatter : IFormatter {
        private static JSONFormatter _instance;

        private object _cached, _cachedFormatted;

        private JSONFormatter() { }

        public static JSONFormatter GetInstance() { return _instance ?? (_instance = new JSONFormatter()); }

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
