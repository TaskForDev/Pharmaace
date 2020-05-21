using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    class CASharesDimensionFactory : DimensionFactory
    {
        public override bool Probe(string sentence)
        {
            var thesaurus = StaticResources.GetInstance()?.Database?.Thesaurus;

            var m = GetR3MShareEntity(sentence);
            if (m == null)
                m = GetMonthlyShareEntity(sentence);
            if (m == null)
                m = GetR2MShareEntity(sentence);
            if (m != null)
            {
                this.ruleEngine = new CASharesRuleEngine(m);
                this.chartEngine = new CASharesChartEngine();
                return true;
            }
            else
            {
                var entity = new R2MShare();
                m = new RecognizedEntity
                {
                    Entity = entity, //default
                    Index = -1, //a -1 index indicates the entity is added virtually
                    RecognizedName = entity.DomainName,
                    RecognizedValue = entity.DomainName
                };
                this.ruleEngine = new CASharesRuleEngine(m);
                this.chartEngine = new CASharesChartEngine();
                return false; //yes, return false to let the caller know that it is not identified
            }
        }

        private RecognizedEntity GetMonthlyShareEntity(string sentence)
        {
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                string pattern = @"^(?=.*\bmonthly\b)(?=.*\bshare(s)?\b)?.*$";
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);

                if (match.Success)
                {
                    var entity = new Share();
                    return new RecognizedEntity
                    {
                        Entity = entity,
                        Index = match.Index,
                        RecognizedName = entity.DomainName,
                        RecognizedValue = entity.DomainName
                    };
                }
            }

            return null;
        }

        private RecognizedEntity GetR2MShareEntity(string sentence)
        {
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                string pattern = @"^(((?=.*\brolling\b)(?=.*\b2\smonth(s)?\b))|(?=.*\bR2M\b))?(?=.*\bshare(s)?\b)?.*$";
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);

                if (match.Success)
                {
                    var entity = new R2MShare();
                    return new RecognizedEntity
                    {
                        Entity = entity,
                        Index = match.Index,
                        RecognizedName = entity.DomainName,
                        RecognizedValue = entity.DomainName
                    };
                }
            }

            return null;
        }

        private RecognizedEntity GetR3MShareEntity(string sentence)
        {
            if (!String.IsNullOrWhiteSpace(sentence))
            {
                string pattern = @"^(((?=.*\brolling\b)(?=.*\b3\smonth(s)?\b))|(?=.*\bR3M\b))(?=.*\bshare(s)?\b)?.*$";
                Match match = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).Match(sentence);

                if (match.Success)
                {
                    var entity = new R3MShare();
                    return new RecognizedEntity
                    {
                        Entity = entity,
                        Index = match.Index,
                        RecognizedName = entity.DomainName,
                        RecognizedValue = entity.DomainName
                    };
                }
            }

            return null;
        }
    }
}
