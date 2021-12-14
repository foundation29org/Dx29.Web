using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dx29.Web.Models;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("management/api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsersController : ControllerBase
    {
        public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
        {
            UserManager = userManager;
            Logger = logger;
        }

        public UserManager<ApplicationUser> UserManager { get; }
        public ILogger<UsersController> Logger { get; }

        

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserModel model)
        {
            if (model.Role != "Patient" && model.Role != "Physician")
            {
                return BadRequest("Invalid used Role.");
            }

            try
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Language = model.Language
                };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    Logger.LogInformation("New user account created with password.");
                    await UserManager.AddToRoleAsync(user, "User");
                    await UserManager.AddToRoleAsync(user, model.Role);
                    Logger.LogInformation("User added to role {role}.", model.Role);

                    // Confirm user
                    var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
                    result = await UserManager.ConfirmEmailAsync(user, token);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                }
                return BadRequest(result.Errors.FirstOrDefault()?.Description);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserIdByEmailAsync([FromQuery] string email)
        {
            Console.WriteLine("GetUserIdByEmailAsync Controller");
            try
            {
                Console.WriteLine(email);
                var user = await UserManager.FindByEmailAsync(email);
                Console.WriteLine(user);
                if (user != null)
                {
                    return Ok(user.Id);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.....................");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync(string email)
        {
            try
            {
                var user = await UserManager.FindByEmailAsync(email);
                if (user != null)
                {
                    await UserManager.DeleteAsync(user);
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
