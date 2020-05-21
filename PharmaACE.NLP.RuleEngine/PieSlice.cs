using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    /// <summary>
    /// represents individual slice in a pie chart
    /// </summary>
    public class PieSlice
    {
        public double Value { get; set; }
        public string Legend { get; set; }
    }
}
