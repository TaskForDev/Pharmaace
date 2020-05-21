using System.Collections.Generic;
using System.Data;

namespace PharmaACE.NLP.Framework
{
    public abstract class ChartBase
    {
        public abstract Visualization ChartType { get; protected set; }
        public abstract string Caption { get; protected set; }
        public abstract string Narrative { get; protected set; }
        public abstract void Populate(List<SentenceFragment> dataSlices);
        
    }
}
