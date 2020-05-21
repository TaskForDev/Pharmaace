using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    class CAIRChartEngine : CAChartEngine
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
                    chart = new IRLineChartGroup(viz);
                    break;
                case Visualization.StackedBar:
                    chart = new IRStackedBarChart();
                    break;
                case Visualization.Pie:
                    chart = new IRPieChartGroup();
                    break;
                default:
                    break;
            }

            return chart;
        }
    }
}
