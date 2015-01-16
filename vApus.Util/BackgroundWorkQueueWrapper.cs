/*
 * Copyright 2010 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */

using RandomUtils;
namespace vApus.Util {
    public static class BackgroundWorkQueueWrapper {
        public static readonly BackgroundWorkQueue BackgroundWorkQueue;

        static BackgroundWorkQueueWrapper() { BackgroundWorkQueue = new BackgroundWorkQueue(); }
    }
}