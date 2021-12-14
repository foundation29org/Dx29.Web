using System;
using System.ComponentModel.DataAnnotations;

namespace Dx29.Data
{
    public class UserPreferences
    {
        public UserPreferences()
        {
            FirstName = "";
            Language = "en-US";
        }

        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name is too long.")]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessage = "Name is too long.")]
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string Language { get; set; }
    }
}
