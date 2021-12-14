using System;
using System.ComponentModel.DataAnnotations;

namespace Dx29.Web.Models
{
    public class UserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
