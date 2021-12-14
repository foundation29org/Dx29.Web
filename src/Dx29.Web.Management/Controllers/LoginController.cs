using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Dx29.Web.Models;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [ApiController]
    [Route("management/api/v1/[controller]")]
    public class LoginController : ControllerBase
    {
        public LoginController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public SignInManager<ApplicationUser> SignInManager { get; }
        public UserManager<ApplicationUser> UserManager { get; }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] UserModel model)
        {
            if (await ValidateUserAsync(model))
            {
                var token = TokenService.CreateToken(model.Email, "Admin");
                return Ok(token);
            }
            return Unauthorized();
        }

        private async Task<bool> ValidateUserAsync(UserModel model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await SignInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return await UserManager.IsInRoleAsync(user, "Admin");
                }
            }
            return false;
        }
    }
}
