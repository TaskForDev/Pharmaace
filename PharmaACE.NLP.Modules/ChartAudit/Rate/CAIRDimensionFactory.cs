using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PharmaACE.NLP.Framework;
using PharmaACE.Utility;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    class CAIRDimensionFactory : DimensionFactory
    {
        string testStatusPattern;
        public CAIRDimensionFactory()
        {
            var statusSet = StaticResources.GetInstance().Database.GetTableByName(CAConstants.ITR_VIEW).GetColumnByName(new TumorTestStatus().DomainName).MatchCandidates;
            var listOfEquivalents = new List<string>();
            foreach (var val in statusSet)
            {
                if (StaticResources.GetInstance().Database.Thesaurus.Dictionary.ContainsKey(val))
                {
                    listOfEquivalents.AddRange(StaticResources.GetInstance().Database.Thesaurus.Dictionary[val]);
                }
            }
            statusSet.AddOrUpdate(listOfEquivalents);
            testStatusPattern = "\\b" + String.Join("\\b|\\b", statusSet) + "\\b";
        }

        public override bool Probe(string sentence)
        {
            var thesaurus = StaticResources.GetInstance()?.Database?.Thesaurus;
            var measureREs = new List<RecognizedEntity>{
                new RecognizedEntity
                {
                    Entity = new Share()
                }
            };

            var m = GetR3MIREntity(sentence);
            if (m == null)
                m = GetMonthlyIREntity(sentence);
            if (m == null)
                m = GetR2MIREntity(sentence);

            if (m != null)
            {
                this.ruleEngine = new CAIRRuleEngine(m);
                this.chartEngine = new CAIRChartEngine();
                return true;
            }

            return false;
        }

        private RecognizedEntity GetMonthlyIREntity(string sentence)
        {
            string pattern = @"^(?=.*\bmonthly\b)(?=.*\brate(s)?\b).*$";
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                if (match.Success)
                {
                    return new RecognizedEntity
                    {
                        Entity = new IR(),
                        Index = match.Index
                    };
                }
            }

            return null;
        }

        private RecognizedEntity GetR2MIREntity(string sentence)
        {
            string pattern = @"^(((?=.*\brolling\b)(?=.*\b2\smonth(s)?\b))|(?=.*\bR2M\b))?(?=.*\brate(s)?\b).*$";
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                //if "rate" is not found, see if any of the test status is present or not, if present then additionally check if "share" present or not
                if (!match.Success)
                {
                    match = new Regex(testStatusPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                    if (match.Success)
                    {
                        //"share" should not be present
                        Match shareMatch = new Regex("\\bshares?\\b", RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                        if (shareMatch.Success)
                            return null;
                    }
                }
                if (match.Success)
                {
                    return new RecognizedEntity
                    {
                        Entity = new R2MIR(),
                        Index = match.Index
                    };
                }
            }

            return null;
        }

        private RecognizedEntity GetR3MIREntity(string sentence)
        {
            string pattern = @"^(((?=.*\brolling\b)(?=.*\b3\smonth(s)?\b))|(?=.*\bR3M\b))(?=.*\brate(s)?\b).*$";
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                if (match.Success)
                {
                    return new RecognizedEntity
                    {
                        Entity = new R3MIR(),
                        Index = match.Index
                    };
                }
            }

            return null;
        }
    }
}
