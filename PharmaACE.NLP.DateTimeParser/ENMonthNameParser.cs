using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENMonthNameParser : Parser
    {
        const int MONTH_NAME_GROUP = 2;
        const int YEAR_GROUP = 3;
        const int YEAR_BE_GROUP = 4;

        public ENMonthNameParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern
        {
            get
            {
                return new Regex("(^|\\D\\s+|[^\\w\\s])?" +
    "(Jan\\.?|January|Feb\\.?|February|Mar\\.?|March|Apr\\.?|April|May\\.?|Jun\\.?|June|Jul\\.?|July|Aug\\.?|August|Sep\\.?|Sept\\.?|September|Oct\\.?|October|Nov\\.?|November|Dec\\.?|December)" +
    "\\s*" +
    "(?:" +
        "[,-]?\\s*([0-9]{4})(\\s*BE|AD|BC)?" +
    ")?" +
    "(?=[^\\s\\w]|\\s+[^0-9]|\\s+$|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt) {
            var index = match.Index + match.Groups[1].Length;
            var text = match.Groups[0].Value.Substring(match.Groups[1].Length, match.Groups[0].Length - match.Groups[1].Length);

            var result = new ParsedResult(new TemporalResult
            {
                Text = text,
                Index = index,
                Reference = reference
            });

            int month = -1;
            var monthStr = match.Groups[MONTH_NAME_GROUP].Value.ToLower();
            MONTH_OFFSET monthEnum;
            if (Enum.TryParse(monthStr, true, out monthEnum) && Enum.IsDefined(typeof(MONTH_OFFSET), monthEnum))
                month = (int)monthEnum;
                var day = 1;
            int year = -1;
            string yearStr = null;
            if (match.Groups[YEAR_GROUP].Captures.Count != 0)
            {
                yearStr = match.Groups[YEAR_GROUP].Value;
                int.TryParse(yearStr, out year);

                if (match.Groups[YEAR_BE_GROUP].Captures.Count != 0)
                {
                    if (new Regex("BE", RegexOptions.IgnoreCase).IsMatch(match.Groups[YEAR_BE_GROUP].Value))
                    {
                        // Buddhist Era
                        year = year - 543;
                    }
                    else if (new Regex("BC", RegexOptions.IgnoreCase).IsMatch(match.Groups[YEAR_BE_GROUP].Value))
                    {
                        // Before Christ
                        year = -year;
                    }

                }
                else if (year < 100)
                {
                    year = year + 2000;
                }
            }

            if (year != -1)
            {
                result.Start.Imply("day", day);
                result.Start.Assign("month", month);
                result.Start.Assign("year", year);
            }
            else
            {
                //Find the most appropriated year
                PredictAppropriateYear(reference, month, day, ref result);
            }

            result.Tags["ENMonthNameParser"] = true;
            return result;
        }
    }
}