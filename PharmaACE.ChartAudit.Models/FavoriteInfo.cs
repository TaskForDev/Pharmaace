using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
    public class FavoriteInfo
    {
        List<ReportSlideDetail> ReportslideInfo = new List<ReportSlideDetail>();
        List<ReportDetail> ReportDetails = new List<ReportDetail>();
    }

    public class UserReportMapping
    {
       public int ReportId { get; set; }
       public int UserId { get; set; }
    }

    public class UserReportSlideMapping
    {
        public int SlideId { get; set; }
        public int UserId { get; set; }
    }
    
}
