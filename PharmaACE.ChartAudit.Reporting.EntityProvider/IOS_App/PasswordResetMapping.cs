namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PasswordResetMapping")]
    public partial class PasswordResetMapping
    {
        public int ID { get; set; }

        public int UserId { get; set; }

        public DateTime PasswordResetOn { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
