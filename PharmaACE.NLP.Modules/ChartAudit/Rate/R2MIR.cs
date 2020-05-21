using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D3 : Testing Rate rolling 2 months
    /// </summary>
    public class R2MIR : Measure
    {
        public override string FieldName { get { return CAConstants.MEASURE2_FIELD; } }
        public override string DomainName { get { return CAConstants.MEASURE8; } }
    }
}
