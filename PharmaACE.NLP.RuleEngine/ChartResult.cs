namespace PharmaACE.NLP.Framework
{
    public class ChartResult
    {
        public string RealizedQuery { get; set; }
        public int Status { get; set; }
        //public List<ChartAuditData> CAData { get; set; }
        public ChartBase Chart { get; set; }
    }
}
