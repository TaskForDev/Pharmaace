using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    /// <summary>
    /// abstract factory for context and chart
    /// </summary>
    public abstract class DimensionFactory
    {
        protected ChartEngineBase chartEngine;
        protected RuleEngineBase ruleEngine;

        public RuleEngineBase RuleEngine { get { return ruleEngine; } }
        public ChartEngineBase ChartEngine { get { return chartEngine; } }

        public abstract bool Probe(string sentence);
    }
}
