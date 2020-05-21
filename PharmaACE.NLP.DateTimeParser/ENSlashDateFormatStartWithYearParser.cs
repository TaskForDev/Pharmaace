using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENSlashDateFormatStartWithYearParser : Parser
    {
        public ENSlashDateFormatStartWithYearParser(Config config) : base(config)
        {
        }

        protected override ParsedResult Extract(string originalText, DateTime? reference, Match match, Option opt) { return null; }
    }
}