using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
    [Route("management/api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class GeneReportsController: ControllerBase
    {
        public GeneReportsController(GeneReportsService geneReportsService, UserServices userServices)
        {
            GeneReportsService = geneReportsService;
            UserServices = userServices;
        }

        public GeneReportsService GeneReportsService { get; }
        public UserServices UserServices { get; }

        [HttpGet("all/{caseId}")]
        public async Task<IActionResult> GetReportsAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await GeneReportsService.GetReportsAsync(userId, caseId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("results/{caseId}/{resourceId}")]
        public async Task<IActionResult> GetReportResultAsync(string caseId, string resourceId, [FromQuery] string selectedGenes)
        {
            try
            {
                string userId = UserServices.GetUserId();
                if (String.IsNullOrEmpty(selectedGenes))
                {
                    var items = await GeneReportsService.GetReportResultAsync(userId, caseId, resourceId);
                    return Ok(items);
                }
                else
                {
                    var selectedGenesList = selectedGenes.Split(',');
                    var items = await GeneReportsService.GetFilterReportResultAsync(userId, caseId, resourceId, selectedGenesList);
                    return Ok(items);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("exomiser/{caseId}/{resourceId}")]
        public async Task<IActionResult> GetExomiserResultAsync(string caseId, string resourceId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await GeneReportsService.GetExomiserResultAsync(userId, caseId, resourceId);
                return Ok(items);
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
                    var status = await GeneReportsService.ProcessFileAsync(userId, caseId, model);
                    return Ok(status);
                }
                return BadRequest("ResourceId cannot be null.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{caseId}")]
        public async Task<IActionResult> ProcessReportAsync(string caseId, [FromBody] GenotypeProcess process)
        {
            try
            {
                if (process.ResourceId != null)
                {
                    string userId = UserServices.GetUserId();
                    var status = await GeneReportsService.ProcessFileAsync(userId, caseId, process);
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
                await GeneReportsService.DeleteReportAsync(userId, caseId, reportId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Notification")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Callback([FromBody] ExomiserNotification notification)
        {
            try
            {
                await GeneReportsService.HandleNotificationAsync(notification);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
