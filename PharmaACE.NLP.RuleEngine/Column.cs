using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    public class Column
    {
        public string Name { get; set; }
        public string DisplayName { get { return DomainModel.Load().GetDisplayColumnName(Name); } }
        public List<string> Types { get; set; }
        public HashSet<string> MatchCandidates { get; set; }
        public List<string> Equivalences { get; set; }
        public bool Primary { get; set; }
        public Foreign Foreign { get; set; }

        bool IsEquivalent(string word)
        {
            return Equivalences.Contains(word);
        }

        public Column()
        {
            Equivalences = new List<string>();
        }
    }

    public class Foreign
    {
        public string ForeignTable { get; set; }
        public string ForeignColumn { get; set; }
    }
}
