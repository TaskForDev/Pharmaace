namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SlideFavorite")]
    public partial class SlideFavorite
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int SlideID { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual ReportSlide ReportSlide { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
