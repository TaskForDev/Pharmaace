using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PharmaACE.NLP.Framework;
using PharmaACE.Utility;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public class IRStackedBarChart : CAChartBase
    {
        public List<StackedBar> StackedBars { get; set; }

        public IRStackedBarChart()
        {
            ChartType = Visualization.StackedBar;
        }

        public override void Populate(List<SentenceFragment> dataSlices)
        {
            string monthYear = dataSlices[0].RecognizedEntities.
                Where(re => re.Entity is Time).
                Select(re => re.Value.ToFormattedDateTimeStr("MMM yyyy")).
                FirstOrDefault();
            //bool isPanTumor = ChartAuditRuleEngine.GetTumors(sentenceFragments[0]).Count == 0;
            var measureRE = dataSlices[0].RecognizedEntities.Where(re => re.IsMeasure).FirstOrDefault();
            string measureName = measureRE.Entity.DomainName;
            string measureField = measureRE.Entity.FieldName;
            string measureRecognizedName = measureRE.RecognizedName;
            if (String.IsNullOrEmpty(measureField))
                return;
            HashSet<string> tumorNames = new HashSet<string>();
            var stackedBarMap = new Dictionary<string, StackedBar>();
            HashSet<string> legends = new HashSet<string>();
            int count = 0;
            foreach (var row in dataSlices)
            {
                count++;
                string tumorName = TumorNames[count - 1];
                if (!tumorNames.Contains(tumorName))
                    tumorNames.Add(tumorName);
                StackedBar stackedBar = null;
                if (!stackedBarMap.ContainsKey(tumorName))
                    stackedBarMap.Add(tumorName, new StackedBar { Abscissa = tumorName, Stacks = new List<Ordinate>() });
                stackedBar = stackedBarMap[tumorName];
                var measureVal = row.RecognizedEntities.
                    Where(re => re.Entity is Measure).
                    Select(re => re.Value.SafeToDouble()).
                    FirstOrDefault();
                stackedBar.Stacks.Add(new Ordinate
                {
                    Legend = String.Empty,
                    Value = measureVal
                });
            }

            StackedBars = stackedBarMap.Values.ToList();
            //the following code is added because of the limitation of the front end chart library to identify distinct legends from stacked bars correctly
            foreach (var legend in legends)
            {
                var stackedBarsMissingThisLegend = StackedBars.Where(sb => !sb.Stacks.Select(sbs => sbs.Legend).Contains(legend));
                foreach (var stackedBar in stackedBarsMissingThisLegend)
                {
                    stackedBar.Stacks.Add(new Ordinate { Legend = legend, Value = 0 });
                }
            }

            string captionStr = IsPanTumor ? CAConstants.PAN_TUMOR : (legends.Count == 1 ? legends.First() : String.Join(", ", tumorNames));
            Caption = String.Format("{0} {1} in {2}", captionStr, measureRecognizedName, monthYear);
            StackedBars.ForEach(sb => sb.Stacks = sb.Stacks.OrderBy(sbs => sbs.Legend).ToList());
            Arrange(StackedBars);
            BuildNarrative(monthYear, measureName);
        }

        private void Arrange(List<StackedBar> stackedBars)
        {   
            foreach (var sb in stackedBars)
            {
                //adjust residual
                double residualVal = Math.Round(100 - sb.Stacks.Sum(sbs => sbs.Value), CAConstants.CHART_LABEL_PRECISION);
                if (residualVal > 0)
                    AdjustResidual(sb, residualVal);
                //slide "Others" stack to the bottom
                var residualStack = sb.Stacks.Where(sbs => String.Compare(sbs.Legend, CAConstants.RESIDUAL_SLICE, true) == 0).FirstOrDefault();
                if (residualStack != null)
                {
                    sb.Stacks.Remove(residualStack);
                    sb.Stacks.Add(residualStack);
                }
            }
        }

        private void AdjustResidual(StackedBar stackedBar, double residualVal)
        {
            var residualStack = stackedBar.Stacks.Where(s => String.Compare(s.Legend, CAConstants.RESIDUAL_SLICE) == 0).FirstOrDefault();
            if (residualStack == null)
                stackedBar.Stacks.Add(new Ordinate { Legend = CAConstants.RESIDUAL_SLICE, Value = Math.Round(residualVal, CAConstants.CHART_LABEL_PRECISION) });
            else
                residualStack.Value += Math.Round(residualVal, CAConstants.CHART_LABEL_PRECISION);
        }

        private void BuildNarrative(string monthYear, string measureName)
        {
            int distinctLegendCount = StackedBars.SelectMany(sb => sb.Stacks.Select(sbs => sbs.Legend)).Distinct().Count();
            List<string> narratives = new List<string>();
            if (distinctLegendCount == 1)
            {
                for (int i = 0; i < StackedBars.Count; i++)
                {
                    string narrative = null;
                    if (StackedBars[i] == null)
                        continue;
                    for (int j = 0; j < StackedBars[i].Stacks.Count; j++)
                    {
                        if (StackedBars[i].Stacks[j] == null)
                            continue;
                        if (narratives.Count == 0 && !String.IsNullOrEmpty(StackedBars[i].Stacks[j].Legend))
                            narrative = String.Format("{0} {1} for {2} is ", StackedBars[i].Stacks[j].Legend, measureName, monthYear);
                        if (!String.IsNullOrEmpty(narrative) || narratives.Count > 0)
                        {
                            narrative += String.Format("{0}% for {1}", StackedBars[i].Stacks[j].Value, StackedBars[i].Abscissa);
                            narratives.Add(narrative);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < StackedBars.Count; i++)
                {
                    string narrative = null;
                    if (StackedBars[i] == null)
                        continue;
                    var maxStacks = StackedBars[i].Stacks.
                        OrderByDescending(sbs => sbs.Value).
                        GroupBy(g => g.Value).
                        Select(g => g.Select(gv => gv)).
                        FirstOrDefault().
                        ToList();
                    if (maxStacks.Count > 0)
                    {
                        narrative = String.Format("In {0} highest {1} is {2}% for {3}", StackedBars[i].Abscissa, measureName,
                            maxStacks[0].Value, String.Join(",", maxStacks.Select(o => o.Legend)));
                        narratives.Add(narrative);
                    }
                }
            }

            Narrative = String.Join(", ", narratives);
        }

    }

}
