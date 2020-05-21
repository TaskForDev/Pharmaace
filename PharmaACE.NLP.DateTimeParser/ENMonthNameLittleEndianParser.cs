using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENMonthNameLittleEndianParser : Parser
    {
        public ENMonthNameLittleEndianParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern
        {
            get
            {
                return new Regex("(\\W|^)" +
        "(?:on\\s*?)?" +
        "(?:(Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sun|Mon|Tue|Wed|Thu|Fri|Sat)\\s*,?\\s*)?" +
        "(([0-9]{1,2})(?:st|nd|rd|th)?|" + Util.OrdinalWordsPattern + ")" +
        "(?:\\s*" +
            "(?:to|\\-|\\–|until|through|till|\\s)\\s*" +
            "(([0-9]{1,2})(?:st|nd|rd|th)?|" + Util.OrdinalWordsPattern + ")" +
        ")?" +
        @"(?:-|\/|\\s*(?:of)?\\s*)" +
        "(Jan(?:uary|\\.)?|Feb(?:ruary|\\.)?|Mar(?:ch|\\.)?|Apr(?:il|\\.)?|May|Jun(?:e|\\.)?|Jul(?:y|\\.)?|Aug(?:ust|\\.)?|Sep(?:tember|\\.)?|Oct(?:ober|\\.)?|Nov(?:ember|\\.)?|Dec(?:ember|\\.)?)" +
        "(?:" +
            @"(?:-|\/|,?\\s*)" +
            "((?:" +
                "[1-9][0-9]{0,3}\\s*(?:BE|AD|BC)|" +
                "[1-2][0-9]{3}" +
            ")(?![^\\s]\\d))" +
        ")?" +
        "(?=\\W|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            }
        }

        const int WEEKDAY_GROUP = 2;
        const int DATE_GROUP = 3;
        const int DATE_NUM_GROUP = 4;
        const int DATE_TO_GROUP = 5;
        const int DATE_TO_NUM_GROUP = 6;
        const int MONTH_NAME_GROUP = 7;
        const int YEAR_GROUP = 8;

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt)
        {
            var text = match.Groups[0].Value.Substring(match.Groups[1].Length, match.Groups[0].Length - match.Groups[1].Length);
            var index = match.Index + match.Groups[1].Length;
            var result = new ParsedResult(new TemporalResult
            {
                Text = text,
                Index = index,
                Reference = reference
            });

            var monthStr = match.Groups[MONTH_NAME_GROUP].Value.ToLower();
            MONTH_OFFSET monthEnum;
            int month = -1;
            if (Enum.TryParse(monthStr, true, out monthEnum) && Enum.IsDefined(typeof(MONTH_OFFSET), monthEnum))
                month = (int)monthEnum;
            else
                int.TryParse(monthStr, out month);

            string dayStr = null;
            ORDINAL_WORDS dayEnum;
            int day = -1;
            if (match.Groups[DATE_NUM_GROUP].Captures.Count != 0)
            {
                dayStr = match.Groups[DATE_NUM_GROUP].Value.ToLower();

                if (Enum.TryParse(dayStr, true, out dayEnum) && Enum.IsDefined(typeof(MONTH_OFFSET), dayEnum))
                    day = (int)dayEnum;
                else
                    int.TryParse(dayStr, out day);
            }
            else
            {
                dayStr = match.Groups[DATE_GROUP].Value.Trim().Replace('_', ' ').ToLower();
                if (Enum.TryParse(dayStr, true, out dayEnum) && Enum.IsDefined(typeof(ORDINAL_WORDS), dayEnum))
                    day = (int)dayEnum;
            }

            string yearStr = null;
            int year = -1;
            if (match.Groups[YEAR_GROUP].Captures.Count != 0)
            {
                yearStr = match.Groups[YEAR_GROUP].Value;

                if (new Regex("BE", RegexOptions.IgnoreCase).Match(yearStr).Success)
                {
                    // Buddhist Era
                    yearStr = Regex.Replace(yearStr, "BE", String.Empty, RegexOptions.IgnoreCase);
                    int.TryParse(yearStr, out year);
                    year -= 543;
                }
                else if (new Regex("BC", RegexOptions.IgnoreCase).Match(yearStr).Success)
                {
                    // Before Christ
                    yearStr = Regex.Replace(yearStr, "BC", String.Empty, RegexOptions.IgnoreCase);
                    int.TryParse(yearStr, out year);
                    year *= -1;
                }
                else if (new Regex("AD", RegexOptions.IgnoreCase).Match(yearStr).Success)
                {
                    yearStr = Regex.Replace(yearStr, "AD", String.Empty, RegexOptions.IgnoreCase);
                    int.TryParse(yearStr, out year);
                }
                else
                {
                    int.TryParse(yearStr, out year);
                    if (year < 100)
                    {
                        year = year + 2000;
                    }
                }
            }

            if (year != -1)
            {
                result.Start.Assign("day", day);
                result.Start.Assign("month", month);
                result.Start.Assign("year", year);
            }
            else
            {

                //Find the most appropriated year
                PredictAppropriateYear(reference, month, day, ref result);
            }

            // Weekday component
            if (match.Groups[WEEKDAY_GROUP].Captures.Count != 0)
            {
                var weekdayStr = match.Groups[WEEKDAY_GROUP].Value.ToLower();
                WEEKDAY_OFFSET weekdayEnum;
                if (Enum.TryParse(weekdayStr, true, out weekdayEnum) && Enum.IsDefined(typeof(WEEKDAY_OFFSET), weekdayEnum))
                {
                    int weekday = (int)weekdayEnum;
                    result.Start.Assign("weekday", weekday);
                }
            }

            // Text can be 'range' value. Such as '12 - 13 January 2012'
            if (match.Groups[DATE_TO_GROUP].Captures.Count != 0)
            {
                string endDateStr = match.Groups[DATE_TO_NUM_GROUP].Value;
                int endDate = -1;
                ORDINAL_WORDS endDateEnum;
                if (match.Groups[DATE_TO_NUM_GROUP].Captures.Count != 0)
                    int.TryParse(endDateStr, out endDate);
                else if (Enum.TryParse(endDateStr.Trim().Replace('-', ' '), true, out endDateEnum) &&
                    Enum.IsDefined(typeof(WEEKDAY_OFFSET), endDateEnum))
                    endDate = (int)endDateEnum;

                result.End = (ParsedComponents)result.Start.Clone();
                result.End.Assign("day", endDate);
            }

            result.Tags["ENMonthNameLittleEndianParser"] = true;
            return result;
        }
    }
}