namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReportSubCategory")]
    public partial class ReportSubCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReportSubCategory()
        {
            Report = new HashSet<Report>();
        }

        public int ID { get; set; }

        public int ReportCategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Report { get; set; }

        public virtual ReportCategory ReportCategory { get; set; }
    }
}
