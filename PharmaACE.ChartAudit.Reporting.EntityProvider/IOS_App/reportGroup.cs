namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReportGroup")]
    public partial class ReportGroup
    {
        public int ID { get; set; }

        public int ReportID { get; set; }

        public int GroupID { get; set; }

        public virtual Group Group { get; set; }

        public virtual Report Report { get; set; }
    }
}
