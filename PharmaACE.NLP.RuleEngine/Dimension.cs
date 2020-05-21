using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    /// <summary>
    /// abstract class for any field
    /// </summary>
    public abstract class Dimension
    {
        public virtual string FieldName { get { return DomainName; } }
        public virtual string DomainName { get; set; }
        //public bool IsRoot { get; set; }
        public virtual object Default { get; set; }
        public List<Dimension> Dependencies { get; set; }
    }

    /// <summary>
    /// Named Entity Dimension
    /// e.g. textual dimensions like Tumor. Regimen etc..
    /// </summary>
    public class NED : Dimension
    {
        /// <summary>
        /// components that make up the dimension
        /// </summary>
        public virtual List<NED> Components { get; set; }
    }

    /// <summary>
    /// Measures
    /// e.g. any kind of metrics - shares, rolling shares, testing rates etc..
    /// </summary>
    public class Measure : Dimension
    {
    }

    /// <summary>
    /// a custom date range class to server time dimension defaults
    /// </summary>
    public class DateRange
    {
        public DateTime? Start { get; private set; }
        public DateTime? End { get; private set; }

        public DateRange()
        {
            Start = null;
            End = null;
        }
        public DateRange(DateTime? start, DateTime? end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}
