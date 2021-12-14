using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Dx29.Web.Models;
using Dx29.Web.Services;
using Microsoft.Extensions.Localization;

namespace Dx29.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly LegacyClient _legacyClient;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            LegacyClient legacyClient,
            IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _legacyClient = legacyClient;
            _localizer = localizer;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "The field '{0}' is required")]
            [EmailAddress(ErrorMessage = "The '{0}' field is not a valid e-mail address.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "The field '{0}' is required")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    try
                    {
                        // Update LastLogin. Avoid exception here
                        var user = await _userManager.FindByEmailAsync(Input.Email);
                        user.LastLogin = DateTimeOffset.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }
                    catch { }

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                if (await _userManager.FindByEmailAsync(Input.Email) == null)
                {
                    // Try sign-in in legacy app
                    var legacyResponse = await _legacyClient.LegacySignInAsync(Input.Email, Input.Password);
                    if (legacyResponse.Status == "200")
                    {
                        // User is authorized in legacy app, create user in new app
                        var identityResult = await CreateLegacyUserAsync(Input.Email, Input.Password, legacyResponse.UserName, legacyResponse.Role, legacyResponse.Lang);
                        if (identityResult.Succeeded)
                        {
                            if (identityResult.RequiresPasswordUpdate)
                            {
                                // User's password is weak, request change password
                                return RedirectToPage("Legacy/UpdatePassword", new { returnUrl = returnUrl });
                            }
                            else
                            {
                                return LocalRedirect(returnUrl);
                            }
                        }
                    }
                    ModelState.AddModelError(string.Empty, _localizer["Invalid login attempt."]);
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer["Invalid login attempt."]);
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private async Task<LegacyResult> CreateLegacyUserAsync(string email, string password, string userName, string role, string lang)
        {
            var passwordEx = $"P@22w0rd{Input.Password}";

            // If user is already created but didn't chaged the weak password, request again
            var signResult = await _signInManager.PasswordSignInAsync(email, passwordEx, Input.RememberMe, lockoutOnFailure: false);
            if (signResult.Succeeded)
            {
                return new LegacyResult
                {
                    Succeeded = signResult.Succeeded,
                    RequiresPasswordUpdate = true
                };
            }

            // Create new user in new app
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = userName,
                LastName = "",
                Language = lang
            };
            role = ValidateRole(role);
            var legacyResult = await CreateLegacyUserAsync(user, email, password, role);
            if (!legacyResult.Succeeded)
            {
                // User password is weak. Mask the password to be valid and request for password update.
                legacyResult = await CreateLegacyUserAsync(user, email, passwordEx, role);
                legacyResult.RequiresPasswordUpdate = true;
            }
            return legacyResult;
        }

        private async Task<LegacyResult> CreateLegacyUserAsync(ApplicationUser user, string email, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Legacy user created a new account with password.");

                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation("Legacy user added to role {role}.", role);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    var signResult = await _signInManager.PasswordSignInAsync(email, password, Input.RememberMe, lockoutOnFailure: false);
                    return new LegacyResult
                    {
                        Succeeded = signResult.Succeeded
                    };
                }
            }
            return new LegacyResult
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors
            };
        }

        private string ValidateRole(string role)
        {
            if (role == "Patient" || role == "Physician")
            {
                return role;
            }
            return "Patient";
        }
    }

    public class LegacyResult
    {
        public bool Succeeded { get; set; }
        public bool RequiresPasswordUpdate { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
