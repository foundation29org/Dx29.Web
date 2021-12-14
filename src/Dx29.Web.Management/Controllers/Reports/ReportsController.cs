using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Dx29.Web.Models;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("management/api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class ReportsController : ControllerBase
    {
        public ReportsController(PhenReportsService phenReportsService, GeneReportsService geneReportsService, UserServices userServices)
        {
            PhenReportsService = phenReportsService;
            GeneReportsService = geneReportsService;
            UserServices = userServices;
        }

        public PhenReportsService PhenReportsService { get; }
        public GeneReportsService GeneReportsService { get; }
        public UserServices UserServices { get; }

        [HttpGet("{caseId}")]
        public async Task<IActionResult> GetReportsAsync(string caseId)
        {
            try
            {
                var items = new List<ReportModel>();

                string userId = UserServices.GetUserId();
                foreach (var item in await PhenReportsService.GetReportsAsync(userId, caseId))
                {
                    items.Add(item);
                }
                foreach (var item in await GeneReportsService.GetReportsAsync(userId, caseId))
                {
                    items.Add(item);
                }
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
                var items = await PhenReportsService.GetAnnotationsAsync(userId, caseId, reportId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("genotype/{caseId}/{reportId}")]
        public async Task<IActionResult> GetGenotypeAsync(string caseId, string reportId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await GeneReportsService.GetReportResultAsync(userId, caseId, reportId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{caseId}")]
        public async Task<IActionResult> ProcessReportAsync(string caseId, [FromBody] FileModel model)
        {
            try
            {
                if (model.ResourceId != null)
                {
                    string userId = UserServices.GetUserId();
                    if (model.IsPhenotype)
                    {
                        var status = await PhenReportsService.ProcessFileAsync(userId, caseId, model);
                        return Ok(status);
                    }
                    else
                    {
                        var status = await GeneReportsService.ProcessFileAsync(userId, caseId, model);
                        return Ok(status);
                    }
                }
                return BadRequest("ResourceId cannot be null.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
