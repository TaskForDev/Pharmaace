using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENMergeDateTimeRefiner : Refiner
    {
        public Regex Pattern {
            get
            {
                return new Regex(@"^\s*(to|\-)\s*$", RegexOptions.IgnoreCase);
            }
        }

        public override List<ParsedResult> Refine(string originalText, List<ParsedResult> results, Option opt)
        {
            if (results.Count < 2) return results;

            var mergedResult = new List<ParsedResult>();
            ParsedResult currResult = null;
            ParsedResult prevResult = null;

            for (var i = 1; i < results.Count; i++)
            {

                currResult = results[i];
                prevResult = results[i - 1];

                if (prevResult.End == null && currResult.End == null
                    && IsAbleToMerge(originalText, prevResult, currResult))
                {

                    prevResult = this.MergeResult(originalText, prevResult, currResult);
                    currResult = null;
                    i += 1;
                }

                mergedResult.Add(prevResult);
            }

            if (currResult != null)
            {
                mergedResult.Add(currResult);
            }


            return mergedResult;
        }

        private bool IsWeekdayResult(ParsedResult result)
        {
            return result.Start.IsCertain("weekday") && !result.Start.IsCertain("day");
        }

        private ParsedResult MergeResult(string text, ParsedResult fromResult, ParsedResult toResult)
        {
            if (!IsWeekdayResult(fromResult) && !IsWeekdayResult(toResult))
            {

            foreach (var kvp in toResult.Start.knownValues)
            {
                if (!fromResult.Start.IsCertain(kvp.Key))
                {
                    fromResult.Start.Assign(kvp.Key, toResult.Start.GetValue(kvp.Key));
                }
            }

            foreach (var kvp in fromResult.Start.knownValues)
            {
                if (!toResult.Start.IsCertain(kvp.Key))
                {
                    toResult.Start.Assign(kvp.Key, fromResult.Start.GetValue(kvp.Key));
                }
            }
        }

        if (fromResult.Start.Date > toResult.Start.Date) {
            
            var fromMoment = fromResult.Start.Date;
        var toMoment = toResult.Start.Date;

            if (IsWeekdayResult(fromResult) && fromMoment.AddDays(-7) < toMoment) {
                fromMoment = fromMoment.AddDays(-7);
                fromResult.Start.Imply("day", fromMoment.Day);
                fromResult.Start.Imply("month", fromMoment.Month);
                fromResult.Start.Imply("year", fromMoment.Year);
            } else if (this.IsWeekdayResult(toResult) && toMoment.AddDays(7) > fromMoment) {
        toMoment = toMoment.AddDays(7);
        toResult.Start.Imply("day", toMoment.Day);
        toResult.Start.Imply("month", toMoment.Month);
        toResult.Start.Imply("year", toMoment.Year);
    } else {
        var tmp = toResult;
        toResult = fromResult;
        fromResult = tmp;
    }
    }

    fromResult.End = toResult.Start;

        

        foreach (var tag in toResult.Tags) {
            fromResult.Tags[tag.Key] = true;
        }


var startIndex = Math.Min(fromResult.Index, toResult.Index);
var endIndex = Math.Max(
    fromResult.Index + fromResult.Text.Length,
    toResult.Index + toResult.Text.Length);

fromResult.Index = startIndex;
        fromResult.Text  = text.Substring(startIndex, (endIndex - startIndex));
        fromResult.Tags["ENMergeDateTimeRefiner"] = true;
        return fromResult;
        }

        private bool IsAbleToMerge(string text, ParsedResult result1, ParsedResult result2)
        {
            var begin = result1.Index + result1.Text.Length;
            var end = result2.Index;
            string textBetween = String.Empty;
            if(end > begin)
                textBetween = text.Substring(begin, (end - begin));
            else
                textBetween = text.Substring(end, (begin - end)); //swap

            return Pattern.Match(textBetween).Success;
        }
    }
}