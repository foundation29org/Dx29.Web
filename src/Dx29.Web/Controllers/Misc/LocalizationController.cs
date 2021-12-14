using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class LocalizationController : ControllerBase
    {
        public LocalizationController(LocalizationClient localizationClient)
        {
            LocalizationClient = localizationClient;
        }

        public LocalizationClient LocalizationClient { get; }

        [HttpGet("literals")]
        public async Task<IActionResult> GetLiteralsAsync(string lang = "en")
        {
            try
            {
                var items = await LocalizationClient.GetLiteralsAsync(lang);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("literal")]
        public async Task<IActionResult> GetLiteralAsync(string key, string lang = "en")
        {
            try
            {
                var item = await LocalizationClient.GetLiteralAsync(lang, key);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("literals")]
        public async Task<IActionResult> SetLiteralsAsync([FromBody] IDictionary<string, string> literals, string lang = "en")
        {
            try
            {
                await LocalizationClient.SetLiteralsAsync(lang, literals);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("literal")]
        public async Task<IActionResult> SetLiteralAsync([FromBody] KeyValuePair<string, string> literal, string lang = "en")
        {
            try
            {
                await LocalizationClient.SetLiteralAsync(lang, literal);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("register")]
        public async Task<IActionResult> RegisterLiteralAsync([FromBody] KeyValuePair<string, string> literal, string lang = "en")
        {
            try
            {
                var value = await LocalizationClient.RegisterLiteralAsync(lang, literal);
                return Ok(value);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("literal")]
        public async Task<IActionResult> DeleteLiteralAsync(string key, string lang = "en")
        {
            try
            {
                await LocalizationClient.DeleteLiteralAsync(lang, key);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
