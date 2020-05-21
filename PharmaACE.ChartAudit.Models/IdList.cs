using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
    public class GroupDetailsModel
    {

        public int groupId { get; set; }
        public List<int> UserIds { get; set; }
        public string groupName { get; set; }
        public List<UserDetails> usersDetails { get; set; }


    }
 public  class UserDetails
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public bool IsMember { get; set; }
    }
}
