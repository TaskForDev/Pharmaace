namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DeviceTokenMapping")]
    public partial class DeviceTokenMapping
    {
       
        public int ID { get; set; }

        public int UserID { get; set; }

        [Required]
        public string UserDeviceToken { get; set; }

        public virtual UserDetail UserDetail { get; set; }
    }
}
