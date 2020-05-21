using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D3 : Share
    /// </summary>
    public class R2MShare : Measure
    {
        public override string FieldName { get { return CAConstants.MEASURE2_FIELD; } }
        public override string DomainName { get { return CAConstants.MEASURE2; } }
    }
}
