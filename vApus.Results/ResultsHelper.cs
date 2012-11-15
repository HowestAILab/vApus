/*
 * Copyright 2012 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vApus.Results
{
    public static class ResultsHelper
    {
        public static Test CurrentTest { get; private set; }
        /// <summary>
        /// Returns a new test object.
        /// Tries to create a db if needed, and applies the schema if needed.
        /// </summary>
        /// <returns>An exception if the db could not be created.</returns>
        public static Exception TryGetNewTest(out Test test)
        {
            CurrentTest = new Test();
            test = CurrentTest;

            try { SchemaBuilder.Build(); }
            catch (Exception ex) { return ex; }
            return null;
        }
    }
}
