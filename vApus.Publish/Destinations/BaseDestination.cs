/*
 * Copyright 2015 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
namespace vApus.Publish {
    internal abstract class BaseDestination : IDestination {
        private IFormatter _formatter;

        /// <summary>
        /// Set a formatter to format the message before it is posted.
        /// </summary>
        public IFormatter Formatter { get { return _formatter; } set { _formatter = value; } }

        internal object FormatMessage(object message) { return (_formatter == null) ? message : _formatter.Format(message); }

        public abstract void Post(object message);
    }
}
