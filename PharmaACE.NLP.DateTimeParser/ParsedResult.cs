using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    public class ParsedResult
    {
        public DateTime? Reference { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public Dictionary<string, bool> Tags { get; set; }
        public ParsedComponents Start { get; set; }
        public ParsedComponents End { get; set; }

        public ParsedResult(TemporalResult result)
        {
            result = result ?? new TemporalResult();

            Reference = result.Reference;
            Index = result.Index;
            Text = result.Text;
            Tags = result.Tags;
            Start = new ParsedComponents(result.Start, result.Reference);
            if(result.End != null)
                End = new ParsedComponents(result.End, result.Reference);
        }

        public bool HasPossibleDates
        {
            get
            {
                return Start.IsPossibleDate && (End == null || End.IsPossibleDate);
            }
        }
    }
}
