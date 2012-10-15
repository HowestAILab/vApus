/*
 * Copyright 2011 (c) Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 * Author(s):
 *    Dieter Vandroemme
 */
using System;
namespace vApus.LogFixer
{
    public static class UnitTestTextSearchAlgorithm
    {
        public static void Run()
        {
            string[] originalTexts = { "F$GET$$$$FOO", "AF$GET$B$C$D$GH", "AFOO", "FAOO", "FOOA", "F$GET$£FOO%*", "F$GET$£FLO%*JK", "ABC", "ABC", "http://www.google.be/s$GET$pq=hqs&hl=en&cp=1&gs_id$$$haha$ha$$$$" };
            string[] newTexts = { "AF$GET$B$C$D$GH", "F$GET$$$$FOO", "FOOA", "FOOA", "FAOO", "F$GET$BCD£€µE%*G/FOOHI", "AF$GET$BCD£€µE%*G/FOOHI", "DEFG", "DEAG", "/s$GET$pq=hqs&hl=en&cp=1&gs_id$$$haha$ha$$$$" };

            for (int i = 0; i != originalTexts.Length; i++)
            {
                string originalText = originalTexts[i];
                string newText = newTexts[i];

                Changes tracker = new Changes();
                var trackedChanges = tracker.Track(originalText, newText);

                Scenario scenario = new Scenario(trackedChanges);
                string scenarioNewText = scenario.Apply(originalText);
                bool succes = scenarioNewText == newText;

                Console.WriteLine("Make and apply scenario for changing '" + originalText + "' to '" + newText + "' --> " + (succes ? "OK" : "FAIL (scenario new text --> '" + scenarioNewText + "')"));
            }
        }
    }
}
