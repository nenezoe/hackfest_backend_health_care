using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.ViewModel
{
    public class RoleModel
    {
        public string RoleId { get; set; }
        [Required(ErrorMessage = "Role name is required")]
        public string Name { get; set; }
        public List<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
    }
}
