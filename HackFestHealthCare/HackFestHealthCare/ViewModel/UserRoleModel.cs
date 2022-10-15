using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.ViewModel
{
    public class UserRoleModel
    {
        public Guid UserRoleId { get; set; }
        [Required(ErrorMessage = "User is required")]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string RoleId { get; set; }

        public UserModel User { get; set; }
        public RoleModel Role { get; set; }
    }
}
