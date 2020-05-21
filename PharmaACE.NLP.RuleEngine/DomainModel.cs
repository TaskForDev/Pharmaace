using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.Framework
{
    public sealed class DomainModel
    {
        static DomainModel domain;
        Dictionary<string, string> nomenclature;
        private DomainModel()
        {
            
        }
        public static DomainModel Load()
        {
            if (domain == null)
            {
                domain = new DomainModel();
                domain.nomenclature = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                domain.nomenclature.Add("share", "monthly share");
                domain.nomenclature.Add("sharer2m", "share");
            }

            return domain;
        }

        public string GetDisplayColumnName(string actualNameOfColumn)
        {
            if(nomenclature.ContainsKey(actualNameOfColumn))
                return nomenclature[actualNameOfColumn];
            return actualNameOfColumn;
        }
    }
}
