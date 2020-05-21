using MomentSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    class ENCasualDateParser : Parser
    {
        public ENCasualDateParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern {
            get {
                return new Regex(@"(\W|^)(now|today|tonight|last\s* night|(?:tomorrow|tmr|yesterday)\s*|tomorrow|tmr|yesterday)(?=\W|$)",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt)
        {
            var text = match.Groups[0].Value.Substring(match.Groups[1].Length);
            var index = match.Index + match.Groups[1].Length;
            var result = new ParsedResult(new TemporalResult { Index = index, Text = text, Reference = reference });
            var refMoment = GetMoment(reference);
            var startMoment = GetMoment(reference);
            var lowerText = text.ToLower();
            if (String.Compare(lowerText, "tonight") == 0)
            {
                // Normally means this coming midnight
                result.Start.Imply("hour", 22);
                result.Start.Imply("meridiem", 1);
            }
            else if (new Regex("^tomorrow|^tmr").Match(lowerText).Success)
            {
                // Check not "Tomorrow" on late night
                if (refMoment.Hour > 1)
                {
                    startMoment.Day += 1;
                }
            }
            else if (new Regex("^yesterday").Match(lowerText).Success)
            {
                startMoment.Day -= 1;
            }
            else if (new Regex(@"last\s*night").Match(lowerText).Success)
            {
                result.Start.Imply("hour", 0);
                if (refMoment.Hour > 6)
                {
                    startMoment.Day -= 1; ;
                }
            }
            else if (new Regex("now", RegexOptions.IgnoreCase).Match(lowerText).Success)
            {
                result.Start.Assign("hour", refMoment.Hour);
                result.Start.Assign("minute", refMoment.Minute);
                result.Start.Assign("second", refMoment.Second);
                result.Start.Assign("millisecond", refMoment.Millisecond);
            }

            result.Start.Assign("day", startMoment.Day);
            result.Start.Assign("month", startMoment.Month);
            result.Start.Assign("year", startMoment.Year);
            result.Tags["ENCasualDateParser"] = true;
            return result;
        }

    }
}
