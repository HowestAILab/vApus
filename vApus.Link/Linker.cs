/*
 * 2009 Sizing Servers Lab, affiliated with IT bachelor degree NMCT
 * University College of West-Flanders, Department GKG (www.sizingservers.be, www.nmct.be, www.howest.be/en)
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.DistributedTest;
using vApus.Monitor;
using vApus.SolutionTree;
using vApus.StressTest;

namespace vApus.Link {
    /// <summary>
    ///     This solves the circular dependency problem.
    ///     Only vApus.Gui should have a reference to this assembly.
    /// </summary>
    public static class Linker {
        public static void Link() {
            Solution.RegisterProjectType(typeof(DistributedTestProject));
            Solution.RegisterProjectType(typeof(MonitorProject));
            Solution.RegisterProjectType(typeof(StressTestProject));
        }
    }
}