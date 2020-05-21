using System;
using System.Collections.Generic;
using System.Linq;
using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    /// <summary>
    /// D1 : Tumor
    /// </summary>
    public class Tumor : NED
    {
        public override string FieldName { get { return CAConstants.DIMENSION1_COMPONENT1; } }
        public override string DomainName { get { return String.Join(",", Components.Select(c => c.DomainName)); } }
        public override List<NED> Components { get; set; }
    }
}
