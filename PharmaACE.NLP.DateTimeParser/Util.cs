using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    static class Util
    {
        public static string IntegerWordsPattern
        {
            get
            {
                return "(?:" + String.Join("|", Enum.GetNames(typeof(INTEGER_WORDS))) + ")";
            }
        }

        public static string OrdinalWordsPattern
        {
            get
            {
                return "(?:" + Regex.Replace(String.Join("|", Enum.GetNames(typeof(ORDINAL_WORDS))), " ", "[ -]") + ")";
            }
        }

        static string TimeUnit
        {
            get
            {
                return "(" + IntegerWordsPattern + "|[0-9]+|an?(?:\\s*few)?|half(?:\\s*an?)?)\\s*" +
                       "(sec(?:onds?)?|min(?:ute)?s?|hours?|weeks?|days?|months?|years?)\\s*";
            }
        }

        static string TimeUnitStrict
        {
            get
            {
                return "([0-9]+|an?)\\s*" +
                       "(seconds?|minutes?|hours?|days?)\\s*";
            }
        }

        static Regex PatternTimeUnit
        {
            get
            {
                return new Regex(TimeUnit, RegexOptions.IgnoreCase);
            }
        }

        internal static string TimeUnitPattern
        {
            get
            {
                return "(?:" + TimeUnit + ")+";
            }
        }

        internal static string TimeUnitStrictPattern {
            get
            {
                return "(?:" + TimeUnitStrict + ")+";
            }
        }

        public static Dictionary<TEMPORAL_COMPONENT, int> ExtractDateTimeUnitFragments(string timeunitText)
        {   
            var matches = PatternTimeUnit.Matches(timeunitText);
            return CollectDateTimeFragment(matches.Cast<Match>().SelectMany(m => m.Groups.Cast<Group>().Select(g => g.Value)).ToArray());
        }

        static Dictionary<TEMPORAL_COMPONENT, int> CollectDateTimeFragment(string[] match)
        {
            var fragments = new Dictionary<TEMPORAL_COMPONENT, int>();
            var numStr = match[1].ToLower();
            int num = -1;
            INTEGER_WORDS enumIntWord;
            if (Enum.TryParse(numStr, out enumIntWord))
            {
                num = (int)enumIntWord;
            }
            else if (String.Compare(numStr, "a", true) == 0 || String.Compare(numStr, "an", true) == 0)
            {
                num = 1;
            }
            else if (numStr.Contains("few"))
            {
                num = 3;
            }
            else if (numStr.Contains("half"))
            {
                //num = 0.5; //TODO : take care of half later
            }
            else
            {
                int.TryParse(numStr, out num);
            }

            if (Regex.Match(match[2], "hour", RegexOptions.IgnoreCase).Success)
            {
                fragments[TEMPORAL_COMPONENT.Hour] = num;
            }
            else if (Regex.Match(match[2], "min", RegexOptions.IgnoreCase).Success)
            {
                fragments[TEMPORAL_COMPONENT.Minute] = num;
            }
            else if (Regex.Match(match[2], "sec", RegexOptions.IgnoreCase).Success)
            {
                fragments[TEMPORAL_COMPONENT.Second] = num;
            }
            else if (Regex.Match(match[2], "week", RegexOptions.IgnoreCase).Success)
            {
                fragments[TEMPORAL_COMPONENT.Week] = num;
            }
            if (Regex.Match(match[2], "day", RegexOptions.IgnoreCase).Success)
            {
                fragments[TEMPORAL_COMPONENT.Day] = num;
            }
            if (Regex.Match(match[2], "month", RegexOptions.IgnoreCase).Success)
            {
                fragments[TEMPORAL_COMPONENT.Month] = num;
            }
            if (Regex.Match(match[2], "year", RegexOptions.IgnoreCase).Success)
            {
                fragments[TEMPORAL_COMPONENT.Year] = num;
            }

            return fragments;
        }

        /// <summary>
        /// returns quarter index from date, e.g. March 6 with offset -1 should return 4
        /// </summary>
        /// <param name="date"></param>
        /// <param name="offset">offset from current quarter, default is 0, i.e. current quarter
        ///                      negative for previous, positive for next</param>
        /// <returns></returns>
        internal static int GetQuarterByOffset(DateTime date, int offset = 0)
        {
            return (int)Math.Ceiling((double)date.AddMonths(3 * offset).Month / (double)3);
        }
    }


    enum TEMPORAL_COMPONENT
    {
        Evolution,
        Trend,
        Movement,
        Progression,
        Year,
        Quarter,
        Month,
        Week,
        Day,
        Hour,
        Minute,
        Second
    }

    enum WEEKDAY_OFFSET
    {
        sunday = 0,
        sun = 0,
        monday = 1,
        mon = 1,
        tuesday = 2,
        tue = 2,
        wednesday = 3,
        wed = 3,
        thursday = 4,
        thur = 4,
        thu = 4,
        friday = 5,
        fri = 5,
        saturday = 6,
        sat = 6
    }


    enum MONTH_OFFSET
    {
        january = 1,
        jan = 1,
        //jan. = 1,
        february = 2,
        feb = 2,
        //feb. = 2,
        march = 3,
        mar = 3,
        //mar. = 3,
        april = 4,
        apr = 4,
        //apr. = 4,
        may = 5,
        june = 6,
        jun = 6,
        //jun. = 6,
        july = 7,
        jul = 7,
        //jul. = 7,
        august = 8,
        aug = 8,
        //aug. = 8,
        september = 9,
        sep = 9,
        //sep. = 9,
        sept = 9,
        //sept. = 9,
        october = 10,
        oct = 10,
        //oct. = 10,
        november = 11,
        nov = 11,
        //nov. = 11,
        december = 12,
        dec = 12,
        //dec. = 12
    }

    enum INTEGER_WORDS
    {
        This = 0,
        one = 1,
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6,
        seven = 7,
        eight = 8,
        nine = 9,
        ten = 10,
        eleven = 11,
        twelve = 12
    }

    enum ORDINAL_WORDS
    {
        first = 1,
        second = 2,
        third = 3,
        fourth = 4,
        fifth = 5,
        sixth = 6,
        seventh = 7,
        eighth = 8,
        ninth = 9,
        tenth = 10,
        eleventh = 11,
        twelfth = 12,
        thirteenth = 13,
        fourteenth = 14,
        fifteenth = 15,
        sixteenth = 16,
        seventeenth = 17,
        eighteenth = 18,
        nineteenth = 19,
        twentieth = 20,
        twenty_first = 21,
        twenty_second = 22,
        twenty_third = 23,
        twenty_fourth = 24,
        twenty_fifth = 25,
        twenty_sixth = 26,
        twenty_seventh = 27,
        twenty_eighth = 28,
        twenty_ninth = 29,
        thirtieth = 30,
        thirty_first = 31
    }
}
