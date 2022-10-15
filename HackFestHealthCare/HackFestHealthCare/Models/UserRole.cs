using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Models
{
    public class UserRole : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string RoleId { get; set; }

        public UserProfile User { get; set; }
        public Role Role { get; set; }
    }
}
