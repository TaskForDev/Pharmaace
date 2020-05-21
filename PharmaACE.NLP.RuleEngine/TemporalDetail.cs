using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    public class TemporalDetail
    {
        public DateRange Range { get; set; }
        public string TemporalText { get; set; }
        public string Sentence { get; set; }
        public string Replacement { get; set; }
        public List<string> Tags { get; set; }
    }

    /// <summary>
    /// parsed datetime from natural language
    /// </summary>
    public class NLDT
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Text { get; set; }
    }
}
