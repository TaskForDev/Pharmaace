using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    class CASharesChartEngine : CAChartEngine
    {
        public override ChartBase GetChart(Visualization viz)
        {
            ChartBase chart = null;
            switch (viz)
            {
                case Visualization.None:
                    break;
                case Visualization.Table:
                    break;
                case Visualization.LineSingleRegimen:
                case Visualization.LineSingleTumor:
                case Visualization.LineMultiTumorMultiRegimen:
                    chart = new SharesLineChartGroup(viz);
                    break;
                case Visualization.StackedBar:
                    chart = new SharesStackedBarChart();
                    break;
                case Visualization.Pie:
                    chart = new SharesPieChartGroup();
                    break;
                default:
                    break;
            }

            return chart;
        }
    }
}
