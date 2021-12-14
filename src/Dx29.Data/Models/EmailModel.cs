using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Dx29.Data;
using Dx29.Data.Resources;

namespace Dx29.Web.Models
{
    
    public class EmailModel
    {
        public string Message { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "The_email_address_is_required")]
        [EmailAddress(ErrorMessageResourceType = typeof(UISharedResource), ErrorMessageResourceName = "Invalid_Email_Address")]
        public string EmailContact { get; set; }
    }

    
}
