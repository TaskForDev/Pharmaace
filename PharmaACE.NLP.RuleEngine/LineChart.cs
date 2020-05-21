using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    /// <summary>
    /// represents a single line (time series) of data
    /// </summary>
    public class LineChart
    {
        public List<DataPoint> DataPoints { get; set; }
        public string Legend { get; set; }
    }
}
