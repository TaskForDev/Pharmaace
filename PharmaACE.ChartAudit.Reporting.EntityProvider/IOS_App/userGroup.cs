namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserGroup")]
    public partial class UserGroup
    {
        public int ID { get; set; }

        public int GroupID { get; set; }

        public int UserID { get; set; }

        public virtual Group Group { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
