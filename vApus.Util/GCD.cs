/*
 * 2013 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
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
