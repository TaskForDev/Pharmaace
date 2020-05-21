namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Comments_users")]
    public partial class comments_users
    {
        [Key]
        public int comments_users_id { get; set; }

        public int? user_details_id { get; set; }

        public int? groups_id { get; set; }

        public int? comments_id { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
