using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    public class TemporalResult
    {
        public DateTime? Reference { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public Dictionary<string, bool> Tags { get; set; }
        public Dictionary<string, int> Start { get; set; }
        public Dictionary<string, int> End { get; set; }

        public TemporalResult()
        {
            Tags = new Dictionary<string, bool>();
            //Start = new Dictionary<string, int>();
            //End = new Dictionary<string, int>();
        }
    }
}
