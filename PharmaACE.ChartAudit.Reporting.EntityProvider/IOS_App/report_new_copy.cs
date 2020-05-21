namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Report_new_copy")]
    public partial class report_new_copy
    {
        [Key]
        public int report_id { get; set; }

        public int? report_sub_category_id { get; set; }

        [StringLength(500)]
        public string report_description { get; set; }

        [StringLength(200)]
        public string report_image { get; set; }

        public DateTime? report_added { get; set; }

        public int? user_author { get; set; }

        public byte? report_for_all { get; set; }
    }
}
