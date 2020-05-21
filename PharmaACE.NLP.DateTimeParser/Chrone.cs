using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    public class Chrone
    {
        Option Option { get; set; }
        Config Config { get; set; }
        List<Parser> Parsers { get; set; }
        List<Refiner> Refiners { get; set; }

        public Chrone(Option option = null, Config config = null)
        {
            Config = config;
            option = option ?? Options.CasualOption(config);
            Parsers = option.Parsers;
            Refiners = option.Refiners;
        }

        public List<ParsedResult> Parse(string text, DateTime? refDate = null, Option opt = null)
        {
            var allResults = new List<ParsedResult>();
            refDate = refDate ?? DateTime.Now;
            refDate = new DateTime(refDate.Value.Year, refDate.Value.Month, 1); //shift to 1st date of month as 'day' is irrelevant for a 'month year' parser! 
                                                                                //Also using a day here will give problems when 31st Oct is deducted by 1 month: it will lead to 31st sept which is non-existent!

            foreach (var parser in Parsers)
            {
                //override parser's config with caller's config
                if (Config != null)
                    parser.Config = Config;
                allResults.AddRange(parser.Execute(text, refDate, opt));
            }
            allResults.OrderBy(res => res.Index);
            foreach (var refiner in Refiners)
            {
                allResults = refiner.Refine(text, allResults, opt);
            }

            return allResults;
        }
    }
}
