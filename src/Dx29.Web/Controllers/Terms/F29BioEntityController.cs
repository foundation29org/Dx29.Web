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
    public class F29BioEntityController : ControllerBase
    {
        public F29BioEntityController(F29BioEntityClient F29bioEntityClient)
        {
            F29BioEntityClient = F29bioEntityClient;
        }

        public F29BioEntityClient F29BioEntityClient { get; }

       
        [HttpPost("disease/phenotypes")]
        public async Task<IActionResult> GetSymptomsOfDiseaseAsync([FromBody] string[] ids, string lang = "en")
        {
            try
            {
                var items = await F29BioEntityClient.GetSymptomsOfDiseaseAsync(ids, lang);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("disease/gene")]
        public async Task<IActionResult> GetGenesOfDiseaseAsync([FromBody] string[] ids)
        {
            try
            {
                var items = await F29BioEntityClient.GetGenesOfDiseaseAsync(ids);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
