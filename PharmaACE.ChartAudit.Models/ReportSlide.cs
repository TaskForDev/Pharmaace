using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{

    public class ReportInfo
    {
        public int ReportId { get; set; }
        public List<ReportSlideDetail> Slides {get;set;}
        public List<LoginDetail> Users { get; set; }
        public AuthorInfo Author { get; set; }
        

    }
    public class ReportSlideDetail

    { 
        //public int Id { get; set; }
        public int SlideId { get; set; }
        public int UserId { get; set; }
        public byte[] TempImage { get; set; }
        public string Image { get; set; }
        public int ReportId { get; set; }
        public bool IsLiked { get; set; }
        public AuthorInfo Author { get; set; }   //use Userpeer
        public List<LoginDetail> Users { get; set; }
        public bool IsPublic { get; set; }

    }

  
    public class AuthorInfo
    {
        public int? AuthorId { get; set; }
        public string AuthorName { get; set;}
    }
    //public class ReportAdded
    //{
    //    public int TimezoneType { get; set; }
    //    public DateTime CreationDate { get; set; }
    //    public string TimeZone { get; set; }
    //}

    public class MailWithSlide
    {
        public int UserId { get; set; }   //Sender
        public int SlideId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<int> ReceiverIds { get; set; }
        public string Image { get; set;}
    }
}