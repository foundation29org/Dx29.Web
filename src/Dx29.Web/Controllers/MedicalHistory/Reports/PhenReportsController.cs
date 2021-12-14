using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Dx29.Data;
using Dx29.Web.Models;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PhenReportsController : ControllerBase
    {
        public PhenReportsController(PhenReportsService phenReportsService, UserServices userServices)
        {
            PhenReportsService = phenReportsService;
            UserServices = userServices;
        }

        public PhenReportsService PhenReportsService { get; }
        public UserServices UserServices { get; }

        [HttpGet("all/{caseId}")]
        public async Task<IActionResult> GetReportsAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await PhenReportsService.GetReportsAsync(userId, caseId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("annotations/{caseId}/{reportId}")]
        public async Task<IActionResult> GetAnnotationsAsync(string caseId, string reportId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var anns = await PhenReportsService.GetAnnotationsAsync(userId, caseId, reportId);
                return Ok(anns);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("process/{caseId}")]
        public async Task<IActionResult> ProcessReportAsync(string caseId, [FromBody] FileModel model)
        {
            try
            {
                if (model.ResourceId != null)
                {
                    string userId = UserServices.GetUserId();
                    var status = await PhenReportsService.ProcessFileAsync(userId, caseId, model);
                    return Ok(status);
                }
                return BadRequest("ResourceId cannot be null.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{caseId}/{reportId}")]
        public async Task<IActionResult> DeleteReportAsync(string caseId, string reportId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await PhenReportsService.DeleteReportAsync(userId, caseId, reportId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("process")]
        public async Task<IActionResult> ProcessTextAsync([FromBody] TextDocument doc)
        {
            try
            {
                var auth = Request.Headers["Authorization"];
                if (PhenReportsService.AnnotationsClient.Authorization == auth)
                {
                    if (!String.IsNullOrEmpty(doc?.Text ?? ""))
                    {
                        var results = await PhenReportsService.ProcessTextAsync(doc.Text);
                        return Ok(results);
                    }
                    return Ok(new List<DocAnnotations>());
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
