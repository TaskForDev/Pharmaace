using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{

    public class Categories
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreationDate { get; set; }

        public List<SubCategories> SubCategories{ get;set;}
    }

    public class SubCategories
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
