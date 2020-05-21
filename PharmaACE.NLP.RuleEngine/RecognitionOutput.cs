using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    public class RecognitionOutput
    {
        /// <summary>
        /// recognized entities against equl/notequal/.. etc. clauses
        /// </summary>
        public Dictionary<NERClause, List<SentenceFragment>> SentenceFragments { get; set; }
        
        /// <summary>
        /// a flattened sentence in a common pattern that can be understood by Parser
        /// </summary>
        public string ParsableSentence { get; set; }
    }
}
