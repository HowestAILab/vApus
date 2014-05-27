/*
 * Copyright 2014 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using RandomUtils.Log;

namespace vApus.Util {
    public class LogPanel : FileLoggerPanel {
        public override string ToString() {
            return "Application Logging";
        }
    }
}
