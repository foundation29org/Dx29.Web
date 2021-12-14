using System;
using System.ComponentModel.DataAnnotations;

namespace Dx29.Web.Models
{
    public class ShareModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Message { get; set; }

    }
}
