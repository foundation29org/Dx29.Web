using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class BioEntityController : ControllerBase
    {
        public BioEntityController(BioEntityClient bioEntityClient)
        {
            BioEntityClient = bioEntityClient;
        }

        public BioEntityClient BioEntityClient { get; }

        [HttpPost("symptoms")]
        public async Task<IActionResult> DescribeSymptomsAsync([FromBody] string[] ids, string lang = "en")
        {
            try
            {
                var items = await BioEntityClient.DescribeSymptomsAsync(ids, lang);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("conditions")]
        public async Task<IActionResult> DescribeConditionsAsync([FromBody] string[] ids, string lang = "en")
        {
            try
            {
                var items = await BioEntityClient.DescribeConditionsAsync(ids, lang);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("terms")]
        public async Task<IActionResult> DescribeTermsAsync([FromBody] string[] ids, string lang = "en")
        {
            try
            {
                var items = await BioEntityClient.DescribeTermsAsync(ids, lang);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
