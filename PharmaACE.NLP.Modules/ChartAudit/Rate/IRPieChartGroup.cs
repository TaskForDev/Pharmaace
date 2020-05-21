using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PharmaACE.NLP.Framework;
using PharmaACE.Utility;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public class IRPieChartGroup : CAChartBase
    {
        public List<PieChart> PieCharts { get; set; }

        public IRPieChartGroup()
        {
            ChartType = Visualization.Pie;
        }

        public override void Populate(List<SentenceFragment> dataSlices)
        {
            //since data fetched is always ordered by datetime ascending, the last row will give the latest date
            //the rest of the datetimes are for narrations
            string monthYear = dataSlices[dataSlices.Count - 1].RecognizedEntities.
                Where(re => re.Entity is Time).
                Select(re => re.Value.ToFormattedDateTimeStr("MMM yyyy")).
                FirstOrDefault();
            string monthYearForDiff = dataSlices[0].RecognizedEntities.
                Where(re => re.Entity is Time).
                Select(re => re.Value.ToFormattedDateTimeStr("MMM yyyy")).
                FirstOrDefault();
            PieCharts = Populate(dataSlices, monthYear);
            List<PieChart> pieChartsForDiff = null;
            if (String.Compare(monthYear, monthYearForDiff, true) != 0)
                pieChartsForDiff = Populate(dataSlices, monthYearForDiff);
            BuildNarrative(pieChartsForDiff);
        }

        private void BuildNarrative(List<PieChart> pieChartsForDiff)
        {
            Narrative = String.Empty;
            for (int i = 0; i < PieCharts.Count; i++)
            {
                var pieChart = PieCharts[i];
                foreach (var sliceRes in pieChart.Slices)
                {
                    if (String.Compare(CAConstants.RESIDUAL_SLICE, sliceRes.Legend, true) == 0)
                        continue;
                    
                    string unformattedString = null;
                    string subject = null;
                    if (String.IsNullOrWhiteSpace(Narrative))
                    {
                        unformattedString = "{0} for {1} is {2}%";
                        subject = pieChart.Caption;
                    }
                    else
                    {
                        subject = "it";
                        unformattedString = " For {1} {0} is {2}%";
                    }
                    double diff;
                    string diffStr = null;
                    if (pieChartsForDiff.AnyOrNotNull())
                    {
                        var sliceDiff = pieChartsForDiff[i]?.Slices?.Where(sd => String.Compare(sd.Legend, sliceRes.Legend, true) == 0).FirstOrDefault();
                        if (sliceDiff != null)
                        {
                            //if both prev and current are 0, exclude it
                            //if (sliceRes.Value == 0 && sliceDiff.Value == 0)
                            //    continue;
                            string increaseOrDecrease = sliceRes.Value > sliceDiff.Value ? "increase" : "decrease";
                            diff = Math.Round(Math.Abs(sliceRes.Value - sliceDiff.Value), CAConstants.CHART_LABEL_PRECISION);
                            if (diff == 0)
                                diffStr = " which remained same as previous month.";
                            else
                                diffStr = " which is a {0}% {1} from previous month.";
                            //}
                            Narrative += String.Format(unformattedString,
                                subject,
                                sliceRes.Legend,
                                sliceRes.Value) +
                                String.Format(diffStr,
                                diff,
                                increaseOrDecrease);
                        }
                    }
                    else //no diff if month itself is the last month available in data
                        Narrative += String.Format(unformattedString,
                            subject,
                            sliceRes.Legend,
                            sliceRes.Value);
                }
            }
        }

        private List<PieChart> Populate(List<SentenceFragment> dataSlices, string monthYear)
        {
            List<PieChart> pieCharts = null;
            //bool isPanTumor = ChartAuditRuleEngine.GetTumors(sentenceFragments[0]).Count == 0;
            var measureRE = dataSlices[0].RecognizedEntities.Where(re => re.IsMeasure).FirstOrDefault();
            string measureName = measureRE.Entity.DomainName;
            string measureColumn = measureRE.Entity.FieldName;
            string measureRecognizedName = measureRE.RecognizedName;
            if (String.IsNullOrEmpty(measureName))
                return null;

            double residualVal = 100;
            string legendColumn = null;
            if (TumorNames.Distinct().Count() == 1)
            {
                legendColumn = CAConstants.DIMENSION1_COMPONENT3;
            }
            else
            {
                legendColumn = CAConstants.DIMENSION1_COMPONENT1;
            }
            pieCharts = new List<PieChart> { new PieChart { Caption = String.Format("{0} {1} {2} in {3}", TumorNames[0],
                    String.Empty, measureRecognizedName, monthYear), Slices = new List<PieSlice>() } };

            foreach (var row in dataSlices)
            {
                var sliceMonthYear = row.RecognizedEntities.
                    Where(re => re.Entity is Time).
                    Select(re => re.Value.ToFormattedDateTimeStr("MMM yyyy"))
                    .FirstOrDefault();
                if (String.Compare(monthYear, sliceMonthYear, true) == 0)
                {
                    string tumorName = row.RecognizedEntities.
                Where(re => String.Compare(re.Entity.FieldName, legendColumn, true) == 0).
                Select(RE => RE.Value.SafeTrim()).
                FirstOrDefault();
                    if (String.IsNullOrEmpty(tumorName))
                        return null;
                    double measureVal = row.RecognizedEntities.
                        Where(re => re.IsMeasure).
                        Select(re => re.Value.SafeToDouble()).
                        FirstOrDefault();
                    residualVal -= measureVal;
                    pieCharts[0].Slices.Add(new PieSlice { Legend = String.Empty, Value = measureVal });
                }

            }
            if (residualVal > 0)
                pieCharts[0].Slices.Add(new PieSlice { Legend = CAConstants.RESIDUAL_SLICE, Value = Math.Round(residualVal, CAConstants.CHART_LABEL_PRECISION) });

            Arrange(pieCharts);
            return pieCharts;
        }

        private void Arrange(List<PieChart> pieCharts)
        {
            //slide "Others" slice to the end
            if (pieCharts.AnyOrNotNull())
            {
                var residualSlice = pieCharts[0].Slices.Where(s => String.Compare(s.Legend, CAConstants.RESIDUAL_SLICE) == 0).FirstOrDefault();
                if(residualSlice != null)
                {
                    pieCharts[0].Slices.Remove(residualSlice);
                    pieCharts[0].Slices.Add(residualSlice);
                }
            }
        }

        private void AdjustResidual(List<PieChart> pieCharts, double residualVal)
        {
            var residualSlice = pieCharts[0].Slices.Where(s => String.Compare(s.Legend, CAConstants.RESIDUAL_SLICE) == 0).FirstOrDefault();
            if (residualSlice == null)
                pieCharts[0].Slices.Add(new PieSlice { Legend = CAConstants.RESIDUAL_SLICE, Value = Math.Round(residualVal, CAConstants.CHART_LABEL_PRECISION) });
            else
                residualSlice.Value += Math.Round(residualVal, CAConstants.CHART_LABEL_PRECISION);
        }
    }

}
