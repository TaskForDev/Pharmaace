namespace PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("WordScore")]
    public partial class WordScore
    {
        public int ID { get; set; }

        [StringLength(400)]
        public string KeyPhrase { get; set; }

        public decimal? Score { get; set; }
    }
}
