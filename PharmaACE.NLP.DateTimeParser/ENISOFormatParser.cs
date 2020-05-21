using MomentSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    /// <summary>
    ///ISO 8601
    ///http://www.w3.org/TR/NOTE-datetime
    /// YYYY-MM-DD
    /// YYYY-MM-DDThh:mmTZD
    /// YYYY-MM-DDThh:mm:ssTZD
    /// YYYY-MM-DDThh:mm:ss.sTZD 
    /// TZD = (Z or +hh:mm or -hh:mm)
    /// </summary>
    class ENISOFormatParser : Parser
    {
        protected override Regex Pattern
        {
            get
            {
                return new Regex("(\\W|^)"
+ "([0-9]{4})\\-([0-9]{1,2})\\-([0-9]{1,2})"
+ "(?:T" //..
+ "([0-9]{1,2}):([0-9]{1,2})" // hh:mm
+ "(?::([0-9]{1,2})(?:\\.(\\d{1,4}))?)?" // :ss.s
+ "(?:Z|([+-]\\d{2}):?(\\d{2})?)?" // TZD (Z or ±hh:mm or ±hhmm or ±hh)
+ ")?"  //..
+ "(?=\\W|$)", RegexOptions.IgnoreCase);
            }
        }


        const int YEAR_NUMBER_GROUP = 2;
        const int MONTH_NUMBER_GROUP = 3;
        const int DATE_NUMBER_GROUP = 4;
        const int HOUR_NUMBER_GROUP = 5;
        const int MINUTE_NUMBER_GROUP = 6;
        const int SECOND_NUMBER_GROUP = 7;
        const int MILLISECOND_NUMBER_GROUP = 8;
        const int TZD_HOUR_OFFSET_GROUP = 9;
        const int TZD_MINUTE_OFFSET_GROUP = 10;

        public ENISOFormatParser(Config config) : base(config)
        {
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt)
        {
            var text = match.Groups[0].Value.Substring(match.Groups[1].Length);
            var index = match.Index + match.Groups[1].Length;

            var result = new ParsedResult(new TemporalResult
            {
                Text = text,
                Index = index,
                Reference = reference
            });

            result.Start.Assign("year", int.Parse(match.Groups[YEAR_NUMBER_GROUP].Value));
            result.Start.Assign("month", int.Parse(match.Groups[MONTH_NUMBER_GROUP].Value));
            result.Start.Assign("day", int.Parse(match.Groups[DATE_NUMBER_GROUP].Value));

            //validation
            if (result.Start.GetValue("month") > 12 || result.Start.GetValue("month") < 1 ||
                result.Start.GetValue("day") > 31 || result.Start.GetValue("day") < 1)
            {
                return null;
            }

            if (!String.IsNullOrWhiteSpace(match.Groups[HOUR_NUMBER_GROUP].Value))
            {

                result.Start.Assign("hour",
                        int.Parse(match.Groups[HOUR_NUMBER_GROUP].Value));
                result.Start.Assign("minute",
                        int.Parse(match.Groups[MINUTE_NUMBER_GROUP].Value));

                if (!String.IsNullOrWhiteSpace(match.Groups[SECOND_NUMBER_GROUP].Value))
                {

                    result.Start.Assign("second",
                            int.Parse(match.Groups[SECOND_NUMBER_GROUP].Value));
                }

                if (!String.IsNullOrWhiteSpace(match.Groups[MILLISECOND_NUMBER_GROUP].Value))
                {

                    result.Start.Assign("millisecond",
                            int.Parse(match.Groups[MILLISECOND_NUMBER_GROUP].Value));
                }

                if (!String.IsNullOrWhiteSpace(match.Groups[TZD_HOUR_OFFSET_GROUP].Value))
                {

                    result.Start.Assign("timezoneOffset", 0);
                }
                else
                {
                    var minuteOffset = 0;
                    var hourOffset = int.Parse(match.Groups[TZD_HOUR_OFFSET_GROUP].Value);
                    if (!String.IsNullOrWhiteSpace(match.Groups[TZD_MINUTE_OFFSET_GROUP].Value))
                        minuteOffset = int.Parse(match.Groups[TZD_MINUTE_OFFSET_GROUP].Value);

                    var offset = hourOffset * 60;
                    if (offset < 0)
                    {
                        offset -= minuteOffset;
                    }
                    else
                    {
                        offset += minuteOffset;
                    }

                    result.Start.Assign("timezoneOffset", offset);
                }
            }

            result.Tags["ENISOFormatParser"] = true;
            return result;
        }
    }
    
}
