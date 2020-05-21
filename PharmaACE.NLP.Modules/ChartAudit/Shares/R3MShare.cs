using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D3 : Share
    /// </summary>
    public class R3MShare : Measure
    {
        public override string FieldName { get { return CAConstants.MEASURE3_FIELD; } }
        public override string DomainName { get { return CAConstants.MEASURE3; } }
    }
}
