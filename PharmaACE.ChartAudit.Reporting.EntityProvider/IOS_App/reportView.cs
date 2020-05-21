namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReportView")]
    public partial class ReportView
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int ReportID { get; set; }

        public DateTime LastViewedDate { get; set; }

        public int ViewCount { get; set; }

        public virtual Report Report { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
