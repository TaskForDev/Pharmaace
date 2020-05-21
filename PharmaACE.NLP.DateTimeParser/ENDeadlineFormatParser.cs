using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENDeadlineFormatParser : Parser
    {
        public ENDeadlineFormatParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern
        {
            get
            {
                return IsStrictMode ?
                new Regex("(\\W|^)(within|in|for|during)\\s*(" + Util.IntegerWordsPattern + "|[0-9]+|an?)\\s*(seconds?|minutes?|hours?|days?)\\s*" +
                "(?=\\W|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                :
                new Regex("(\\W|^)(within|in|for|during)\\s*(" + Util.IntegerWordsPattern + "|[0-9]+|an?(?:\\s*few)?|half(?:\\s*an?)?)\\s*" + "(seconds?|min(?:ute)?s?|hours?|days?|weeks?|months?|years?)\\s*" + "(?=\\W|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                
            }
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt)
        {
            var index = match.Index + match.Groups[1].Length;
            var text = match.Groups[0].Value.Substring(match.Groups[1].Length, match.Groups[0].Length - match.Groups[1].Length);

            var result = new ParsedResult(new TemporalResult
            {
                Text = text,
                Index = index,
                Reference = reference
            });

            var numStr = match.Groups[3].Value.ToLower();
            INTEGER_WORDS numEnum;
            int num = -1;
            if (Enum.TryParse(numStr, true, out numEnum) && Enum.IsDefined(typeof(INTEGER_WORDS), numEnum))
            {
                num = (int)numEnum;
            }
            else if (String.Compare(numStr, "a", true) == 0 || String.Compare(numStr, "an", true) == 0)
            {
                num = 1;
            }
            else if (new Regex("few", RegexOptions.IgnoreCase).Match(numStr).Success)
            {
                num = 3;
            }
            else if (new Regex("half", RegexOptions.IgnoreCase).Match(numStr).Success)
            { //let's take care of half if the requirement comes!
              //num = 0.5;
            }
            else
            {
                int.TryParse(numStr, out num); //failed int.parse => num = 0
            }

            DateTime date = new DateTime(reference.Value.Year, reference.Value.Month, reference.Value.Day);
            if (new Regex("day|week|month|year", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
            {
                if (new Regex("day", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
                {
                    if (Config.Direction == DateTimeDirection.Backward)
                        date = date.AddDays(-num);
                    else
                        date = date.AddDays(num);
                }
                else if (new Regex("week", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
                {
                    if (Config.Direction == DateTimeDirection.Backward)
                        date = date.AddDays(-num * 7);
                    else
                        date = date.AddDays(num * 7);
                }
                else if (new Regex("month", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
                {
                    if (Config.Direction == DateTimeDirection.Backward)
                        date = date.AddMonths(-num);
                    else
                        date = date.AddMonths(num);
                }
                else if (new Regex("year", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
                {
                    if (Config.Direction == DateTimeDirection.Backward)
                        date = date.AddYears(-num);
                    else
                        date = date.AddYears(num);
                }
            }
            else if (new Regex("hour", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
            {
                if (Config.Direction == DateTimeDirection.Backward)
                    date = date.AddHours(-num);
                else
                    date = date.AddHours(num);
            }
            else if (new Regex("min", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
            {
                if (Config.Direction == DateTimeDirection.Backward)
                    date = date.AddMinutes(-num);
                else
                    date = date.AddMinutes(num);
            }
            else if (new Regex("second", RegexOptions.IgnoreCase).Match(match.Groups[4].Value).Success)
            {
                if (Config.Direction == DateTimeDirection.Backward)
                    date = date.AddSeconds(-num);
                else
                    date = date.AddSeconds(num);
            }

            SetStartEndDates(result, date, reference ?? DateTime.Now);
            return result;
        }

        private void SetStartEndDates(ParsedResult result, DateTime date, DateTime reference)
        {
            ParsedComponents pivot = null;
            ParsedComponents node = null;
            result.End = result.End ?? result.Start.Clone() as ParsedComponents;

            if (Config.Direction == DateTimeDirection.Backward)
            {
                pivot = result.End;
                node = result.Start;
            }
            else
            {
                pivot = result.Start;
                node = result.End;
            }
            pivot.Imply("year", reference.Year);
            pivot.Imply("month", reference.Month);
            pivot.Imply("day", reference.Day);
            pivot.Imply("hour", reference.Hour);
            pivot.Imply("minute", reference.Minute);
            pivot.Imply("second", reference.Second);

            node.Imply("year", date.Year);
            node.Imply("month", date.Month);
            node.Imply("day", date.Day);
            node.Imply("hour", date.Hour);
            node.Imply("minute", date.Minute);
            node.Imply("second", date.Second);

            result.Tags[this.GetType().Name] = true;
        }
    }
}