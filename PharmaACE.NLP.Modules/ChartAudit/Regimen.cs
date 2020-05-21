using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D2 : Regimen
    /// </summary>
    public class Regimen : NED
    {
        public override string FieldName { get { return CAConstants.DIMENSION2; } }
        public override string DomainName { get { return CAConstants.DIMENSION2; } }
    }
}
