using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.Models
{
    public class UserProfile : BaseEntity<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAccountActivated { get; set; } = false;
        public DateTime? AccountActivationDate { get; set; }
        public string PasswordHash { get; set; }
        public string Address { get; set; }
        public string PicturePath { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}