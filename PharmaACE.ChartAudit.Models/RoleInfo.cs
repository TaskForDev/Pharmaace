using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaACE.ChartAudit.Models
{
    public class RoleInfo
    {
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public string RoleName { get; set; }
    }
}
