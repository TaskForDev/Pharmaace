using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
  
    public enum SortBy
    {
        Recency = 0,
        MostViewed = 1,
        MostFavorite =2
    }

    public enum ViewBy
    {
        ShowAll = 0,
        Read = 1,
        UnRead = 2
    }

    public enum ThreadBy
    {
        ShowAll = 0,
        FromMe  = 1,
        ToMe = 2
    }

    public enum PrivacyBy
    {
        ShowAll = 0,
        Public = 1,
        Private = 2
    }
}
