using System.Collections.Generic;

namespace PharmaACE.NLP.DateTimeParser
{
    abstract class Refiner
    {
        public virtual List<ParsedResult> Refine(string originalText, List<ParsedResult> results, Option opt)
        {
            return results;
        }
    }

    class Filter
    {
        public virtual bool IsValid(string originalText, List<ParsedResult> results, Option opt) { return true; }

        public virtual List<ParsedResult> Refine(string originalText, List<ParsedResult> results, Option opt)
        {
            var filteredResult = new List<ParsedResult>();
            foreach (var result in results)
            {
                if (IsValid(originalText, results, opt))
                    filteredResult.Add(result);
            }

            return filteredResult;
        }
    }
}