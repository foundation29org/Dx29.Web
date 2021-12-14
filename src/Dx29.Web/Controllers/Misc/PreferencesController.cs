using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Dx29.Data;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PreferencesController : ControllerBase
    {
        public PreferencesController(UserServices userServices)
        {
            UserServices = userServices;
        }

        public UserServices UserServices { get; }

        [HttpGet]
        public async Task<IActionResult> GetPreferencesAsync()
        {
            try
            {
                var resp = await UserServices.GetPreferencesAsync();
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> SetPreferencesAsync([FromBody] UserPreferences userPreferences)
        {
            try
            {
                var resp = await UserServices.SetPreferencesAsync(userPreferences);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
