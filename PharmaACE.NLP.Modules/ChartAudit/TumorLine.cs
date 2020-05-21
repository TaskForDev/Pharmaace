using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// C2 : Line
    /// </summary>
    public class TumorLine : NED
    {
        public TumorLine(){}
        public TumorLine(string defaultLine)
        {
            Default = defaultLine;
        }
        public override string FieldName { get { return CAConstants.DIMENSION1_COMPONENT2; } }
        public override string DomainName { get { return CAConstants.DIMENSION1_COMPONENT2; } }
    }
}
