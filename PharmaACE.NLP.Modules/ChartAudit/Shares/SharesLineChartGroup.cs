using System;
using System.Collections.Generic;
using System.Linq;
using PharmaACE.NLP.Framework;
using PharmaACE.Utility;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public class SharesLineChartGroup : CAChartBase
    {
        const int NARRATION_STYLE_CHANGEPOINT = 3; //max number of line charts to show diff with previous month
        public List<LineChart> LineCharts { get; set; }

        public SharesLineChartGroup(Visualization viz)
        {
            ChartType = viz;
        }

        public override void Populate(List<SentenceFragment> dataSlices)
        {
            //bool isPanTumor = ChartAuditRuleEngine.GetTumors(sentenceFragments[0]).Count == 0;
            var measureRE = dataSlices.
                SelectMany(sf => sf.RecognizedEntities).
                Where(re => re.IsMeasure).
                FirstOrDefault();
            string measureName = measureRE.Entity.DomainName;
            string measureColumn = measureRE.Entity.FieldName;
            string measureRecognizeName = measureRE.RecognizedName;
            if (String.IsNullOrEmpty(measureName))
                return;
            string captionEntityName = null;
            var lineChartMap = new Dictionary<string, LineChart>();
            int count = 0;
            foreach (var row in dataSlices)
            {
                count++;
                string legend = null;
                var tumorName = TumorNames?[count - 1];
                //if pan tumor only include pan tumor members
                //if (IsPanTumor && !PAN_TUMOR_MEMBERS.Contains(tumorName))
                //    continue;
                var regimenName = row.RecognizedEntities.
                Where(re => re.Entity is Regimen).
                Select(re => re.Value.SafeTrim()).
                FirstOrDefault();
                if (ChartType == Visualization.LineSingleRegimen)
                {
                    legend = tumorName;
                    captionEntityName = regimenName;
                }
                else if (ChartType == Visualization.LineSingleTumor)
                {
                    legend = regimenName;
                    captionEntityName = tumorName;
                }
                else
                {
                    legend = tumorName + " " + regimenName;
                    captionEntityName = String.Empty;
                }
                if (String.IsNullOrWhiteSpace(legend))
                    return;


                string monthYear = row.RecognizedEntities.
                Where(re => re.Entity is Time).
                Select(re => re.Value).
                FirstOrDefault().
                ToFormattedDateTimeStr("MMM yyyy");
                if (String.IsNullOrEmpty(monthYear))
                    return;

                LineChart lineChart = null;
                if (!lineChartMap.ContainsKey(legend))
                    lineChartMap.Add(legend, new LineChart { Legend = legend, DataPoints = new List<DataPoint>() });
                lineChart = lineChartMap[legend];
                var measureVal = row.RecognizedEntities.Where(re => re.Entity is Measure).Select(re => re.Value.SafeToDouble()).FirstOrDefault();
                lineChart.DataPoints.Add(new DataPoint { Abscissa = monthYear, Ordinate = measureVal });
            }

            Caption = String.Format("{0} {1}", IsPanTumor ? CAConstants.PAN_TUMOR : captionEntityName, measureRecognizeName).Trim();
            LineCharts = lineChartMap.Values.ToList();
            Arrange(LineCharts);
            BuildNarrative();
        }

        private void Arrange(List<LineChart> lineCharts)
        {
            lineCharts.RemoveAll(lc => String.Compare(lc.Legend, CAConstants.RESIDUAL_SLICE, true) == 0);
        }

        private void BuildNarrative()
        {
            Narrative = Caption.Trim() + " ";
            List<string> narratives = new List<string>();
            string abscissa = null;
            foreach (var lineChart in LineCharts)
            {
                var lastDP = lineChart.DataPoints.Last();
                if (LineCharts.Count <= NARRATION_STYLE_CHANGEPOINT)
                {
                    var secondLastDP = lineChart.DataPoints.Reverse<DataPoint>().Skip(1).Take(1).FirstOrDefault();
                    var changeVal = Math.Round(lastDP.Ordinate - secondLastDP.Ordinate, CAConstants.CHART_LABEL_PRECISION);
                    var changePhrase = changeVal > 0 ? "increased" : "decreased";
                    if (String.Compare(abscissa, lastDP.Abscissa, true) == 0)
                    {
                        narratives.Add(String.Format("{0} by {1}% from previous month for {2}", changePhrase,
                        Math.Abs(changeVal), lineChart.Legend));
                    }
                    else
                    {
                        narratives.Add(String.Format("in {0} {1} by {2}% from previous month for {3}", lastDP.Abscissa, changePhrase,
                            Math.Abs(changeVal), lineChart.Legend));
                        abscissa = lastDP.Abscissa;
                    }
                }
                else
                {
                    if (String.Compare(abscissa, lastDP.Abscissa, true) == 0)
                    {
                        narratives.Add(String.Format("{0}% for {1}", Math.Round(lastDP.Ordinate, CAConstants.CHART_LABEL_PRECISION), lineChart.Legend));
                    }
                    else
                    {
                        narratives.Add(String.Format("in {0} is {1}% for {2}", lastDP.Abscissa, Math.Round(lastDP.Ordinate, CAConstants.CHART_LABEL_PRECISION), lineChart.Legend));
                        abscissa = lastDP.Abscissa;
                    }
                }
            }

            Narrative += String.Join(", ", narratives);
        }
    }

}
