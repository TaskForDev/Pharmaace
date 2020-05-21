using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PharmaACE.ChartAudit.Models
{
    public class ReportDetail
    {
        public int ReportId { get; set; }

        public string SubCategoryName { get; set; }

        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; } 
        public byte[] tempImage { get; set; }
        public bool IsPublic { get; set; }
        public bool IsViewed { get; set; }           
        public List<int> GroupIds { get; set; }
        public int AuthorId { get; set; }
        public string Author { get; set; }
        public int? ReportViewCount { get; set; }      
        public bool IsLiked { get; set; }
        public List<ReportSlideDetail> ReportSlideList { get; set; }
        public List<UserGroupMapping> UserGroupMapping { get; set; }
        public DateTime CreationDate { get; set; }
        public List<GroupInfo> GroupInfos { get; set; }
        public int? GlobalReportViewCount { get; set; }
        public int? ReportLikeCount { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class ReportInformation
    {
        public ReportDetail ReportDetail { get; set; }

        public MemoryStream FileStream { get; set; }
    }

  

    public class UserGroupMapping
    {
        public int GroupId;
        public int UserId;
    }
    public class ReportAndSlideFavorite
    {
        public List<ReportDetail> Reports { get; set; }
        public List<ReportSlideDetail> Slides{ get; set; }
    }

    public class GroupInfo
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }
    }
    
    public class ReportFilter
    {
        public int UserId { get; set; }
        public int SortBy { get; set; }
        public int ViewBy { get; set; }
        public List<int> Categories { get; set; }

    }

    public class CategoryReportDetails
    {
        public int SubcategoryId { get; set; }
        public List<ReportDetail> ReportDetails { get; set; }

    }

}
