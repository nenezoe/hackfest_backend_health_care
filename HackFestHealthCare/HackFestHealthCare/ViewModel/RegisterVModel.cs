using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HackFestHealthCare.ViewModel
{
    public class RegisterVModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Password { get; set; }
        public string RTPassword { get; set; }
    }

    public class TokenRequestModel
    {
        public string GrantType { get; set; }
        public string RefreshToken { get; set; }

        [Required(ErrorMessage = "Username or email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(50, ErrorMessage = "Invalid string length")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(30, ErrorMessage = "Invalid string length")]
        public string Password { get; set; }
    }

    public class TokenResponseModel
    {
        public string token { get; set; } 
        public Guid userId { get; set; } 
        public DateTime expiration { get; set; } 
        public string refresh_token { get; set; }
        public string[] roles { get; set; }
    }

    public class AddUserVModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Password { get; set; }
        public string RTPassword { get; set; }
        [Required]
        public string[] Roles { get; set; }
    }

    public class UserProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Current Password is required")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
