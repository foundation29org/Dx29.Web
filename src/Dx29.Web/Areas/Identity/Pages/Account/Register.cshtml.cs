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

using System.Globalization;
using Dx29.Web.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace Dx29.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly EmailHelper _emailHelper;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            EmailHelper emailHelper,
            DocumentsService documentsService,
            OpenDataService openDataService,
            IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailHelper = emailHelper;
            DocumentsService = documentsService;
            OpenDataService = openDataService;
        }

        public DocumentsService DocumentsService { get; set; }
        public OpenDataService OpenDataService { get; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string TermsAndConditions { get; set; } = "";
        public string DataAgreement { get; set; } = "";
        public class InputModel
        {
            [Required(ErrorMessage = "The field '{0}' is required")]
            [EmailAddress(ErrorMessage = "The '{0}' field is not a valid e-mail address.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "The field '{0}' is required")]
            [PasswordDigit(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_digit")]
            [PasswordSpecialCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_non_alphanumeric_character")]
            [PasswordUpperCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_upper_case_character")]
            [PasswordLowerCase(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_one_lower_case_character")]
            [PasswordLenght(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "The_password_must_have_at_least_6_characters_long")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "The field '{0}' is required")]
            [Display(Name = "FirstName")]
            [StringLength(100, ErrorMessage = "The {0} must at max {1} characters long.")]
            public string FirstName { get; set; }

            [Display(Name = "LastName")]
            [StringLength(100, ErrorMessage = "The {0} must at max {1} characters long.")]
            public string LastName { get; set; }

            public string Language { get; set; }

            [Required(ErrorMessage = "The field '{0}' is required")]
            [Display(Name = "Role")]
            public string Role { get; set; }

            [Required(ErrorMessage = "The field '{0}' is required")]
            [IsTrue(ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "You_must_accept_the_privacy_policy")]
            [Display(Name = "I have read and understood the")]
            public bool Privacy { get; set; }

            [Required(ErrorMessage = "The field '{0}' is required")]
            [IsTrueConditional("Role", "Physician", ErrorMessageResourceType = typeof(SharedPublicResource), ErrorMessageResourceName = "You_must_agree_with_the_data_processing_by_Dx29")]
            [Display(Name = "I agree with the")]
            public bool DataAgreement { get; set; }

            public string OpenData { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null, string opendata = null)
        {
            Input = new InputModel { OpenData = opendata };
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            await GetTermsAndConditionsAsync(CultureInfo.CurrentCulture.Name);
            await GetDataAgreementAsync(CultureInfo.CurrentCulture.Name);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {                Input.Language = SetInputLanguage(CultureInfo.CurrentCulture.Name);
                var user = new ApplicationUser
                {
                    Email = Input.Email,
                    UserName = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Language = Input.Language,
                    CreatedOn = DateTimeOffset.UtcNow
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (Input.Role == "Patient" || Input.Role == "Physician")
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        await _userManager.AddToRoleAsync(user, Input.Role);
                        _logger.LogInformation("User added to role {role}.", Input.Role);
                    }
                    else
                    {
                        _logger.LogInformation("Invalid role {role}.", Input.Role);
                    }
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "AccountMessages", new { token, email = user.Email }, Request.Scheme);
                    bool sendOk = await _emailHelper.SendConfirmationEmailAsync(Input.Email, Input.Language, confirmationLink);

                    // OpenData: Create case if OpenData is available
                    if (!String.IsNullOrEmpty(Input.OpenData))
                    {
                        CreateOpenDataCase(user.Email, Input.OpenData);
                    }

                    if (sendOk)
                    {
                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { userId = user.Id, token = token, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Can not send email confirmation account for {email}.", Input.Email);
                        ModelState.AddModelError(string.Empty, _localizer["Fail sending email. Your user account is created correctly but we can send you the confirmation email. Please contact with us for solving this issue."]);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            await GetTermsAndConditionsAsync(CultureInfo.CurrentCulture.Name);
            await GetDataAgreementAsync(CultureInfo.CurrentCulture.Name);

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private string SetInputLanguage(string language)
        {
            if((language == "es")||(language == "es-ES"))
            {
                return "es-ES";
            }
            else
            {
                return "en-US";
            }
        }
        private async void CreateOpenDataCase(string email, string openData)
        {
            try
            {
                await OpenDataService.CreateOpenDataCaseAsync(email, openData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task GetTermsAndConditionsAsync(string language)
        {
            TermsAndConditions = (await DocumentsService.Download("TermsAndConditions", "termsAndConditions.txt", language)).Replace("\r\n", "");
        }

        public async Task GetDataAgreementAsync(string language)
        {
            DataAgreement = (await DocumentsService.Download("TermsAndConditions", "dataProcessingAgreement.txt", language)).Replace("\r\n", "");
        }
    }
}
