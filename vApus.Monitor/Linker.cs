/*
 * Copyright 2009 (c) Sizing Servers Lab
 * University College of West-Flanders, department PIH
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using vApus.Monitor;
using vApus.DistributedTesting;
using vApus.SolutionTree;
using vApus.Stresstest;

namespace vApus.Link
{
    /// <summary>
    /// This solves the circular dependency problem.
    /// Only vApus.Gui should have a reference to the assembly.
    /// </summary>
    public static class Linker
    {
        public static void Link()
        {
            Solution.RegisterProjectType(typeof(DistributedTestingProject));
            Solution.RegisterProjectType(typeof(MonitorProject));
            Solution.RegisterProjectType(typeof(StresstestProject));
        }
    }
}
