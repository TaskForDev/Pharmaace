using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D3 : Testing Rate
    /// </summary>
    public class IR : Measure
    {
        public override string FieldName { get { return CAConstants.MEASURE1_FIELD; } }
        public override string DomainName { get { return CAConstants.MEASURE7; } }
    }
}
