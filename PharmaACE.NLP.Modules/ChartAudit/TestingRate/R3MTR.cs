using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D3 : Testing Rate rolling 3 months
    /// </summary>
    public class R3MTR : Measure
    {
        public override string FieldName { get { return CAConstants.MEASURE3_FIELD; } }
        public override string DomainName { get { return CAConstants.MEASURE6; } }
    }
}
