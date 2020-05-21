using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    public abstract class ChartEngineBase
    {
        public abstract ChartBase GetChart(Visualization viz);
    }
}
