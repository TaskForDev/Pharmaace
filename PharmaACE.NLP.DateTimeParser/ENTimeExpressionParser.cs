using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENTimeExpressionParser : Parser
    {
        Regex FIRST_REG_PATTERN = new Regex("(^|\\s|T)" +
    "(?:(?:at|from)\\s*)??" +
    "(\\d{1,4}|noon|midnight)" +
    "(?:" +
        "(?:\\.|\\:|\\：)(\\d{1,2})" +
        "(?:" +
            "(?:\\:|\\：)(\\d{2})(?:\\.(\\d{1,6}))?" +
        ")?" +
    ")?" +
    "(?:\\s*(A\\.M\\.|P\\.M\\.|AM?|PM?|O\\W*CLOCK))?" +
    "(?=\\W|$)", RegexOptions.IgnoreCase);


        Regex SECOND_REG_PATTERN = new Regex("^\\s*" +
            "(\\-|\\–|\\~|\\〜|to|\\?)\\s*" +
            "(\\d{1,4})" +
            "(?:" +
                "(?:\\.|\\:|\\：)(\\d{1,2})" +
                "(?:" +
                    "(?:\\.|\\:|\\：)(\\d{1,2})(?:\\.(\\d{1,6}))?" +
                ")?" +
            ")?" +
            "(?:\\s*(A\\.M\\.|P\\.M\\.|AM?|PM?|O\\W*CLOCK))?" +
            "(?=\\W|$)", RegexOptions.IgnoreCase);

        const int HOUR_GROUP = 2;
        const int MINUTE_GROUP = 3;
        const int SECOND_GROUP = 4;
        const int MILLI_SECOND_GROUP = 5;
        const int AM_PM_HOUR_GROUP = 6;

        public ENTimeExpressionParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern
        {
            get
            {
                return FIRST_REG_PATTERN;
            }
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt)
        {
            // This pattern can be overlaped Ex. [12] AM, 1[2] AM
            if (match.Index > 0 && new Regex(@"/\w/", RegexOptions.IgnoreCase).Match(originalText[match.Index - 1].ToString()).Success) return null;
            
            int index = match.Index + match.Groups[1].Length;
            string text = match.Groups[0].Value.Substring(match.Groups[1].Length);
            var result = new ParsedResult(new TemporalResult
            {
                Text = text,
                Index = index,
                Reference = reference
            });
            result.Tags["ENTimeExpressionParser"] = true;

            result.Start.Imply("day", reference.Value.Day);
            result.Start.Imply("month", reference.Value.Month);
            result.Start.Imply("year", reference.Value.Year);

            var hour = 0;
            var minute = 0;
            var meridiem = -1;

            // ----- Millisecond
            if (match.Groups[MILLI_SECOND_GROUP].Captures.Count != 0)
            {
                int millisecond = 0;
                int.TryParse(match.Groups[MILLI_SECOND_GROUP].Value.Substring(0, 3), out millisecond);
                if (millisecond >= 1000)
                    return null;

                result.Start.Assign("millisecond", millisecond);
            }

            // ----- Second
            if (match.Groups[SECOND_GROUP].Captures.Count != 0)
            {
                int second = 0;
                int.TryParse(match.Groups[SECOND_GROUP].Value, out second);
                if (second >= 60)
                    return null;

                result.Start.Assign("second", second);
            }

            // ----- Hours
            if (String.Compare(match.Groups[HOUR_GROUP].Value.ToLower(), "noon", true) == 0)
            {
                meridiem = 1;
                hour = 12;
            }
            else if (String.Compare(match.Groups[HOUR_GROUP].Value.ToLower(), "midnight", true) == 0)
            {
                meridiem = 0;
                hour = 0;
            }
            else
            {
                int.TryParse(match.Groups[HOUR_GROUP].Value, out hour);
            }

            // ----- Minutes
            if (match.Groups[MINUTE_GROUP].Captures.Count != 0)
            {
                int.TryParse(match.Groups[MINUTE_GROUP].Value, out minute);
            }
            else if (hour > 100)
            {
                minute = hour % 100;
                hour = (int)(hour / 100);
            }

            if (minute >= 60)
            {
                return null;
            }

            if (hour > 24)
            {
                return null;
            }
            if (hour >= 12)
            {
                meridiem = 1;
            }

            // ----- AM & PM  
            if (match.Groups[AM_PM_HOUR_GROUP].Captures.Count != 0)
            {
                if (hour > 12) return null;
                var ampm = match.Groups[AM_PM_HOUR_GROUP].Captures[0].Value.ToLower();
                if (String.Compare(ampm, "a", true) == 0)
                {
                    meridiem = 0;
                    if (hour == 12)
                        hour = 0;
                }

                if (String.Compare(ampm, "p", true) == 0)
                {
                    meridiem = 1;
                    if (hour != 12)
                        hour += 12;
                }
            }

            result.Start.Assign("hour", hour);
            result.Start.Assign("minute", minute);

            if (meridiem >= 0)
            {
                result.Start.Assign("meridiem", meridiem);
            }
            else
            {
                if (hour < 12)
                {
                    result.Start.Imply("meridiem", 0);
                }
                else
                {
                    result.Start.Imply("meridiem", 1);
                }
            }

            // ==============================================================
            //                  Extracting the "to" chunk
            // ==============================================================
            match = SECOND_REG_PATTERN.Match(originalText.Substring(result.Index + result.Text.Length));
            if (!match.Success)
            {
                // Not accept number only result
                if (new Regex(@"/^\d+$/", RegexOptions.IgnoreCase).Match(result.Text).Success)
                {
                    return null;
                }
                return result;
            }



            // Pattern "YY.YY -XXXX" is more like timezone offset
            if (new Regex((@"/^\s*(\+|\-)\s*\d{3,4}$/"), RegexOptions.IgnoreCase).Match(match.Groups[0].Value).Success) {
                return result;
            }

            if (result.End == null)
            {
                result.End = new ParsedComponents(null, result.Start.Date);
            }

            hour = 0;
            minute = 0;
            meridiem = -1;

            // ----- Millisecond
            if (match.Groups[MILLI_SECOND_GROUP].Captures.Count != 0)
            {
                int millisecond;
                int.TryParse(match.Groups[MILLI_SECOND_GROUP].Value.Substring(0, 3), out millisecond);
                if (millisecond >= 1000) return null;

                result.End.Assign("millisecond", millisecond);
            }

            // ----- Second
            if (match.Groups[SECOND_GROUP].Captures.Count != 0)
            {
                int second;
                int.TryParse(match.Groups[SECOND_GROUP].Value, out second);
                if (second >= 60) return null;

                result.End.Assign("second", second);
            }

            int.TryParse(match.Groups[2].Value, out hour);

            // ----- Minute
            if (match.Groups[MINUTE_GROUP].Captures.Count != 0)
            {

                int.TryParse(match.Groups[MINUTE_GROUP].Value, out minute);
                if (minute >= 60) return result;

            }
            else if (hour > 100)
            {

                minute = hour % 100;
                hour = (int)(hour / 100);
            }

            if (minute >= 60)
            {
                return null;
            }

            if (hour > 24)
            {
                return null;
            }
            if (hour >= 12)
            {
                meridiem = 1;
            }

            // ----- AM & PM 
            if (match.Groups[AM_PM_HOUR_GROUP].Captures.Count != 0)
            {

                if (hour > 12) return null;

                var ampm = match.Groups[AM_PM_HOUR_GROUP].Captures[0].Value.ToLower();
                if (String.Compare(ampm, "a", true) == 0)
                {
                    meridiem = 0;
                    if (hour == 12)
                    {
                        hour = 0;
                        if (!result.End.IsCertain("day"))
                        {
                            result.End.Imply("day", result.End.GetValue("day") + 1);
                        }
                    }
                }

                if ( ampm == "p")
                {
                    meridiem = 1;
                    if (hour != 12) hour += 12;
                }

                if (!result.Start.IsCertain("meridiem"))
                {
                    if (meridiem == 0)
                    {

                        result.Start.Imply("meridiem", 0);

                        if (result.Start.GetValue("hour") == 12)
                        {
                            result.Start.Assign("hour", 0);
                        }

                    }
                    else
                    {
                        result.Start.Imply("meridiem", 1);

                        if (result.Start.GetValue("hour") != 12)
                        {
                            result.Start.Assign("hour", result.Start.GetValue("hour") + 12);
                        }
                    }
                }
            }

            result.Text = result.Text + match.Groups[0].Value;
            result.End.Assign("hour", hour);
            result.End.Assign("minute", minute);
            if (meridiem >= 0)
            {
                result.End.Assign("meridiem", meridiem);
            }
            else
            {
                var startAtPM = result.Start.IsCertain("meridiem") && result.Start.GetValue("meridiem") == 1;
                if (startAtPM && result.Start.GetValue("hour") > hour)
                {
                    // 10pm - 1 (am)
                    result.End.Imply("meridiem", 0);
                }
                else if (hour > 12)
                {
                    result.End.Imply("meridiem", 1);
                }
            }

            if (result.End.Date < result.Start.Date)
            {
                result.End.Imply("day", result.End.GetValue("day") + 1);
            }

            return result;
        }
    }
}