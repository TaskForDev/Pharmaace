using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
   public class GroupListModel
    {
        public int ID { get; set; }

        public string GroupName { get; set; }

        public int GroupMemberCount { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
