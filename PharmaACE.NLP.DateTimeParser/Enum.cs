using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.NLP.DateTimeParser
{
    /// <summary>
    /// in some contexts, there is no point looking at future dates (e.g. reporting on actual data)
    /// /// in some contexts, there is no point looking at past dates (e.g. reporting on forecast data)
    /// </summary>
    public enum DateTimeDirection
    {
        Default,
        Backward,
        Forward
    }
}
