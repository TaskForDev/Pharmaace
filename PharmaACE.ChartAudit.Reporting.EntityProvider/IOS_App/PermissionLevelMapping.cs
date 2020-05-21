namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PermissionLevelMapping")]
    public partial class PermissionLevelMapping
    {
        
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PermissionId { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual UserDetail UserDetail { get; set; }

        public virtual PermissionLevel PermissionLevel { get; set; }

    }
}
