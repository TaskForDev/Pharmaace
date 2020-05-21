using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmaACE.NLP.Framework;

namespace PharmaACE.NLP.ChartAudit.NLIDB
{
    public class Prober
    {
        //append at the bottom of the following dictionary for upcoming measures
        //order in the list is important to decide preference in case of muliple dimensionfactories found matching
        static List<DimensionFactory> Detectors = new List<DimensionFactory> {
            new CATRDimensionFactory(),
            new CAIRDimensionFactory(),
            new CASharesDimensionFactory()
        };

        public static DimensionFactory Probe(string sentence)
        {
            foreach (var detector in Detectors)
            {
                if (detector.Probe(sentence))
                    return detector;
                else if (detector.RuleEngine != null) //in case of default dimension factory when probe returns false but rule engine is ppulated
                    return detector;
                //TODO : what if multiple probes are successfull?
            }
            
            return null;
        }
    }

    public enum MeasureEnum
    {
        Share,
        TestingRate,
        Rate
    }
}
