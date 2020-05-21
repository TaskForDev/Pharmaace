using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
    public class CommentDetails
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string CommentAuthor { get; set; }
        public int? SlideId { get; set; }
        public bool IsPublic { get; set; }
        public string CommentDescription { get; set; }
        public int ParentId { get; set; }
        public string Image { get; set; }
       // public string ImageString { get; set; }
        public DateTime CreationDate { get; set; }
        public List<int> TaggedUsers { get; set; }
        public int CommentLikes { get; set; }   
        public int Replies { get; set; }
        public int? Views { get; set; }
        public bool IsViewed { get; set; }
        public bool IsLiked { get; set; }
        public List<int> CommentTagUser { get; set; }
        public int Subcategory { get; set; }
        public string Message { get; set; }
        public int? GlobalCommentViewCount { get; set; }
       
    }
    public class SubCommentInfo
    {
        public List<CommentDetails> SubCommentDetails { get; set; }
        public List<LoginDetail> UserList { get; set; }
        public AuthorInfo AuthorInfo { get; set; }
    }

    
    public class CommentsFilter
    {
        public int UserId { get; set; }
        public int SortBy { get; set; }
        public int ThreadBy { get; set; }
        public int PrivacyBy { get; set; }
        public List<int> Categories { get; set; }
    }

}