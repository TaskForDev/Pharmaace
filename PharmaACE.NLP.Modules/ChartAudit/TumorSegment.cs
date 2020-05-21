using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// C3 : Segment
    /// </summary>
    public class TumorSegment : NED
    {
        string defaultSegment;
        public TumorSegment(string defaultSegment = CAConstants.DEFAULT_SEGMENT)
        {
            this.defaultSegment = defaultSegment;
        }
        public override string FieldName { get { return CAConstants.DIMENSION1_COMPONENT3; } }
        public override string DomainName { get { return CAConstants.DIMENSION1_COMPONENT3; } }
        public override object Default { get { return defaultSegment; } }
    }
}
