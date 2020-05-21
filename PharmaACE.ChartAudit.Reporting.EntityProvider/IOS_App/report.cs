namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Report")]
    public partial class Report
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Report()
        {
            ReportFavorite = new HashSet<ReportFavorite>();
            ReportSlide = new HashSet<ReportSlide>();
            ReportView = new HashSet<ReportView>();
            ReportGroup = new HashSet<ReportGroup>();
        }

        public int ID { get; set; }

        public int SubcategoryID { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public int Author { get; set; }

        public bool IsPublic { get; set; }

        public virtual ReportSubCategory ReportSubCategory { get; set; }

        public virtual UserDetail UserDetail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportFavorite> ReportFavorite { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportSlide> ReportSlide { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportView> ReportView { get; set; }

        public virtual UserDetail UserDetail1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportGroup> ReportGroup { get; set; }
    }
}
