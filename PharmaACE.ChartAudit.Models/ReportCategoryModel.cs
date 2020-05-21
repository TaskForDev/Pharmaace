using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
   public class ReportCategoryModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int SubCategoryCount { get; set; }

        public DateTime CreationDate { get; set; }

        public List<ReportSubCategoryModel> ReportSubCategoryModel { get; set; }
    }

    public class ReportSubCategoryModel
    {
        public int SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
