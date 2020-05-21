namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhraseTable")]
    public partial class PhraseTable
    {
        public int ID { get; set; }

        [Required]
        [StringLength(4000)]
        public string KeyPhrase { get; set; }
    }
}
