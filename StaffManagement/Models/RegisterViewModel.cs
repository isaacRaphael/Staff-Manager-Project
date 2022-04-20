using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StaffManagement.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DisplayName("User Role")]
        public string UserRole { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password must match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DisplayName("Upload Profile Picture...")]
        public IFormFile Photo { get; set; }
    }
}
