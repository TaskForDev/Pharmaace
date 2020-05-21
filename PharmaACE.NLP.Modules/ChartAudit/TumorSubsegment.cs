using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// C4 : SubSegment
    /// </summary>
    public class TumorSubsegment : NED
    {
        public TumorSubsegment(string defaultSubSegment)
        {
            Default = defaultSubSegment;
        }
        public override string FieldName { get { return CAConstants.DIMENSION1_COMPONENT4; } }
        public override string DomainName { get { return CAConstants.DIMENSION1_COMPONENT4; } }
    }
}
