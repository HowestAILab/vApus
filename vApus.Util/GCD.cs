/*
 * Copyright 2013 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System.Linq;

namespace vApus.Util {
    public static class GCD {
        public static int Get(int[] numbers) { return numbers.Aggregate(Get); }
        static int Get(int a, int b) { return b == 0 ? a : Get(b, a % b); }
        public static float Get(float[] numbers) { return numbers.Aggregate(Get); }
        static float Get(float a, float b) { return b == 0 ? a : Get(b, a % b); }
    }
}
