namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comment()
        {
            CommentLike = new HashSet<CommentLike>();
            CommentTagUser = new HashSet<CommentTagUser>();
            CommentView = new HashSet<CommentView>();
        }

        public int ID { get; set; }

        public int UserID { get; set; }

        public int? ReportSlideID { get; set; }

        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        
        public string Image { get; set; }

        public int ParentCommentID { get; set; }

        public bool IsPublic { get; set; }

        public DateTime UpdatedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommentLike> CommentLike { get; set; }

        public virtual UserDetail UserDetail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommentTagUser> CommentTagUser { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommentView> CommentView { get; set; }
    }
}
