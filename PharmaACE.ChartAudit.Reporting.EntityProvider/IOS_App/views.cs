namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Views")]
    public partial class Views
    {
        [Key]
        public int view_id { get; set; }

        public int? comments_id { get; set; }

        public int? user_details_id { get; set; }

        public int? report_slides_id { get; set; }

        public TimeSpan? time_spent { get; set; }

        public int? no_of_views { get; set; }

        public virtual UserDetail userDetail { get; set; }
    }
}
