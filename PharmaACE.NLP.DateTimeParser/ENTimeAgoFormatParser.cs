using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENTimeAgoFormatParser : Parser
    {
        public ENTimeAgoFormatParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern
        {
            get
            {
                return IsStrictMode ?
                new Regex("(\\W|^)" +
    "(?:within\\s*)?" +
    "(" + Util.TimeUnitStrictPattern + ")" +
    "ago(?=(?:\\W|$))", RegexOptions.IgnoreCase)
                :
                new Regex("(\\W|^)" +
    "(?:within\\s*)?" +
    "(" + Util.TimeUnitPattern + ")" +
    "(?:ago|before|earlier)(?=(?:\\W|$))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            }
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt)
        {
            if (match.Index > 0 && new Regex(@"/\w/").Match(originalText[match.Index - 1].ToString()).Success)
                return null;

            var text = match.Groups[0].Value;
            text = text.Substring(match.Groups[1].Length, match.Groups[0].Length - match.Groups[1].Length);
            int index = match.Index + match.Groups[1].Length;

            var result = new ParsedResult(new TemporalResult
            {
                Text = text,
                Index = index,
                Reference = reference
            });

            var fragments = Util.ExtractDateTimeUnitFragments(match.Groups[2].Value);
            var date = new DateTime(reference.Value.Year, reference.Value.Month, reference.Value.Day);

            foreach (var kvp in fragments)
            {
                switch (kvp.Key)
                {
                    case TEMPORAL_COMPONENT.Year:
                        date = date.AddYears(-kvp.Value);
                        break;
                    case TEMPORAL_COMPONENT.Month:
                        date = date.AddMonths(-kvp.Value);
                        break;
                    case TEMPORAL_COMPONENT.Day:
                        date = date.AddDays(-kvp.Value);
                        break;
                    case TEMPORAL_COMPONENT.Hour:
                        date = date.AddHours(-kvp.Value);
                        break;
                    case TEMPORAL_COMPONENT.Minute:
                        date = date.AddMinutes(-kvp.Value);
                        break;
                    case TEMPORAL_COMPONENT.Second:
                        date = date.AddSeconds(-kvp.Value);
                        break;
                    case TEMPORAL_COMPONENT.Week:
                        date = date.AddDays(-kvp.Value * 7);
                        break;
                    default:
                        break;
                }
            }

            if ((fragments.ContainsKey(TEMPORAL_COMPONENT.Hour) && fragments[TEMPORAL_COMPONENT.Hour] > 0) ||
                (fragments.ContainsKey(TEMPORAL_COMPONENT.Minute) && fragments[TEMPORAL_COMPONENT.Minute] > 0) || 
                (fragments.ContainsKey(TEMPORAL_COMPONENT.Second) && fragments[TEMPORAL_COMPONENT.Second] > 0))
            {
                result.Start.Assign("hour", date.Hour);
                result.Start.Assign("minute", date.Minute);
                result.Start.Assign("second", date.Second);
                result.Tags["ENTimeAgoFormatParser"] = true;
            }

            if ((fragments.ContainsKey(TEMPORAL_COMPONENT.Day) && fragments[TEMPORAL_COMPONENT.Day] > 0) ||
                (fragments.ContainsKey(TEMPORAL_COMPONENT.Month) && fragments[TEMPORAL_COMPONENT.Month] > 0) ||
                (fragments.ContainsKey(TEMPORAL_COMPONENT.Year) && fragments[TEMPORAL_COMPONENT.Year] > 0))
            {
                result.Start.Assign("day", date.Day);
                result.Start.Assign("month", date.Month);
                result.Start.Assign("year", date.Year);
            }
            else
            {
                if (fragments.ContainsKey(TEMPORAL_COMPONENT.Week) && fragments[TEMPORAL_COMPONENT.Week] > 0)
                {
                    result.Start.Imply("weekday", date.Day);
                }

                result.Start.Imply("day", date.Day);
                result.Start.Imply("month", date.Month);
                result.Start.Imply("year", date.Year);
            }

            return result;
        }
    }
}