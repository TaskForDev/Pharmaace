using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    abstract class CAChartEngine : ChartEngineBase
    {
        public override abstract ChartBase GetChart(Visualization viz);
    }
}
