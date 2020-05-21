using MomentSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    abstract class Parser
    {
        public Parser(Config config)
        {
            this.Config = config ?? new Config();
        }

        internal Config Config { get; set; }
        protected bool IsStrictMode { get { return Config.Strict; } }
        protected virtual Regex Pattern { get { return new Regex("/./i"); } }
        protected virtual ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt) { return null; }
        internal List<ParsedResult> Execute(string text, DateTime? reference, Option opt)
        {
            var results = new List<ParsedResult>();
            var regex = this.Pattern;
            var remainingText = text;
            var match = regex.Match(remainingText);
            while (match.Success)
            {
                var result = Extract(text, reference, match, opt);
                if (result != null)
                {
                    // If success, start from the end of the result
                    remainingText = text.Substring(result.Index + result.Text.Length);
                    if (!this.IsStrictMode || result.HasPossibleDates)
                        results.Add(result);
                }
                else
                {
                    // If fail, move on by 1
                    remainingText = text.Substring(match.Index + 1);
                }

                //match = match.NextMatch();
                match = regex.Match(remainingText);
            }

            return results;
        }

        internal static Moment GetMoment(DateTime? dt)
        {
            if (!dt.HasValue)
                dt = DateTime.UtcNow;
            return new Moment
            {
                Day = dt.Value.Day,
                Hour = dt.Value.Hour,
                Millisecond = dt.Value.Millisecond,
                Minute = dt.Value.Minute,
                Month = dt.Value.Month,
                Second = dt.Value.Second,
                Year = dt.Value.Year
            };
        }

        protected void PredictAppropriateYear(DateTime? reference, int month, int day, ref ParsedResult result)
        {
            var refMoment = new DateTime(reference.Value.Year, month, day);
            var nextYear = new DateTime(refMoment.Year + 1, refMoment.Month, refMoment.Day);
            var lastYear = new DateTime(refMoment.Year - 1, refMoment.Month, refMoment.Day);
            var distanceToCurrentYearDate = (refMoment - reference.Value).TotalHours;
            var distanceToNextYearDate = (nextYear - reference.Value).TotalHours;
            var distanceToPreviousYearDate = (lastYear - reference.Value).TotalHours;

            if(Config.Direction == DateTimeDirection.Default)
            {
                //set the year based on nearest of the 3 dates from now 
                //- the one in previous year
                //- the one in current year
                //- the one in next year
                if (Math.Abs(distanceToNextYearDate) < Math.Abs(distanceToCurrentYearDate))
                    refMoment = nextYear;
                else if (Math.Abs(distanceToPreviousYearDate) < Math.Abs(distanceToCurrentYearDate))
                    refMoment = lastYear;
            }
            //if distance to current year is positive with always-backward config, set to last year, else set to current year
            else if (Config.Direction == DateTimeDirection.Backward && distanceToCurrentYearDate > 0)
                refMoment = lastYear;
            //if distance to current year is negative with always-forward config, set to next year, else set to current year
            else if (Config.Direction == DateTimeDirection.Forward && distanceToCurrentYearDate < 0)
                refMoment = nextYear;
            

            result.Start.Assign("day", day);
            result.Start.Assign("month", month);
            result.Start.Imply("year", refMoment.Year);
        }
    }
}
