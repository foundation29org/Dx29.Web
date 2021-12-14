using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Dx29.Web.Models;
using Dx29.Web.Services;

namespace Dx29.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string ReturnUrl { get; set; }

        [BindProperty]
        public ApplicationUser AppUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string token, string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if ((userId == null) || (token == null))
            {
                return RedirectToPage("/");
            }
            ApplicationUser AppUser = await _userManager.FindByIdAsync(userId);

            var userFound = await _userManager.FindByEmailAsync(AppUser.Email);
            if (userFound == null)
            {
                return NotFound($"Unable to load user '{AppUser.Email}'.");
            }
            return Page();
        }
    }
}
