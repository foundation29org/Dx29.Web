using System;
using Microsoft.AspNetCore.Identity;

namespace Dx29.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Language { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastLogin { get; set; }
    }
}
