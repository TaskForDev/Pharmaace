namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ShMeasure")]
    public partial class ShMeasure
    {
        public int ID { get; set; }

        [Required]
        [StringLength(500)]
        public string Tumor { get; set; }

        [StringLength(50)]
        public string Line { get; set; }

        [Required]
        [StringLength(500)]
        public string Regimen { get; set; }

        [StringLength(500)]
        public string Segment { get; set; }

        [StringLength(500)]
        public string SubSegment { get; set; }

        [Column(TypeName = "date")]
        public DateTime Monthyear { get; set; }

        public decimal Share { get; set; }

        public decimal ShareR2M { get; set; }

        public decimal ShareR3M { get; set; }
    }
}
