using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Dx29.Web.Services;
using Dx29.Data;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("management/api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class DataAnalysisController : ControllerBase
    {
        public DataAnalysisController(DataAnalysisService dataAnalysisService, UserServices userServices)
        {
            DataAnalysisService = dataAnalysisService;
            UserServices = userServices;
        }

        public DataAnalysisService DataAnalysisService { get; }
        public UserServices UserServices { get; }

        [HttpGet("analysis/{caseId}")]
        public async Task<IActionResult> GetAnalysisListAsync(string caseId, string lang = "en")
        {
            try
            {
                string userId = UserServices.GetUserId();
                var item = await DataAnalysisService.GetAnalysisListAsync(userId, caseId, lang);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("analysis/{caseId}/{resourceId}")]
        public async Task<IActionResult> GetAnalysisAsync(string caseId, string resourceId, string lang = "en", int count = 100)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var item = await DataAnalysisService.GetAnalysisAsync(userId, caseId, resourceId, lang, count);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("process/{caseId}")]
        public async Task<IActionResult> GetOrCreateAnalysisAsync(string caseId, [FromBody] IList<string> symptoms)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await DataAnalysisService.GetOrCreateAnalysisAsync(userId, caseId, symptoms);
                items ??= new List<DiffDisease>();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("process/{caseId}")]
        public async Task<IActionResult> CreateAnalysisAsync(string caseId, [FromBody] IList<string> symptoms)
        {
            try
            {
                // TODO: Review
                string userId = UserServices.GetUserId();
                (var id, var items) = await DataAnalysisService.CreateAnalysisAsync(userId, caseId, symptoms);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("calculateDiagnosis")]
        public async Task<IActionResult> CalculateDiagnosisAsync([FromBody] DataAnalysisInfo dataAnalysisInfo)
        {
            try
            {
                IList<string> hpos = dataAnalysisInfo.symptoms;
                IList<DataAnalysisGene> genes = dataAnalysisInfo.genes;
                IList<DiffDisease> result = await DataAnalysisService.CalculateAsync(hpos, genes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("geneInput")]
        public async Task<IActionResult> GetGeneInputAsync([FromBody] ExomiserJSON exomiserJSON)
        {
            try
            {
                IList<DataAnalysisGene> result = await DataAnalysisService.GetGeneInputAsync(exomiserJSON);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("analysis/{caseId}/{resourceId}")]
        public async Task<IActionResult> DeleteAnalysisAsync(string caseId, string resourceId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await DataAnalysisService.DeleteAnalysisAsync(userId, caseId, resourceId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("geneReport/{caseId}/{resourceId}")]
        public async Task<IActionResult> GetGeneReportIdAsync(string caseId, string resourceId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                string item = await DataAnalysisService.GetGeneReportIdAsync(userId, caseId, resourceId);
                if (item != null)
                {
                    return Ok(item);
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
