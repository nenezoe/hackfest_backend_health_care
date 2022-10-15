using System;
using System.ComponentModel.DataAnnotations;

namespace HackFestHealthCare.ViewModel
{
    public class UserModel
    {
        public Guid UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAccountActivated { get; set; } = false;
        public DateTime? AccountActivationDate { get; set; }
        public string PasswordHash { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public RoleModel[] Roles { get; set; }
    }
}
