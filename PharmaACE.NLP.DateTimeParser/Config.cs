using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    public class Config : ICloneable
    {
        public bool Strict { get; set; }
        public DateTimeDirection Direction { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
