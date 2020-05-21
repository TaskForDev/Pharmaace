using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    class CATRChartEngine : CAChartEngine
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
                    chart = new TRLineChartGroup(viz);
                    break;
                case Visualization.StackedBar:
                    chart = new TRStackedBarChart();
                    break;
                case Visualization.Pie:
                    chart = new TRPieChartGroup();
                    break;
                default:
                    break;
            }

            return chart;
        }
    }
}
