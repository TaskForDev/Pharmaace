namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommentView")]
    public partial class CommentView
    {
        public int ID { get; set; }

        public int CommentId { get; set; }

        public int UserId { get; set; }

        public int ViewCount { get; set; }

        public virtual Comment Comment { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
