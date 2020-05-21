using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using PharmaACE.NLP.DateTimeParser;
using PharmaACE.NLP.Framework;
using PharmaACE.Utility;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public abstract class CARuleEngine : RuleEngineBase
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        
        public override abstract string DbEntity { get; }
        
        public bool IsPanTumor { get; set; }
        public List<string> TumorNames { get; set; }
        List<SentenceFragment> PanTumors {
            get
            {
                return new List<SentenceFragment> {
                    new SentenceFragment {
                        RecognizedEntities =
                        new List<RecognizedEntity>{
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT1 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT1,
                                Value = "NSCLC",
                                RecognizedValue = "NSCLC"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT2 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT2,
                                Value = "1L",
                                RecognizedValue = "1L"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT3 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT3,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT4 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT4,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT5 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT5,
                                Value = null,
                                RecognizedValue = null
                            }
                        }
                    },
                    new SentenceFragment {
                        RecognizedEntities =
                        new List<RecognizedEntity>{
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT1 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT1,
                                Value = "NSCLC",
                                RecognizedValue = "NSCLC"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT2 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT2,
                                Value = "2L",
                                RecognizedValue = "2L"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT3 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT3,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT4 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT4,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT5 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT5,
                                Value = null,
                                RecognizedValue = null
                            }
                        }
                    },
                    new SentenceFragment {
                        RecognizedEntities =
                        new List<RecognizedEntity>{
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT1 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT1,
                                Value = "ADJ Melanoma",
                                RecognizedValue = "ADJ Melanoma"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT2 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT2,
                                Value = null,
                                RecognizedValue = null
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT3 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT3,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT4 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT4,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT5 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT5,
                                Value = null,
                                RecognizedValue = null
                            }
                        }
                    },
                    new SentenceFragment {
                        RecognizedEntities =
                        new List<RecognizedEntity>{
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT1 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT1,
                                Value = "Melanoma",
                                RecognizedValue = "Melanoma"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT2 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT2,
                                Value = "1L",
                                RecognizedValue = "1L"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT3 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT3,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT4 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT4,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT5 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT5,
                                Value = null,
                                RecognizedValue = null
                            }
                        }
                    },
                    new SentenceFragment {
                        RecognizedEntities =
                        new List<RecognizedEntity>{
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT1 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT1,
                                Value = "HCC",
                                RecognizedValue = "HCC"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT2 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT2,
                                Value = "2L",
                                RecognizedValue = "2L"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT3 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT3,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT4 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT4,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT5 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT5,
                                Value = null,
                                RecognizedValue = null
                            }
                        }
                    },
                    new SentenceFragment {
                        RecognizedEntities =
                        new List<RecognizedEntity>{
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT1 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT1,
                                Value = "RCC",
                                RecognizedValue = "RCC"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT2 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT2,
                                Value = "2L",
                                RecognizedValue = "2L"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT3 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT3,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT4 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT4,
                                Value = "Total",
                                RecognizedValue = "Total"
                            },
                            new RecognizedEntity
                            {
                                Entity = new NED { DomainName = CAConstants.DIMENSION1_COMPONENT5 },
                                RecognizedName = CAConstants.DIMENSION1_COMPONENT5,
                                Value = null,
                                RecognizedValue = null
                            }
                        }
                    }
                };
            }
        }

        protected abstract void SetNamedEntityDimensions();
        
        protected sealed override RuleValidationResult ApplyRules(ref List<SentenceFragment> sentenceFragments)
        {
            RuleValidationResult result = new RuleValidationResult();

            //there should be exactly 1 measure, otherwise error
            result = ApplyMeasureRules(ref sentenceFragments);
            if (result.Error != null)
                return result;
            
            result = ApplyNEDRules(ref sentenceFragments);
            if (result.Error != null)
                return result;

            result = ApplyTemporalRules(ref sentenceFragments);
            if (result.Error != null)
                return result;

            return result;
        }

        protected RuleValidationResult ApplyTemporalRules(ref List<SentenceFragment> sentenceFragments)
        {
            var res = new RuleValidationResult();
            var temporalFragmentIndices = GetFragmentBasedTemporals(sentenceFragments);

            //if no temporal found, use default based on measure singular or plural
            if (!temporalFragmentIndices.Any())
            {
                SetDefaultTemporal(sentenceFragments, TimeSeriesBoundary.End.Value);
            }
            else
            {   
                foreach (var fragment in sentenceFragments)
                {
                    foreach (var re in fragment.RecognizedEntities)
                    {
                        //shift datetime according to data availability
                        if(re.Entity is Time && (re.Entity as Time).PreferDataAvailability)
                        {
                            ShiftAsPerDataAvailability(fragment, re, TimeSeriesBoundary.End.Value);
                        }
                    }
                }
            }

            return res;
        }

        protected void ShiftAsPerDataAvailability(SentenceFragment fragment, RecognizedEntity timeRE, DateTime dataAvailableTill)
        {   
            var dr = timeRE.Value as DateRange;
            if (dr.End != null)
                return;
            //var shift = dr.Start.Value.Month - dataAvailableTill.Month;
            var start = dataAvailableTill;
            //DateTime? end = null;
            //if (dr.End != null)
            //    end = dr.End.Value.AddMonths(shift);
            timeRE.Value = new DateRange(start, null/*end*/);
            timeRE.RecognizedValue = timeRE.Value;
        }

        protected bool HasPluralMeasure(SentenceFragment fragment)
        {
            var measureRE = fragment.RecognizedEntities.Where(sfre => sfre.Entity as Measure != null).FirstOrDefault();
            if (measureRE != null)
                return measureRE.IsPlural;

            return false;
        }

        protected void SetDefaultTemporal(List<SentenceFragment> fragments, DateTime dataAvailableTill)
        {
            bool isMultipleDataPoint = fragments.Any(fr => HasPluralMeasure(fr));
        DateRange defaultTemporal = null;
            if (isMultipleDataPoint)
            {                
                var last12MonthsStart = dataAvailableTill.AddMonths(-12);
                defaultTemporal = new DateRange(last12MonthsStart, dataAvailableTill);
            }
            else
            {
                defaultTemporal = new DateRange(dataAvailableTill, null);
            }

            //TODO: temporals to be added to only those fragments which have 
            //      no or single temporal in case isMultipleDataPoint is true and
            //      no or multiple temporal in case isMultipleDataPoint is false (this case will never happen though, as in a mix of single and multiple
            //      temporals multiple temporals prevail)
            foreach (var fragment in fragments)
            {
                fragment.RecognizedEntities.Add(new RecognizedEntity
                {
                    Entity = new Time
                    {
                        Default = defaultTemporal
                    },
                    Value = defaultTemporal,
                    RecognizedValue = defaultTemporal
                });
            }
        }

        protected virtual RuleValidationResult ApplyNEDRules(ref List<SentenceFragment> sentenceFragments)
        {
            logger.Info("Inside ApplyNEDRules, pan tumor: " + IsPanTumor);
            var res = new RuleValidationResult();
            var tumorIndices = GetFragmentBasedTumors(sentenceFragments);
            //if no tumor found, default to pan tumor
            if (!tumorIndices.Any())
            {
                AdjustMeasureBasedOnNED(sentenceFragments);
                IsPanTumor = true;
                logger.Info("Pan tumor set to " + IsPanTumor);
                var fragments = new List<SentenceFragment>();
                foreach (var fragment in sentenceFragments)
                {
                    foreach (var tumorFragment in PanTumors)
                    {
                        var reList = new List<RecognizedEntity>(fragment.RecognizedEntities);
                        reList.AddRange(tumorFragment.RecognizedEntities.Where(re => re.Value != null));
                        fragments.Add(new SentenceFragment { RecognizedEntities = reList });
                    }
                }
                sentenceFragments = fragments;
            }

            return res;
        }

        /// <summary>
        /// this method assumes there is no tumor mentioned and temporal rules are not yet set
        /// </summary>
        /// <param name="sentenceFragments"></param>
        protected void AdjustMeasureBasedOnNED(List<SentenceFragment> sentenceFragments)
        {
            //if no tumor, no regimen, single or no temporal datapoint singularize plural measures if any
            var regimenIndices = GetFragmentBasedRegimens(sentenceFragments);
            if(!regimenIndices.Any() && IsSingleOrNoDataPoint(sentenceFragments))
            {
                var measureREs = sentenceFragments.SelectMany(sf => sf.RecognizedEntities.Where(re => re.IsMeasure).Select(re => re)).ToList();
                foreach (var measureRE in measureREs)
                {
                    measureRE.Singularize();
                }
            }
        }

        /// <summary>
        /// no time mentioned will be assumed as the last month where data is available
        /// </summary>
        /// <param name="sentenceFragments"></param>
        /// <returns></returns>
        protected bool IsSingleOrNoDataPoint(List<SentenceFragment> sentenceFragments)
        {
            var temporalREs = sentenceFragments.SelectMany(sf => sf.
            RecognizedEntities.Where(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION4, true) == 0));
            //return true if no temporal entity
            if (!temporalREs.AnyOrNotNull())
                return true;
            //return true if no end in any temporal entity, i.e. single data point
            temporalREs = temporalREs.Where(re => re.Value as DateRange != null && 
            (re.Value as DateRange)?.Start != null && (re.Value as DateRange)?.End != null);
            return !temporalREs.AnyOrNotNull();
        }

        protected RuleValidationResult ApplyMeasureRules(ref List<SentenceFragment> sentenceFragments)
        {
            var res = new RuleValidationResult();
            var measureIndices = GetFragmentBasedMeasures(sentenceFragments);

            //if no measure/metrics found, default to share
            if (!measureIndices.Any())
            {
                foreach (var fragment in sentenceFragments)
                {
                    var defaultMeasure = new RecognizedEntity
                    {
                        Entity = new Measure
                        {
                            DomainName = CAConstants.DEFAULT_MEASURE
                        },
                        RecognizedName = CAConstants.DEFAULT_MEASURE
                    };
                    fragment.RecognizedEntities.Add(defaultMeasure);
                }
            }
            else
            {
                //measureEntity = first detected measure
                RecognizedEntity measureEntity = null;
                for (int j = 0; j < measureIndices.Count; j++)
                {
                    int i = measureIndices[j];
                    foreach (var re in sentenceFragments[i].RecognizedEntities)
                    {
                        if (!re.IsMeasure)
                            continue;
                        if (measureEntity == null)
                            measureEntity = re;
                        else
                        {
                            //if multiple measures - invalid
                            if (String.Compare(re.Entity.DomainName, measureEntity.Entity.DomainName, true) != 0)
                            {
                                throw new ChartAuditMultipleMeasureException();
                                //return res;
                            }
                            else
                            {
                                //if same measure is repeated as plural form, plural is preferred
                                if (measureEntity.IsPlural)
                                    re.Pluralize();
                                else if (re.IsPlural)
                                    measureEntity.Pluralize();
                            }
                        }
                    }
                }
            }

            return res;
        }

        protected List<int> GetFragmentBasedMeasures(List<SentenceFragment> sentenceFragments)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < sentenceFragments.Count; i++)
            {
                if (sentenceFragments[i].RecognizedEntities.Any(re => re.Entity as Measure != null))
                    res.Add(i);
            }

            return res;
        }

        protected List<int> GetFragmentBasedTumors(List<SentenceFragment> sentenceFragments)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < sentenceFragments.Count; i++)
            {
                if (sentenceFragments[i].RecognizedEntities.Any(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION1_COMPONENT1, true) == 0))
                    res.Add(i);
            }

            return res;
        }

        protected List<int> GetFragmentBasedRegimens(List<SentenceFragment> sentenceFragments)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < sentenceFragments.Count; i++)
            {
                if (sentenceFragments[i].RecognizedEntities.Any(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION2, true) == 0))
                    res.Add(i);
            }

            return res;
        }

        protected List<int> GetFragmentBasedTemporals(List<SentenceFragment> sentenceFragments)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < sentenceFragments.Count; i++)
            {
                if (sentenceFragments[i].RecognizedEntities.Any(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION4, true) == 0 && 
                !String.IsNullOrEmpty(re.Value?.ToString())))
                    res.Add(i);
            }

            return res;
        }
        
        protected Dictionary<NERClause, List<SentenceFragment>> sentenceFragments;
        protected bool isSingleDataPoint; //is it single datetime or a datetime range?
        protected Visualization chartType;
        
        protected ParsedResult ParseAsync(string sentence)
        {
            try
            {
                return new Chrone(null, new Config { Direction = DateTimeDirection.Backward }).Parse(sentence)[0];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// date time parser
        /// </summary>
        /// <param name="fromTable"></param>
        /// <param name="sentence"></param>
        /// <returns>1st item in the list is the input sentence excluding temporal part, rest of the items are the parsed temporal strings</returns>
        protected TemporalDetail ParseTemporal(string sentence)
        {
            List<string> res = new List<string>();
            TemporalDetail td = new TemporalDetail();
            string replacement = String.Empty;
            //using chrono
            var parsedTemoralPart = ParseAsync(sentence);
            try
            {
                if (parsedTemoralPart != null)
                {
                    if (!String.IsNullOrEmpty(parsedTemoralPart.Text))
                    {
                        string match = parsedTemoralPart.Text;
                        if (parsedTemoralPart.Start != null)
                        {
                            td.Range = new DateRange(parsedTemoralPart.Start.Date, parsedTemoralPart.End?.Date);
                            td.Replacement = FormatTemporal(td.Range);
                            td.Sentence = Regex.Replace(sentence, "\\b" + Regex.Escape(match) + "\\b", String.Empty, RegexOptions.IgnoreCase);
                            td.Tags = parsedTemoralPart.Tags.Keys.ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            td.TemporalText = parsedTemoralPart?.Text;
            return td;
        }
        
        public override abstract Visualization ResolveChartTypeFromActualData(List<SentenceFragment> dataSlices);

        protected string FormatTemporal(DateRange rng, DateTime? additionalDateTimeReference = null)
        {
            string replacement = String.Empty;
            //date format - date is always set to 01 as chartaudit deals with only month and above
            if (rng.End != null && rng.End.HasValue)
            {
                replacement = String.Format("{0} ge '{1}' {0} le '{2}'", CAConstants.DIMENSION4,
                    rng.Start.Value.ToString("yyyy-MM-01"), rng.End.Value.ToString("yyyy-MM-01")).Replace("$", "$$");
            }
            else if (additionalDateTimeReference != null && additionalDateTimeReference.HasValue)
            {
                replacement = String.Format("{0} ge '{1}' {0} le '{2}'", CAConstants.DIMENSION4,
                    additionalDateTimeReference.Value.ToString("yyyy-MM-01"), rng.Start.Value.ToString("yyyy-MM-01")).Replace("$", "$$");
            }
            else
            {
                replacement = String.Format("{0} is '{1}'", CAConstants.DIMENSION4, rng.Start.Value.ToString("yyyy-MM-01")).Replace("$", "$$");
            }

            return replacement;
        }

        public override RecognitionOutput ResolveDimensions(NaturalLanguageQuestion question, Database databaseObject)
        {
            RecognitionOutput output = new RecognitionOutput();
            var thesaurus = databaseObject.Thesaurus;

            try
            {
                if (question.RelatedQuestions.AnyOrNotNull())
                {
                    //try resolving unknown operations
                    PredictRefiners(question);
                }
                List<SentenceFragment> unionFragments = null;

                //if no table contaning any match candidate, throw no data exception
                if (databaseObject.Tables == null || !databaseObject.Tables.Any(tbl => tbl.Columns != null &&
                 tbl.Columns.Any(col => col.MatchCandidates.AnyOrNotNull())))
                    throw new ChartAuditNoDataException();
                var originalQuestion = question.Question;
                RefineQuestion(question);
                var contributorSentences = SplitQuestionFragments(question.Question/*originalQuestion*/, thesaurus);
                if (contributorSentences.AnyOrNotNull())
                {
                    var reevaluatedFragments = AccumulateEntities(contributorSentences, databaseObject);

                    if (reevaluatedFragments.AnyOrNotNull())
                    {
                        unionFragments = reevaluatedFragments[NERClause.EQUAL];
                        NormalizeEntities(ref unionFragments, -1/*GetBaseFragmentIndex(unionFragments)*/);
                        if (unionFragments == null)
                            return null; //invalid
                        FlattenForSingleDimensionInstances(unionFragments);
                        AddTemporalEntities(question.Question, unionFragments);
                        var result = ApplyRules(ref unionFragments);
                        reevaluatedFragments[NERClause.EQUAL] = unionFragments;
                        if (!reevaluatedFragments.ContainsKey(NERClause.NOTEQUAL))
                            reevaluatedFragments.Add(NERClause.NOTEQUAL, new List<SentenceFragment>());
                        output.SentenceFragments = reevaluatedFragments;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return output;
        }
        
        //TODO : use NLP libraries to find which part can be considered as the base fragment
        //e.g. "get me shares for sclc 2l for last 6 months including keytruda shares", the base fragment here 
        //can be considered to be "get me shares for sclc 2l for last 6 months"
        private int GetBaseFragmentIndex(List<SentenceFragment> unionFragments)
        {
            for (int i =0; i < unionFragments.Count; i++)
            {
                if(unionFragments[i].HasMeasure())
                    return i;
            }

            return -1;
        }

        private void AddTemporalEntities(string question, List<SentenceFragment> fragments)
        {
            //replace the time spans, if any
            var parsedTemporal = ParseTemporal(GetUnrecognizedFragment(question, fragments));
            RecognizedEntity temporalNer = null;
            if (parsedTemporal != null && !String.IsNullOrEmpty(parsedTemporal.Replacement))
            {
                temporalNer = new RecognizedEntity
                {
                    Entity = new Time { PreferDataAvailability = parsedTemporal.Tags.Contains("ENCasualMonthParser") },
                    Value = parsedTemporal.Range,
                    RecognizedValue = parsedTemporal.Range
                };
            }

            //add temporal ner, if any, to each of the identified sentence fragments
            if (temporalNer != null)
            {
                foreach (var fragment in fragments)
                {
                    fragment.RecognizedEntities.Add(temporalNer);
                }
            }
        }

        private void RefineReplaceableFragments(ref List<SentenceFragment> fragments, NaturalLanguageQuestion question, Database db)
        {
            if (!question.RelatedQuestions.AnyOrNotNull())
                return;
            var replaceFragments = question.RelatedQuestions.
                Where(rq => rq.Operation == Operation.Replace).
                Select(rq => rq.Question).
                ToList();
            if (!replaceFragments.AnyOrNotNull())
                return;
            var clausedRefineReplace = SplitReplaceFragments(replaceFragments);
            var refineReplaceFragments = AccumulateEntities(clausedRefineReplace, db);
            if (!refineReplaceFragments.ContainsKey(NERClause.EQUAL))
                return;
            RefineReplaceQuestionText(question, clausedRefineReplace);
            //if no replacee present, e.g. silent dimensions
            if (!refineReplaceFragments.ContainsKey(NERClause.NOTEQUAL))
            {
                for (int i = 0; i < fragments.Count; i++)
                {
                    List<SentenceFragment> sfs = new List<SentenceFragment>();
                    foreach (var sf in refineReplaceFragments[NERClause.EQUAL])
                    {
                        var clonedFragment = fragments[i].Clone() as SentenceFragment;
                        clonedFragment.ReplaceNERs(sf);
                        sfs.Add(clonedFragment);
                    }
                    fragments.RemoveAt(i);
                    fragments.InsertRange(i, sfs);
                    i += sfs.Count - 1;
                }
            }
            else
            {
                for (int i = 0; i < fragments.Count; i++)
                {
                    foreach (var re in fragments[i].RecognizedEntities)
                    {
                        int idx = refineReplaceFragments[NERClause.NOTEQUAL].
                            FindIndex(fr => fr.RecognizedEntities.Contains(re, new NERComparer()));

                        if (idx >= 0)
                        {
                            //if notequal idx is beyonf equal idx, reset to last equal idx
                            if (idx >= refineReplaceFragments[NERClause.EQUAL].Count)
                                idx = refineReplaceFragments[NERClause.EQUAL].Count - 1;
                            fragments[i].ReplaceNERs(refineReplaceFragments[NERClause.EQUAL][idx]);
                            //pop out the last replaced item until there is only 1 replacer remaining
                            if (refineReplaceFragments[NERClause.EQUAL].Count > 1)
                                refineReplaceFragments[NERClause.EQUAL].RemoveAt(idx);
                            break;
                        }
                    }
                }

                //include the residuals
                for (int j = 1; j < refineReplaceFragments[NERClause.EQUAL].Count; j++)
                {
                    var sf = refineReplaceFragments[NERClause.EQUAL][j];
                    var fr = fragments.Last().Clone() as SentenceFragment;
                    fr.ReplaceNERs(sf);
                    fragments.Add(sf);
                    refineReplaceFragments[NERClause.EQUAL].RemoveAt(j);
                }
            }
        }

        private void RefineReplaceQuestionText(NaturalLanguageQuestion question, Dictionary<NERClause, List<string>> clausedRefineReplace)
        {
            var replacers = clausedRefineReplace[NERClause.EQUAL];
            if (!clausedRefineReplace.ContainsKey(NERClause.NOTEQUAL) || !clausedRefineReplace[NERClause.NOTEQUAL].AnyOrNotNull())
            {
                //TODO : the append should be with last word in the text and any non-word char like ? etc. should be added back at the very last
                question.Question += " for " + String.Join(" and ", replacers);
            }
            else
            {
                var replacees = clausedRefineReplace[NERClause.NOTEQUAL];
                if (replacers.Count == 1 && replacees.Count > 0)
                {
                    Regex regEx = new Regex("\\b" + String.Join("|", replacees) + "\\b", RegexOptions.IgnoreCase);
                    question.Question = regEx.Replace(question.Question, replacers[0]);
                }
                else if (replacees.Count == 1 && replacers.Count > 0)
                {
                    Regex regEx = new Regex("\\b" + replacees[0] + "\\b", RegexOptions.IgnoreCase);
                    question.Question = regEx.Replace(question.Question, String.Join(" and ", replacers));
                }
                else if (replacers.Count == replacees.Count)
                {
                    for (int i = 0; i < replacees.Count; i++)
                    {
                        Regex regEx = new Regex("\\b" + replacees[i] + "\\b", RegexOptions.IgnoreCase);
                        question.Question = regEx.Replace(question.Question, replacers[i]);
                    }
                }
                else
                {
                    //your wish!
                }
            }
        }
        
    
        protected string GetTumorNameFromData(SentenceFragment tumorData)
        {
            string tumorName = String.Empty;
            string tumorGroupName = tumorData.GetEntityValueByFieldName(CAConstants.DIMENSION1_COMPONENT1).SafeTrim();
            if (!String.IsNullOrEmpty(tumorGroupName))
            {
                tumorName += tumorGroupName;
                string lineName = tumorData.GetEntityValueByFieldName(CAConstants.DIMENSION1_COMPONENT2).SafeTrim();
                if (!String.IsNullOrEmpty(lineName))
                    tumorName += " " + lineName;

                string segmentName = tumorData.GetEntityValueByFieldName(CAConstants.DIMENSION1_COMPONENT3).SafeTrim();
                if (!String.IsNullOrEmpty(segmentName) && String.Compare(segmentName, CAConstants.DEFAULT_SEGMENT, true) != 0)
                    tumorName += " " + segmentName;

                string subSegmentName = tumorData.GetEntityValueByFieldName(CAConstants.DIMENSION1_COMPONENT4).SafeTrim();
                if (!String.IsNullOrEmpty(subSegmentName) && String.Compare(subSegmentName, CAConstants.DEFAULT_SUBSEGMENT, true) != 0)
                    tumorName += " " + subSegmentName;

                string testStatus = tumorData.GetEntityValueByFieldName(CAConstants.DIMENSION1_COMPONENT5).SafeTrim();
                if (!String.IsNullOrEmpty(testStatus) && String.Compare(subSegmentName, CAConstants.DEFAULT_TEST_STATUS, true) != 0)
                    tumorName += " " + testStatus;
            }

            return tumorName;
        }
        
        protected bool IsSingleDataPoint(List<SentenceFragment> fragments)
        {
            var dtList = fragments.Select(fr => fr.GetEntityValueByFieldName(CAConstants.DIMENSION4).SafeTrim()).
                Where(my => my.ToFormattedDateTimeStr("MMM yyyy") != null).
                Distinct().
                ToList();
            return dtList.Count == 1;
        }
        
        public override string GetDefaultDimensionOrder()
        {
            return String.Format("order by {0}", CAConstants.DIMENSION4);
        }

        private string GetUnrecognizedFragment(string question, List<SentenceFragment> fragments)
        {
            string unrecognizedPart = question;
            foreach (var item in fragments)
            {
                foreach (var re in item.RecognizedEntities)
                {
                    unrecognizedPart = unrecognizedPart.Replace((re.RecognizedValue ?? re.RecognizedName).ToString(), String.Empty); //TODO: consider using a regex to replace word including word-break that follows
                }
            }

            return unrecognizedPart;
        }

        private Dictionary<NERClause, List<string>> SplitReplaceFragments(List<string> replaceFragments)
        {
            Dictionary<NERClause, List<string>> res = new Dictionary<NERClause, List<string>>();
            foreach (var replaceFragment in replaceFragments)
            {
                var replaceRegEx = new Regex(@"\bwith\b", RegexOptions.IgnoreCase);
                var replaceContainer = replaceRegEx.Split(replaceFragment).ToList();
                var subtracts = Regex.Split(replaceContainer[0], @"\sAND\s", RegexOptions.IgnoreCase).
                    Select(sf => sf.SafeTrim()).
                    Where(sf => !String.IsNullOrWhiteSpace(sf)).
                    ToList();
                replaceContainer.RemoveAt(0);
                var unions = replaceContainer.SelectMany(rep => Regex.Split(rep, @"\sAND\s", RegexOptions.IgnoreCase).
                    Select(sf => sf.SafeTrim()).
                    Where(sf => !String.IsNullOrWhiteSpace(sf))).
                    ToList();
                res.AddOrUpdate(NERClause.EQUAL, unions);
                res.AddOrUpdate(NERClause.NOTEQUAL, subtracts);
            }

            return res;
        }

        private Dictionary<NERClause, List<string>> SplitQuestionFragments(string question, Thesaurus thesaurus)
        {
            question = thesaurus.EncodePhrases(question);
            var result = new Dictionary<NERClause, List<string>>();
            //natural language AND is equivalent to query OR; 
            //e.g. opdivo and keytruda => regimen is opdivo or regimen is keytruda
            //e.g. opdivo nsclc 1l and nsclc 2l => regimen is opdivo and tumor is nsclc 1l or tumor is nsclc 2l
            //question = Regex.Replace(question, " AND ", " OR ", RegexOptions.IgnoreCase);

            //first, attempt to get what's in the removal set, and cut them down from the full question
            List<string> subtractions = PredictSubtractedPart(ref question);
            //the modified question now is expected to have only the addendums
            string unionKeyword = String.Format("\\s({0}|{1})\\s", GetIncludingKeyword(), GetConjunctionKeyword());
            var unions = Regex.Split(question, unionKeyword, RegexOptions.IgnoreCase|RegexOptions.ExplicitCapture).
                Select(sf => sf.SafeTrim()).
                Where(sf => !String.IsNullOrWhiteSpace(sf)).
                ToList();
            foreach (var union in unions)
            {
                if (!String.IsNullOrWhiteSpace(union))
                    result.AddOrUpdate(NERClause.EQUAL, thesaurus.Decode(union));
            }
            foreach (var subtraction in subtractions)
            {
                var conjugateSubtractions = Regex.Split(subtraction, GetConjunctionKeyword(), RegexOptions.IgnoreCase).
                    Select(sf => sf.SafeTrim()).
                    Where(sf => !String.IsNullOrWhiteSpace(sf)).
                    ToList();
                foreach (var conjugateSubtraction in conjugateSubtractions)
                {
                    result.AddOrUpdate(NERClause.NOTEQUAL, thesaurus.Decode(conjugateSubtraction));
                }
            }

            //var unions = Regex.Split(question, @"\sAND\s", RegexOptions.IgnoreCase).
            //    Where(sf => !String.IsNullOrWhiteSpace(sf.SafeTrim())).
            //    ToList();
            //var subtractRegEx = new Regex(@"\bwithout|excluding|not\shaving\b", RegexOptions.IgnoreCase);
            //foreach (var union in unions)
            //{
            //    var unionsubtractParts = subtractRegEx.Split(union);
            //    if(!String.IsNullOrWhiteSpace(unionsubtractParts[0]))
            //        result.AddOrUpdate(NERClause.EQUAL, unionsubtractParts[0]);
            //    for(int i = 1; i < unionsubtractParts.Count(); i++)
            //    {
            //        if (!String.IsNullOrWhiteSpace(unionsubtractParts[i]))
            //            result.AddOrUpdate(NERClause.NOTEQUAL, unionsubtractParts[i]);
            //    }
            //}

            return result;
        }

        private string GetConjunctionKeyword()
        {
            return "AND";
        }

        private List<string> PredictSubtractedPart(ref string question)
        {
            //in the following regex: .*? is for non-greedy search (stops at 1st match), ?<= is for positive look-behind (excludes the phrase that's 
            //matched as prefix), ?= positive look-ahead (excludes the pjrase that's matched as suffix)
            var subtractRegEx = new Regex(String.Format(@"\b({0})\b(.*?)((?=\band\s{1}\b)|(\s(\w+)$))", GetExcludingKeyword(), GetIncludingKeyword()), 
                RegexOptions.IgnoreCase);
            var subtractables = subtractRegEx.Matches(question)?.Cast<Match>().Select(m => m.Value).ToList();
            question = subtractRegEx.Replace(question, String.Empty)?.Trim();
            return subtractables;
        }

        private void ApplyNERClause(List<SentenceFragment> unionFragments, List<SentenceFragment> reevaluatedFragments)
        {
            foreach (var uf in unionFragments)
            {
                foreach (var ufNER in uf.RecognizedEntities)
                {
                    if (ufNER.Clause != NERClause.EQUAL)
                    {
                        foreach (var reevaluatedFragment in reevaluatedFragments)
                        {
                            var reevaluatedNER = reevaluatedFragment.RecognizedEntities.
                                Where(re => String.Compare(re.Entity.DomainName, ufNER.Entity.DomainName, true) == 0).
                                FirstOrDefault();
                            if (reevaluatedNER != null)
                                reevaluatedNER.Clause = ufNER.Clause;
                        }
                    }
                }
            }
        }

        private void RefineQuestion(NaturalLanguageQuestion question)
        {
            if (question.RelatedQuestions != null)
            {
                bool endsWithQuestionMark = false;
                if (question.Question.EndsWith("?"))
                {
                    endsWithQuestionMark = true;
                    question.Question = question.Question.TrimEnd(new char[] { '?' });
                }
                string trimmedQuestion = question.Question.Trim();
                //add/remove all matching entities for refine
                foreach (var relatedQuestion in question.RelatedQuestions)
                {   
                    string includer = GetIncludingKeyword();
                    string excluder = GetExcludingKeyword();
                    var includerPattern = new Regex(String.Format("\\b({0})\\s+{1}\\b", includer, Regex.Escape(relatedQuestion.Question)), 
                        RegexOptions.IgnoreCase);
                    var excluderPattern = new Regex(String.Format("\\b({0})\\s+{1}\\b", excluder, Regex.Escape(relatedQuestion.Question)),
                        RegexOptions.IgnoreCase);

                    if (relatedQuestion.Operation == Operation.Add)
                    {
                        //if trying to include an already excluded one, remove the exclude statement
                        if (excluderPattern.IsMatch(trimmedQuestion))
                            RemoveExcluder(ref trimmedQuestion, excluderPattern);
                        //if trying to include an already included one, no action needed
                        if (!includerPattern.IsMatch(trimmedQuestion))
                        {
                            var randomIncluder = CAConstants.INCLUDERS[new Random().Next(0, CAConstants.INCLUDERS.Count - 1)];
                            trimmedQuestion += " " + randomIncluder + " " + relatedQuestion.Question;
                        }
                    }
                    if (relatedQuestion.Operation == Operation.Remove)
                    {
                        //if trying to exclude an already included one, remove the include statement
                        if (includerPattern.IsMatch(trimmedQuestion))
                            RemoveIncluder(ref trimmedQuestion, includerPattern);
                        //if trying to exclude an already excluded one, no action needed
                        if (!excluderPattern.IsMatch(trimmedQuestion))
                        {
                            var randomExcluder = CAConstants.EXCLUDERS[new Random().Next(0, CAConstants.EXCLUDERS.Count - 1)];
                            trimmedQuestion += " " + randomExcluder + " " + relatedQuestion.Question;
                        }
                    }
                }
                question.Question = trimmedQuestion;
                if (endsWithQuestionMark)
                    question.Question += " ?";
            }
        }

        private void RemoveExcluder(ref string question, Regex excluderPattern)
        {
            question = excluderPattern.Replace(question, String.Empty)?.Trim();
        }

        private void RemoveIncluder(ref string question, Regex includerPattern)
        {
            question = includerPattern.Replace(question, String.Empty)?.Trim();
        }

        private string GetExcludingKeyword()
        {
            List<string> excluderKeywords = CAConstants.EXCLUDERS;
            string excluder = String.Empty;
            foreach (var excluderKeyword in excluderKeywords)
            {
                excluder += "|" + Regex.Escape(excluderKeyword);
            }
            
            return excluder.Trim(new char[] { '|' });
        }

        private string GetIncludingKeyword()
        {
            List<string> includerKeywords = CAConstants.INCLUDERS;
            string includer = String.Empty;
            foreach (var includerKeyword in includerKeywords)
            {
                includer += "|" + Regex.Escape(includerKeyword);
            }

            return includer.Trim(new char[] { '|' });
        }
        
        private void RemoveRefinementsFromQuestion(ref string question, List<SentenceFragment> unionFragments, RecognizedEntity ner, int stringInsertOffset)
        {
            ner.Clause = NERClause.NOTEQUAL;
            var regEx = new Regex("( and )?" + ner.Value.ToString() + "( and)?", RegexOptions.IgnoreCase);
            if (regEx.Match(question).Success)
                question = regEx.Replace(question, String.Empty);
            else
                question += " excluding " + ner.Value.ToString();
            var ufNer = unionFragments.SelectMany(fr => fr.RecognizedEntities).
                Where(re => String.Compare(re.Entity.DomainName, ner.Entity.DomainName, true) == 0).
                SingleOrDefault();
            if (ufNer != null)
                ufNer.Clause = NERClause.NOTEQUAL;
            else
            {
                foreach (var fr in unionFragments)
                {
                    fr.RecognizedEntities.Add(ner);
                }
            }
        }
        
        private SortedDictionary<int, string> RefineFragmentsWithNer(List<SentenceFragment> unionFragments, RecognizedEntity ner)
        {
            var refinedImpacts = new SortedDictionary<int, string>();
            foreach (var fragment in unionFragments)
            {
                var lastFoundRE = fragment.RecognizedEntities.
                    Where(re => String.Compare(re.Entity.DomainName, ner.Entity.DomainName, true) == 0).
                    Last();
                int reIndex = -1;
                if (lastFoundRE != null && lastFoundRE.RecognizedValue != null)
                    reIndex = lastFoundRE.Index + lastFoundRE.RecognizedValue.ToString().Length;
                if (refinedImpacts.ContainsKey(reIndex))
                    refinedImpacts[reIndex] += " " + ner.RecognizedValue ?? String.Empty;
                else
                    refinedImpacts.Add(reIndex, ner.RecognizedValue?.ToString());
            }

            return refinedImpacts;
        }

        public string BuildQuestionInKnownFormat(Database databaseObject, Dictionary<NERClause, List<SentenceFragment>> allFragements)
        {
            string res = String.Empty;
            string whereClause = String.Empty;
            //keep only the first from table expression, rest are unnecessary
            if (!String.IsNullOrWhiteSpace(DbEntity))
            {
                //build a string.format statement
                string unionPart = BuildKnownFormatWhereClause(allFragements[NERClause.EQUAL], "OR");
                string subtractionPart = BuildKnownFormatWhereClause(allFragements[NERClause.NOTEQUAL], "AND");
                whereClause = unionPart + " " + subtractionPart;
            }

            string measure = allFragements[NERClause.EQUAL].
                SelectMany(sf => sf.RecognizedEntities).
                Where(re => re.Entity is Measure).
                Select(re => re.Entity.FieldName).
                FirstOrDefault();
            res = String.Format("{0},{1} for from {2}{3}", String.Join(",", NamedEntityDimensions.Select(
                ned => ned.DomainName)), measure, DbEntity, whereClause);

            return res;
        }

        private string BuildKnownFormatWhereClause(List<SentenceFragment> fragments, string delimiter)
        {
            if (!fragments.AnyOrNotNull())
                return String.Empty;
            string prefix = String.Empty;
            List<string> contributors = new List<string>();
            for (int i = 0; i < fragments.Count; i++)
            {
                string mid = String.Empty;
                string suffix = String.Empty;
                List<string> formatValues = new List<string>();
                for (int j = 0; j < fragments[i].RecognizedEntities.Count; j++)
                {
                    var entity = fragments[i].RecognizedEntities[j];
                    //unionContributors[i] = unionContributors[i].Trim();

                    //if needed, re-parse time dimension
                    if (IsTemporalEntity(entity))
                    {
                        var dtRange = entity.Value as DateRange;
                        DateTime? additionalTimeComponent = null;
                        //adding additional time component for generating narrations for Pie chart
                        if (chartType == Visualization.Pie)
                            additionalTimeComponent = dtRange.Start.Value.AddMonths(-1);
                        suffix = " " + FormatTemporal(dtRange, additionalTimeComponent);
                    }
                    else if (!(entity.Entity is Measure))
                    {
                        mid += entity.NLClause;
                    }
                }
                //replace format key-value
                contributors.Add(String.Format("{0}{1}", mid, suffix));
            }

            //include components names as well
            return String.Join(String.Format(" {0} ", delimiter), contributors);
        }

        private bool IsTemporalEntity(RecognizedEntity entity)
        {
            return String.Compare(entity.Entity.DomainName, NamedEntityDimensions.Last().DomainName, true) == 0;
        }

        private void PredictRefiners(NaturalLanguageQuestion question)
        {
            var operations = Enum.GetValues(typeof(Operation)).Cast<Operation>();
            foreach (var operation in operations)
            {
                if (operation != Operation.Unknown)
                    PredictSplitBasedRefiners(question, operation);
            }
        }

        private void PredictSplitBasedRefiners(NaturalLanguageQuestion question, Operation operation)
        {
            string operationKeyword = operation.ToString();
            List<string> operationKeywords = new List<string> { operationKeyword };
            var operationKeywordPattern = new Regex("\\b" + String.Join("|", operationKeywords.Select(kw => Regex.Escape(kw))) + "\\b", RegexOptions.IgnoreCase);
            var unknownRefiners = question.RelatedQuestions.Where(rq => rq.Operation == Operation.Unknown).Select(rq => rq.Question).ToList();
            foreach (var unknownRefiner in unknownRefiners)
            {
                if (operationKeywordPattern.Match(unknownRefiner).Success)
                {
                    var operatedFragments = operationKeywordPattern.Split(unknownRefiner);
                    foreach (var operatedFragment in operatedFragments)
                    {
                        if (String.IsNullOrWhiteSpace(operatedFragment))
                            continue;
                        question.RelatedQuestions.Add(new Refiner { Operation = operation, Question = operatedFragment.Trim() });
                    }
                }
            }
        }

        private void FilterOutSubtractionFragments(ref List<string> unionContributors, ref List<SentenceFragment> unionFragments,
            List<SentenceFragment> subtractionFragments)
        {
            for (int i = 0; i < unionFragments.Count; i++)
            {
                foreach (var subtractionFragment in subtractionFragments)
                {
                    if (unionFragments[i].IsEquivalent(subtractionFragment))
                        unionFragments.RemoveAt(i--);
                }
            }
        }

        public sealed override List<string> GetAllEntityPhrases()
        {
            var db = StaticResources.GetInstance().Database;
            var uniquePhraseList = new HashSet<string>();
            try
            {
                if (!db.Tables.AnyOrNotNull())
                    throw new ChartAuditNoDataException();

                var tbl = db.GetTableByName(DbEntity);
                if (!tbl.Columns.AnyOrNotNull())
                    throw new ChartAuditNoDataException();
                //named entities
                foreach (var col in tbl.Columns)
                {
                    if (col.MatchCandidates.AnyOrNotNull())
                    {
                        foreach (var phrase in col.MatchCandidates)
                        {
                            if (!uniquePhraseList.Contains(phrase))
                                uniquePhraseList.Add(phrase);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //log
            }
            return uniquePhraseList.ToList();
        }
        
        private Dictionary<NERClause, List<SentenceFragment>> AccumulateEntities(Dictionary<NERClause, List<string>> entityContributors, Database db)
        {
            if (entityContributors == null || db == null)
                return null;
            var fragments = new Dictionary<NERClause, List<SentenceFragment>>();
            
            foreach (var kvp in entityContributors)
            {
                foreach (var item in kvp.Value)
                {
                    var ners = new List<RecognizedEntity> { identifiedMeasure };

                    Table tbl = db.GetTableByName(DbEntity);

                    if (!tbl.Columns.AnyOrNotNull() || !tbl.Columns.Any(col => col.MatchCandidates.AnyOrNotNull()))
                        throw new ChartAuditNoDataException();
                    //named entities
                    foreach (var col in tbl.Columns)
                    {
                        foreach (var match in col.MatchCandidates)
                        {
                            ners.Add(new RecognizedEntity
                            {
                                Entity = new NED
                                {
                                    DomainName = col.Name
                                },
                                Value = match.Trim()
                            });
                        }
                    }

                    ners = AccumulateDimensionsInFragment(item, kvp.Key, db.Thesaurus, ners);
                    if (!ners.AnyOrNotNull())
                        ners = new List<RecognizedEntity> { identifiedMeasure };

                    if (!fragments.ContainsKey(kvp.Key))
                        fragments.Add(kvp.Key, new List<SentenceFragment>());
                    var clausedFragments = fragments[kvp.Key];
                    clausedFragments.Add(new SentenceFragment { RecognizedEntities = ners });
                }
            }

            //FlattenForSingleDimensionInstances(fragments);
            ApplyCompoundDataRule(fragments, entityContributors.Values.SelectMany(v => v).ToList()); //just comment this line if you do not need the compound data feature
            return fragments;
        }

        private void ApplyCompoundDataRule(Dictionary<NERClause, List<SentenceFragment>> fragments, List<string> sentencesToLookIn)
        {
            var additionalFragments = new Dictionary<NERClause, List<SentenceFragment>>();
            foreach (var kvp in fragments)
            {
                foreach (var sf in kvp.Value)
                {
                    foreach (var re in sf.RecognizedEntities)
                    {
                        if (re.IsNamedEntity)
                        {
                            if (!IsIndividual(re.RecognizedValue, sentencesToLookIn))
                            {
                                var compoundData = GetCompoundData(re);
                                if (compoundData.AnyOrNotNull())
                                {
                                    foreach (var ner in compoundData)
                                    {
                                        var clonedFragment = sf.Clone() as SentenceFragment;
                                        clonedFragment.AddOrReplaceNERs(new List<RecognizedEntity> { ner });
                                        additionalFragments.AddOrUpdate(kvp.Key, clonedFragment);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //now include the additional fragments in the original set of fragments
            if (additionalFragments.AnyOrNotNull())
            {
                foreach (var kvp in additionalFragments)
                {
                    fragments[kvp.Key].AddRange(kvp.Value.ToList());
                }
            }
        }

        /// <summary>
        /// need to use a new table in db instead of the following hardcoded values!
        /// </summary>
        /// <param name="ner"></param>
        /// <returns></returns>
        protected abstract List<RecognizedEntity> GetCompoundData(RecognizedEntity ner);

        private bool IsIndividual(object recognizedValue, List<string> sentencesToLookIn)
        {
            string pattern = "((\\b" + String.Join("|", GetIndividualityKeywords()) + "\\b)\\s+\\b" + Regex.Escape(recognizedValue.ToString()) + "\\b)|" +
                             "(\\b" + Regex.Escape(recognizedValue.ToString()) + "\\b\\s+(\\b" + String.Join("|", GetIndividualityKeywords()) + "\\b))";
            return sentencesToLookIn.Any(sentence => new Regex(pattern, RegexOptions.IgnoreCase).
            IsMatch(sentence));
        }

        private string[] GetIndividualityKeywords()
        {
            return new string[] { "only", "mono", "exclusively", "just", "nothing other than", "specifically" };
        }

        private void FlattenForSingleDimensionInstances(List<SentenceFragment> fragments)
        {
            for (int j = 0; j < fragments.Count; j++)
            {
                var fragment = fragments[j];
                var nersInFrgment = new Dictionary<string, List<RecognizedEntity>>();
                for (int i = 0; i < fragment.RecognizedEntities.Count; i++)
                {
                    var ner = fragment.RecognizedEntities[i];
                    if (ner.Entity is NED)
                    {
                        string entityName = ner.Entity.DomainName;
                        string entityValue = ner.Value.SafeTrim();
                        if (!nersInFrgment.ContainsKey(entityName))
                            nersInFrgment.Add(entityName, new List<RecognizedEntity> { ner });
                        else
                        {
                            //if same ner duplicated remove duplicate from fragment, 
                            //else if same entity occurs multiple times in a single fragment, remove from fragment but collect it for flattening operation
                            if (!nersInFrgment[entityName].Any(n => String.Compare(n.Value.SafeTrim(), entityValue, true) == 0))
                                nersInFrgment[entityName].Add(ner);
                            fragment.RecognizedEntities.Remove(ner);
                            i--;
                            if (nersInFrgment[entityName].Count == 2) //just when there is a different entity of same type found in same fragment
                                fragment.RecognizedEntities.Remove(nersInFrgment[entityName][0]);
                        }
                    }
                }

                //flattening operation
                var reCombos = GetAllllCombos(0, nersInFrgment.Values.Where(reList => reList.Count > 1).ToList());
                if (reCombos.AnyOrNotNull())
                {
                    fragments.RemoveAt(j--);
                    foreach (var reCombo in reCombos)
                    {
                        var fragmentClone = fragment.Clone() as SentenceFragment;
                        fragmentClone.RecognizedEntities.AddRange(reCombo);
                        fragments.Add(fragmentClone);
                        j++;
                    }
                }
            }
        }

        private List<List<RecognizedEntity>> GetAllllCombos(int entityLevel, List<List<RecognizedEntity>> nersInFrgment)
        {
            if (!nersInFrgment.AnyOrNotNull())
                return null;
            List<List<RecognizedEntity>> res = null;
            if (entityLevel == nersInFrgment.Count - 1)
            {
                res = new List<List<RecognizedEntity>>();
            }
            else
            {
                res = GetAllllCombos(entityLevel + 1, nersInFrgment);
            }

            foreach (var ner in nersInFrgment[entityLevel])
            {
                res.Add(new List<RecognizedEntity> { ner });
            }
            return res;
        }

        public static List<RecognizedEntity> AccumulateDimensionsInFragment(string fragment, NERClause clause, Thesaurus thesaurus, 
            List<RecognizedEntity> ners)
        {
            List<RecognizedEntity> resNERs = new List<RecognizedEntity>();
            foreach (var ner in ners)
            {
                try
                {
                    var candidateNer = MatchWordInFragment(ner, fragment, thesaurus);
                    //if the candidate ner detected is duplicate, skip it
                    if (candidateNer != null && !resNERs.Any(e => e.Overlaps(candidateNer)))
                    {
                        //if a ner is contained in it remove the already existing ner - i.e. take max of overlapping ners
                        resNERs.Remove(resNERs.FirstOrDefault(e => candidateNer.Overlaps(e)));
                        candidateNer.Clause = clause;
                        resNERs.Add(candidateNer);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return resNERs;
        }

        private static RecognizedEntity MatchWordInFragment(RecognizedEntity ner, string fragment, Thesaurus thesaurus)
        {
            RecognizedEntity resolvedNer = null;
            string key = null;
            string actualMatch = null;
            string recognizedMatch = null;

            if (ner.IsNamedEntity)
            {
                key = ner.Entity.DomainName;
                actualMatch = ner.Value.ToString();
                recognizedMatch = ner.Value.ToString();
            }
            else if (ner.IsMeasure)
            {
                actualMatch = ner.Entity.DomainName;
            }
            if (!String.IsNullOrWhiteSpace(actualMatch))
            {

                Match m = Regex.Match(fragment, String.Format("{0}{1}{2}", Util.WORD_BOUNDARY_START, Regex.Escape(actualMatch),
                                                Util.WORD_BOUNDARY_END), RegexOptions.IgnoreCase);

                if (!m.Success)
                {
                    List<string> synonymousMatches = new List<string>();
                    if (thesaurus != null && thesaurus.Dictionary.ContainsKey(actualMatch))
                        synonymousMatches.AddRange(thesaurus.Dictionary[actualMatch]);
                    foreach (var synonymousMatch in synonymousMatches)
                    {
                        m = Regex.Match(fragment, String.Format("{0}{1}{2}", Util.WORD_BOUNDARY_START, Regex.Escape(synonymousMatch),
                                                     Util.WORD_BOUNDARY_END), RegexOptions.IgnoreCase);
                        if (!m.Success)
                            m = MatchPlural(fragment, synonymousMatch);
                        if (m.Success)
                        {
                            //replace synonymous match with actual match
                            recognizedMatch = m.Value;
                            Regex.Replace(fragment, synonymousMatch, actualMatch, RegexOptions.IgnoreCase);
                            break;
                        }
                    }
                }
                if (!m.Success)
                    m = MatchPlural(fragment, actualMatch);

                if (m.Success)
                {
                    recognizedMatch = m.Value;
                    if (ner.IsNamedEntity)
                        resolvedNer = new RecognizedEntity { Entity = ner.Entity, Value = actualMatch, RecognizedValue = recognizedMatch ?? actualMatch, Index = m.Index };
                    else if (ner.IsMeasure)
                        resolvedNer = new RecognizedEntity { Entity = ner.Entity, RecognizedName = recognizedMatch ?? actualMatch, Index = m.Index };
                }
            }

            return resolvedNer;
        }

        private static Match MatchPlural(string sentence, string word)
        {
            CultureInfo ci = new CultureInfo("en-us");
            PluralizationService ps =
              PluralizationService.CreateService(ci);

            var recognizedMatch = ps.Pluralize(word);
            var m = Regex.Match(sentence, String.Format("{0}{1}{2}", Util.WORD_BOUNDARY_START, Regex.Escape(recognizedMatch),
                                        Util.WORD_BOUNDARY_END), RegexOptions.IgnoreCase);

            return m;
        }
    }
    
}