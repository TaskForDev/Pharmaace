namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("UserSearchDetails")]
    public partial class UserSearchDetails
    {
        public int ID { get; set; }

        public int? UserID { get; set; }

        public string SearchContent { get; set; }

        public DateTime? DateTime { get; set; }
    }
}
