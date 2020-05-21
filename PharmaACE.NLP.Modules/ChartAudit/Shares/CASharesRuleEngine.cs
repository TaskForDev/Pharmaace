using System;
using System.Collections.Generic;
using System.Linq;
using PharmaACE.NLP.Framework;
using PharmaACE.Utility;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    class CASharesRuleEngine : CARuleEngine
    {
        public CASharesRuleEngine(RecognizedEntity measureRE)
        {
            SetNamedEntityDimensions();
            identifiedMeasure = measureRE;
        }

        public override string DbEntity
        {
            get
            {
                return CAConstants.SHARE_VIEW;
            }
        }
        
        protected override void SetNamedEntityDimensions()
        {
            //the order the dimesions are added to the list is important, the normalization scan will be according to this order
            //1st attempt to resolve defaults is done by NormalizeEntities, then by ApplyRules (in case the default itself depends on rules)
            var tg = new TumorGroup();
            var tl = new TumorLine { Dependencies = new List<Dimension> { tg } };
            var ts = new TumorSegment(CAConstants.DEFAULT_SEGMENT) { Dependencies = new List<Dimension> { tg } };
            var tss = new TumorSubsegment(CAConstants.DEFAULT_SUBSEGMENT) { Dependencies = new List<Dimension> { tg} };
            var tts = new TumorTestStatus { Dependencies = new List<Dimension> { tg } };
            NamedEntityDimensions = new List<NED> {
                    new Tumor { Components = new List<NED> { tg, tl, ts, tss, tts } },
                    new Regimen(),
                    new Time()
                };
        }
        
        public override Visualization ResolveChartTypeFromActualData(List<SentenceFragment> dataSlices)
        {
            bool areMultipleTumor = false, areMultipleRegimen = false;
            string lastTumorName = null, lastRegimenName = null;
            //time domension is assumed to be same in all fragments
            isSingleDataPoint = IsSingleDataPoint(dataSlices);
            foreach (var sf in dataSlices)
            {
                string tumorName = GetTumorNameFromData(sf)?.Trim();
                if (!String.IsNullOrWhiteSpace(tumorName))
                {
                    if (String.IsNullOrWhiteSpace(lastTumorName))
                        lastTumorName = tumorName;
                    else if (String.Compare(tumorName, lastTumorName, true) != 0)
                    {
                        areMultipleTumor = true;
                        if (areMultipleRegimen)
                        {
                            if (isSingleDataPoint)
                                break;
                            else
                                throw new ChartAuditMultiTumorMultiRegimenTimeSeriesException();
                        }
                    }
                }

                string regimenName = sf.GetEntityValueByFieldName(CAConstants.DIMENSION2).SafeTrim();
                if (!String.IsNullOrWhiteSpace(regimenName))
                {
                    if (String.IsNullOrWhiteSpace(lastRegimenName))
                        lastRegimenName = regimenName;
                    else if (String.Compare(regimenName, lastRegimenName, true) != 0)
                    {
                        areMultipleRegimen = true;
                        if (areMultipleTumor)
                        {
                            if (isSingleDataPoint)
                                break;
                            else
                                throw new ChartAuditMultiTumorMultiRegimenTimeSeriesException();
                        }
                    }
                }
            }

            if (areMultipleTumor)
            {
                if (isSingleDataPoint)
                    chartType = Visualization.StackedBar;
                else
                    chartType = Visualization.LineSingleRegimen;
            }
            else //if (areMultipleRegimen)
            {
                if (isSingleDataPoint)
                    chartType = Visualization.Pie;
                else
                    chartType = Visualization.LineSingleTumor;
            }
            //else
            //{
            //    if (isSingleDataPoint)
            //        chartType = Visualization.Pie;
            //    else
            //        chartType = Visualization.LineSingleTumor;
            //}

            return chartType;
        }
        
        internal static List<string> GetRegimenNames(Dictionary<NERClause, List<SentenceFragment>> sentenceFragments)
        {
            var includedRegimenNames = GetRegimenNamesFromClausedFragments(sentenceFragments[NERClause.EQUAL]);
            var excludedRegimenNames = GetRegimenNamesFromClausedFragments(sentenceFragments[NERClause.NOTEQUAL]);
            return includedRegimenNames.Except(excludedRegimenNames).ToList();
        }

        private static List<string> GetRegimenNamesFromClausedFragments(List<SentenceFragment> sentenceFragments)
        {
            return sentenceFragments.SelectMany(sf => sf.RecognizedEntities.Where(re => String.Compare(re.Entity.DomainName, CAConstants.DIMENSION2, true) == 0).Select(re => re?.RecognizedValue.ToString())).
                                     Distinct().
                                     ToList();
        }
        
        /// <summary>
        /// need to use a new table in db instead of the following hardcoded values!
        /// </summary>
        /// <param name="ner"></param>
        /// <returns></returns>
        protected override List<RecognizedEntity> GetCompoundData(RecognizedEntity ner)
        {
            var associatedData = new List<RecognizedEntity>();
            var associatedValues = new List<string>();
            switch (ner.Value.ToString().ToLower())
            {
                case "keytruda":
                    associatedValues = new List<string> { "K+Y regimen", "K+Chemo" };
                    foreach (var val in associatedValues)
                    {
                        var clonedNER = ner.Clone() as RecognizedEntity;
                        clonedNER.Value = val;
                        clonedNER.RecognizedValue = val;
                        associatedData.Add(clonedNER);
                    }
                    break;
                case "opdivo":
                    associatedValues = new List<string> { "O+Y regimen", "O+Chemo" };
                    foreach (var val in associatedValues)
                    {
                        var clonedNER = ner.Clone() as RecognizedEntity;
                        clonedNER.Value = val;
                        clonedNER.RecognizedValue = val;
                        associatedData.Add(clonedNER);
                    }
                    break;
                default:
                    break;
            }

            return associatedData;
        }
    }
}