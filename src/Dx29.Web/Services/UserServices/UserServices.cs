using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using Dx29.Data;
using Dx29.Services;
using Dx29.Web.Data;
using Dx29.Web.Models;

namespace Dx29.Web.Services
{
    public class UserServices
    {
        public UserServices(IHttpContextAccessor contextAccessor, UserManager<Dx29.Web.Models.ApplicationUser> userManager, AccountHashService accountHashService, ApplicationDbContext applicationDbContext)
        {
            ContextAccessor = contextAccessor;
            UserManager = userManager;
            AccountHashService = accountHashService;
            ApplicationDbContext = applicationDbContext;
        }

        public IHttpContextAccessor ContextAccessor { get; }
        public UserManager<Dx29.Web.Models.ApplicationUser> UserManager { get; }
        public AccountHashService AccountHashService { get; }
        public ApplicationDbContext ApplicationDbContext { get; }

        public string GetUserId()
        {
            return GetUserId(ContextAccessor.HttpContext.User);
        }

        public string GetUserId(ClaimsPrincipal principal)
        {
            if (principal != null)
            {
                if (principal.Identity.IsAuthenticated)
                {
                    string identifier = principal.FindFirst(ClaimTypes.Email)?.Value;
                    if (identifier != null)
                    {
                        return GetUserIdentifier(identifier);
                    }
                }
            }
            throw new InvalidOperationException("Authentication Error");
        }

        private string GetUserIdentifier(string identifier)
        {
            return AccountHashService.GetHash(identifier);
        }

        public async Task<UserPreferences> GetPreferencesAsync()
        {
            var user = await UserManager.GetUserAsync(ContextAccessor.HttpContext.User);
            return BuildUserPreferences(user);
        }

        public async Task<UserPreferences> SetPreferencesAsync(UserPreferences preferences)
        {
            var user = await UserManager.GetUserAsync(ContextAccessor.HttpContext.User);
            user.FirstName = preferences.FirstName;
            user.LastName = preferences.LastName;
            user.Language = preferences.Language;
            await UserManager.UpdateAsync(user);
            return BuildUserPreferences(user);
        }

        static private UserPreferences BuildUserPreferences(Dx29.Web.Models.ApplicationUser user)
        {
            return new UserPreferences
            {
                Email = user.Email,
                FirstName = user.FirstName ?? user.Email,
                LastName = user.LastName,
                Language = user.Language ?? "en-US",
                UserName = BuildUserName(user.FirstName, user.LastName, user.Email)
            };
        }

        static private string BuildUserName(string firstName, string lastName, string email)
        {
            string userName = $"{firstName} {lastName}".TrimEnd(' ').TrimEnd(',');
            if (String.IsNullOrEmpty(userName))
            {
                return email;
            }
            return userName;
        }
    }
}
