using MomentSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    class ENCasualMonthParser : Parser
    {
        public ENCasualMonthParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern {
            get {
                return new Regex(@"(\W|^)(((latest|recent|current|present)|((this|current|present|last|latest|recent|previous|" +
                    Util.OrdinalWordsPattern + "|([0-9]{1,2})(?:st|nd|rd|th)?)?))+" +
                    @"\s*(months?|quarters?|years?|annual|trends?|evolutions?|movements?|progressions?|data\s*points?))",
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
            Moment endMoment = null;
            var lowerText = text.ToLower();
            if (new Regex(@"(\W|^)(latest|recent)|((last|latest|recent|previous)?\s*months?)").Match(lowerText).Success)
            {
                startMoment.Month -= 1;
                if (startMoment.Month == 0) //move to last year
                    startMoment.Year -= 1;
            }
            else if (new Regex(@"(\W|^)(latest|recent)|((last|latest|recent|previous)\s*quarters?)").Match(lowerText).Success)
            {
                SetDateRangeForQuarterOffset(-1, ref startMoment, ref endMoment);
            }
            else if (new Regex(@"(\W|^)(current|present)|((this|current|present)\s*quarters?)").Match(lowerText).Success)
            {
                SetDateRangeForQuarterOffset(-1, ref startMoment, ref endMoment);
            }
            else if (new Regex(@"(\W|^)((last|latest|recent|previous)\s*years?)").Match(lowerText).Success)
            {
                startMoment.Year -= 1;
                startMoment.Month = 1;
                endMoment = GetMoment(startMoment.DateTime());
                endMoment.Month = 12;
            }
            else if (new Regex(@"(\W|^)((this|current|present)\s*years?)").Match(lowerText).Success)
            {
                startMoment.Month = 1;
                endMoment = GetMoment(startMoment.DateTime());
                endMoment.Month = 12;
            }
            else if (new Regex(@"(\W|^)(annual|trends?|evolutions?|movements?|progressions?)").Match(lowerText).Success)
            {
                //last 12 months
                startMoment.Year -= 1;
                endMoment = GetMoment(startMoment.DateTime());
                endMoment.Month += 11;
                if (endMoment.Month > 12)
                {
                    //move to next year
                    endMoment.Year += 1;
                    endMoment.Month -= 12;
                }
            }
            else if (match.Groups[7].Captures.Count > 0) //things like 3rd,4th.. comes in 7th group
            {
                int qrtr;
                var qrtrStr = match.Groups[7].Value.Trim().Replace('_', ' ').ToLower();
                if (int.TryParse(qrtrStr, out qrtr))
                    SetDateRangeForQuarterIndex(qrtr, ref startMoment, ref endMoment);
            }
            else if(match.Groups[6].Captures.Count > 0) //ordinal number comes in 6th group
            {   
                var ordinalStr = match.Groups[6].Value.Trim().Replace('_', ' ').ToLower();
                ORDINAL_WORDS ordinalEnum;
                if (Enum.TryParse(ordinalStr, true, out ordinalEnum) && Enum.IsDefined(typeof(ORDINAL_WORDS), ordinalEnum))
                    SetDateRangeForQuarterIndex((int)ordinalEnum, ref startMoment, ref endMoment);
            }
            

            result.Start.Assign("day", startMoment.Day);
            result.Start.Assign("month", startMoment.Month);
            result.Start.Assign("year", startMoment.Year);
            if (endMoment != null)
            {
                result.End = result.End ?? result.Start.Clone() as ParsedComponents;
                result.End.Assign("day", endMoment.Day);
                result.End.Assign("month", endMoment.Month);
                result.End.Assign("year", endMoment.Year);
            }
            result.Tags[this.GetType().Name] = true;
            return result;
        }

        private void SetDateRangeForQuarterOffset(int n, ref Moment startMoment, ref Moment endMoment)
        {
            int quarterIndex = Util.GetQuarterByOffset(DateTime.Now, n);
            SetDateRangeForQuarterIndex(quarterIndex, ref startMoment, ref endMoment);
        }

        private void SetDateRangeForQuarterIndex(int quarterIndex, ref Moment startMoment, ref Moment endMoment)
        {
            int currentQuarterIndex = Util.GetQuarterByOffset(DateTime.Now, 0);
            if (quarterIndex > currentQuarterIndex && Config.Direction == DateTimeDirection.Backward) //future quarter
                startMoment.Year -= 1;
            startMoment.Month = (quarterIndex - 1) * 3 + 1;
            endMoment = GetMoment(startMoment.DateTime());
            endMoment.Month = startMoment.Month + 2;
        }
    }
}
