namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CommentTagUser")]
    public partial class CommentTagUser
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        public int CommentID { get; set; }

        public virtual Comment Comment { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
