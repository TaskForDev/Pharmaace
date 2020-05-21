using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
   public  class LoginDetail
    {
      public int Result { get; set; }
      public int UserId { get; set; }
      public string UserName { get; set; }
      public int Permission { get; set; }
      public DateTime PasswordResetOn { get; set; }
      public bool setByAdmin { get; set; }
      public string StatusMessage { get; set; }
    }

   
    
}
