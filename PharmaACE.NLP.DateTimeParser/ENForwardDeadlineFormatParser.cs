using System;
using System.Text.RegularExpressions;

namespace PharmaACE.NLP.DateTimeParser
{
    internal class ENForwardDeadlineFormatParser : ENDeadlineFormatParser
    {
        public ENForwardDeadlineFormatParser(Config config) : base(config)
        {
        }

        protected override Regex Pattern
        {
            get
            {
                //no future date possible for forward deadline, hence keeping next/till etc. is optional
                return IsStrictMode ?
                new Regex("(\\W|^)(next|till|until)?\\s*(" + Util.IntegerWordsPattern + "|[0-9]+|an?)\\s*(seconds?|minutes?|hours?|days?)\\s*" +
                "(?=\\W|$)", RegexOptions.IgnoreCase)
                :
                new Regex("(\\W|^)(next|till|until)?\\s*(" + Util.IntegerWordsPattern + "|[0-9]+|an?(?:\\s*few)?|half(?:\\s*an?)?)\\s*" + "(seconds?|min(?:ute)?s?|hours?|days?|weeks?|months?|years?)\\s*" + "(?=\\W|$)", RegexOptions.IgnoreCase);
                
            }
        }
        
    }
}