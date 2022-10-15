using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Models
{
    public class Role
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
