using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Dx29.Web.Resources;

using Dx29.Web.Models;

namespace Dx29.Web.Areas.Identity.Pages.Account.Legacy
{
    public class UpdatePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<UpdatePasswordModel> _logger;

        public UpdatePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<UpdatePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "The field '{0}' is required")]
            [PasswordDigit(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_digit")]
            [PasswordSpecialCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_non_alphanumeric_character")]
            [PasswordUpperCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_upper_case_character")]
            [PasswordLowerCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_lower_case_character")]
            [PasswordLenght(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_6_characters_long")]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "The field '{0}' is required")]
            [PasswordDigit(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_digit")]
            [PasswordSpecialCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_non_alphanumeric_character")]
            [PasswordUpperCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_upper_case_character")]
            [PasswordLowerCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_lower_case_character")]
            [PasswordLenght(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_6_characters_long")]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            returnUrl ??= Url.Content("~/");

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var oldPassword = $"P@22w0rd{Input.OldPassword}";
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return LocalRedirect(returnUrl);
        }
    }
}
