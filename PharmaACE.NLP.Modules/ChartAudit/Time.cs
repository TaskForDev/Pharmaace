using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D4 : Monthyear
    /// </summary>
    public class Time : NED
    {
        public override string FieldName { get { return CAConstants.DIMENSION4; } }
        public override string DomainName { get { return CAConstants.DIMENSION4; } }
        public bool PreferDataAvailability { get; set; }
    }
}
