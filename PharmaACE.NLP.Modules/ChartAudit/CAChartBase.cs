using System.Collections.Generic;
using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public abstract class CAChartBase : ChartBase
    {   
        protected readonly HashSet<string> PAN_TUMOR_MEMBERS = new HashSet<string> {
                "NSCLC 1L"
                ,"NSCLC 2L"
                ,"ADJ Melanoma"
                ,"Melanoma 1L"
                ,"HCC 2L"
                ,"RCC 2L"
                //,"SCCHN PP*"
            };
        public override Visualization ChartType { get; protected set; }
        public bool IsPanTumor { get; set; }
        public List<string> TumorNames { get; set; }
        public List<string> RegimenNames { get; set; }
        public override string Caption { get; protected set; }
        public override string Narrative { get; protected set; }
        public override abstract void Populate(List<SentenceFragment> dataSlices);
        
    }
}
