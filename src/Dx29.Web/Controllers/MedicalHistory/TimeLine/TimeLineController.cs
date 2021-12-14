using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
    public class TimeLineController : ControllerBase
    {
        public TimeLineController(TimeLineService timeLineService, UserServices userServices)
        {
            TimeLineService = timeLineService;
            UserServices = userServices;
        }

        public TimeLineService TimeLineService { get; }
        public UserServices UserServices { get; }

        [HttpGet("{caseId}")]
        public async Task<IActionResult> GetTimeLineAsync(string caseId, string lang = null)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var timeLine = await TimeLineService.GetTimeLineAsync(userId, caseId, lang);
                return Ok(timeLine);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{caseId}")]
        public async Task<IActionResult> UpsertTimeLineAsync(string caseId, [FromBody] SymptomTimeline timeline)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await TimeLineService.UpsertTimeLineAsync(userId, caseId, timeline);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
