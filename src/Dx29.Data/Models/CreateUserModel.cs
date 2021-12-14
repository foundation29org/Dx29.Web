using System;
using System.ComponentModel.DataAnnotations;

namespace Dx29.Web.Models
{
    public class CreateUserModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        [StringLength(100, ErrorMessage = "The {0} must at max {1} characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [StringLength(100, ErrorMessage = "The {0} must at max {1} characters long.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Language")]
        public string Language { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}
