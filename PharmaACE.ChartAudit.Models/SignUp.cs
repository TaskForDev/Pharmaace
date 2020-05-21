using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
    public class SignInDetail
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordResetOn { get; set; }
    }

     
    public class ResetDetails
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class UserInfoModel: SignInDetail
    {
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Company { get; set; }

        public string Contact { get; set; }

        public DateTime RegisteredDate { get; set; }

        public string PasswordToMail { get; set; }

        public int Permission { get; set; }

        public DateTime? DOB { get; set; }
        
       
    }

}
