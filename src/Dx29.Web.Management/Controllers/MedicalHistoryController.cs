using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("management/api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class MedicalHistoryController : ControllerBase
    {
        public MedicalHistoryController(MedicalHistoryClient medicalHistoryClient, UserServices userServices)
        {
            MedicalHistoryClient = medicalHistoryClient;
            UserServices = userServices;
        }

        public MedicalHistoryClient MedicalHistoryClient { get; }
        public UserServices UserServices { get; }

        [HttpGet("medicalcase/{caseId}")]
        public async Task<IActionResult> GetMedicalCaseAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
                return Ok(medicalCase);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("resourcegroups/{caseId}")]
        public async Task<IActionResult> GetResourceGroupsAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var resourceGroups = await MedicalHistoryClient.GetResourceGroupsAsync(userId, caseId);
                return Ok(resourceGroups);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
