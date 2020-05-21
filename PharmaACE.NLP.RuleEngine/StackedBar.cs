using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    /// <summary>
    /// represents a stacked bar
    /// </summary>
    public class StackedBar
    {
        public string Abscissa { get; set; }
        public List<Ordinate> Stacks { get; set; }
    }
}
