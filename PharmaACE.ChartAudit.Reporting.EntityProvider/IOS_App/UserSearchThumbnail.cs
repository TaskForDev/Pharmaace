namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserSearchThumbnail")]
    public partial class UserSearchThumbnail
    {
        public int ID { get; set; }

        public int UserID { get; set; }

        [StringLength(1000)]
        public string Question { get; set; }

        public string Narrative { get; set; }

        public byte[] Thumbnail { get; set; }
    }
}
