using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    class Options
    {
        internal static Option MergeOptions(List<Option> options)
        {
            var mergedOption = new Option { Parsers = new List<Parser>(), Refiners = new List<Refiner>() };
            foreach (var option in options)
            {
                if(option.Parsers != null)
                {
                    foreach (var parser in option.Parsers)
                    {
                        mergedOption.Parsers.Add(parser);
                    }
                }
                if (option.Refiners != null)
                {
                    foreach (var refiner in option.Refiners)
                    {
                        mergedOption.Refiners.Add(refiner);
                    }
                }
            }

            return mergedOption;
        }

        public static Option CasualOption(Config config = null)
        {
            return MergeOptions(new List<Option> { new ENCasual(config) });
        }
    }

    public class Option
    {
        internal List<Parser> Parsers { get; set; }
        internal List<Refiner> Refiners { get; set; }
    }

    class EN : Option
    {
        public EN(Config config)
        {
            Parsers = new List<Parser> { new ENISOFormatParser(config) };
            Refiners = new List<Refiner>();
        }
    }

    class ENCasual : EN
    {
        public ENCasual(Config config) : base(config)
        {
            config = config ?? new Config();
            config.Strict = false;

            Parsers.InsertRange(0, new List<Parser> {
                new ENCasualDateParser(config),
                new ENTimeAgoFormatParser(config),
                new ENTimeLaterFormatParser(config),
                new ENDeadlineFormatParser(config),
                new ENMonthNameLittleEndianParser(config),
                new ENMonthNameMiddleEndianParser(config),
                new ENMonthNameParser(config),
                new ENSlashDateFormatParser(config),
                new ENSlashDateFormatStartWithYearParser(config),
                new ENSlashMonthFormatParser(config),
                new ENCasualMonthParser(config),
                new ENNumericYearParser(config)//,
                //new ENTimeExpressionParser(config)
                });
            //add direction based parsers which could be either backward or forward (or default if nothing is specified by caller)
            var directionBasedParsers = new List<Parser>();
            if (config.Direction == DateTimeDirection.Backward)
            {
                directionBasedParsers.Add(new ENBackwardDeadlineFormatParser(config));
            }
            else if (config.Direction == DateTimeDirection.Forward)
            {
                directionBasedParsers.Add(new ENForwardDeadlineFormatParser(config));
            }
            else
            {
                var backwardConfig = config.Clone() as Config;
                backwardConfig.Direction = DateTimeDirection.Backward;
                directionBasedParsers.Add(new ENBackwardDeadlineFormatParser(backwardConfig));

                var forwardConfig = config.Clone() as Config;
                forwardConfig.Direction = DateTimeDirection.Forward;
                directionBasedParsers.Add(new ENForwardDeadlineFormatParser(forwardConfig));
            }
            Parsers.InsertRange(3, directionBasedParsers);

            Refiners.AddRange(new List<Refiner>
            {
                new ENMergeDateTimeRefiner()
            });
        }
    }
}
