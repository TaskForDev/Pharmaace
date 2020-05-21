namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("ReportSlide")]
    public partial class ReportSlide
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReportSlide()
        {
            SlideFavorite = new HashSet<SlideFavorite>();
        }
        
        public int ID { get; set; }

        public int ReportID { get; set; }

        public byte[] Slide { get; set; }

        public int Order { get; set; }
        
        public virtual Report Report { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SlideFavorite> SlideFavorite { get; set; }
    }
}
