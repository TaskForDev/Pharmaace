using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// C3 : Segment
    /// </summary>
    public class TumorTestStatus : NED
    {
        public override string FieldName { get { return CAConstants.DIMENSION1_COMPONENT5; } }
        public override string DomainName { get { return CAConstants.DIMENSION1_COMPONENT5; } }
        public override object Default { get { return CAConstants.DEFAULT_TEST_STATUS; } }
    }
}
