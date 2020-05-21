namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AdminDetail")]
    public partial class AdminDetail
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public DateTime RegisteredDate { get; set; }

        [StringLength(20)]
        public string AdminTelephone { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Dob { get; set; }

        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(50)]
        public string Salt { get; set; }

        public byte Permission { get; set; }

    }
}
