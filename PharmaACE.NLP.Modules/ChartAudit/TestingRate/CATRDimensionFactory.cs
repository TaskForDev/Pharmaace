using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    class CATRDimensionFactory : DimensionFactory
    {
        public override bool Probe(string sentence)
        {
            var thesaurus = StaticResources.GetInstance()?.Database?.Thesaurus;
            var measureREs = new List<RecognizedEntity>{
                new RecognizedEntity
                {
                    Entity = new Share()
                }
            };

            var m = GetR3MTREntity(sentence);
            if (m == null)
                m = GetMonthlyTREntity(sentence);
            if (m == null)
                m = GetR2MTREntity(sentence);

            if (m != null)
            {
                this.ruleEngine = new CATRRuleEngine(m);
                this.chartEngine = new CATRChartEngine();
                return true;
            }

            return false;
        }

        private RecognizedEntity GetMonthlyTREntity(string sentence)
        {
            string pattern = @"^(?=.*\bmonthly\b)(?=.*\btesting\b)(?=.*\brate(s)?\b).*$";
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                if (match.Success)
                {
                    return new RecognizedEntity
                    {
                        Entity = new TR(),
                        Index = match.Index
                    };
                }
            }

            return null;
        }

        private RecognizedEntity GetR2MTREntity(string sentence) //default
        {
            string pattern = @"^(((?=.*\brolling\b)(?=.*\b2\smonth(s)?\b))|(?=.*\bR2M\b))?(?=.*\btesting\b)(?=.*\brate(s)?\b).*$";
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                if (match.Success)
                {
                    return new RecognizedEntity
                    {
                        Entity = new R2MTR(),
                        Index = match.Index
                    };
                }
            }

            return null;
        }

        private RecognizedEntity GetR3MTREntity(string sentence)
        {
            string pattern = @"^(((?=.*\brolling\b)(?=.*\b3\smonth(s)?\b))|(?=.*\bR3M\b))(?=.*\btesting\b)(?=.*\brate(s)?\b).*$";
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);
                if (match.Success)
                {
                    return new RecognizedEntity
                    {
                        Entity = new R3MTR(),
                        Index = match.Index
                    };
                }
            }

            return null;
        }
    }
}
