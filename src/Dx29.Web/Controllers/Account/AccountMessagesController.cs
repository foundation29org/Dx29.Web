using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Dx29.Web.Models;

namespace Dx29.Web.Controllers.Account
{
    [Route("[controller]")]
    public class AccountMessagesController : Controller
    {
        public AccountMessagesController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string token, string email)
        {
            var user = await UserManager.FindByEmailAsync(email);
            if (user == null)
                return View("ConfirmEmailError");

            var result = await UserManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "ConfirmEmailOK" : "ConfirmEmailError");
        }
    }
}
