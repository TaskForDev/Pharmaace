using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENNumericYearParser : Parser
    {
        public ENNumericYearParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern
        {
            get
            {
                //19th and 20th centuries only
                return new Regex(@"(\b)(19|20)\d{2}(\b)", RegexOptions.IgnoreCase);
            }
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt)
        {
            var text = match.Value.Trim();
            if (String.IsNullOrWhiteSpace(text))
                return null;
            var result = new ParsedResult(new TemporalResult { Index = match.Index, Text = text, Reference = reference });
            int year;
            if (int.TryParse(text, out year))
            {
                result.Start.Assign("day", 1);
                result.Start.Assign("month", 1);
                result.Start.Assign("year", year);
                result.End = result.Start.Clone() as ParsedComponents;
                result.End.Assign("month", 12);
                result.Tags[this.GetType().Name] = true;
            }
            else
                return null;

            return result;
        }
    }
}