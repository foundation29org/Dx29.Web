using System;
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
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PatientsController : ControllerBase
    {
        public PatientsController(PatientService patientService, UserServices userServices)
        {
            PatientService = patientService;
            UserServices = userServices;
        }

        public PatientService PatientService { get; }
        public UserServices UserServices { get; }

        [HttpGet]
        public async Task<IActionResult> GetMedicalCasesAsync()
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await PatientService.GetPatientsAsync(userId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{caseId}")]
        public async Task<IActionResult> GetMedicalCaseAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var item = await PatientService.GetPatientAsync(userId, caseId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("summary/{caseId}")]
        public async Task<IActionResult> GetCaseSummaryAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var item = await PatientService.GetCaseSummaryAsync(userId, caseId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatientAsync([FromBody] CreatePatitentModel model)
        {
            try
            {
                Console.WriteLine("CreatePatientAsync Controller");
                Console.WriteLine(model.Serialize(true));
                string userId = UserServices.GetUserId();
                var item = await PatientService.CreatePatientAsync(userId, model);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{caseId}")]
        public async Task<IActionResult> UpdatePatientAsync(string caseId, [FromBody] PatientInfoModel model)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await PatientService.UpdatePatientAsync(userId, caseId, model);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{caseId}")]
        public async Task<IActionResult> DeletePatientAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await PatientService.DeletePatientAsync(userId, caseId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //
        //  Sharing
        //
        [HttpPost("share/{caseId}")]
        public async Task<IActionResult> ShareMedicalCaseAsync(string caseId, ShareModel model, [FromQuery] string action)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var item = await PatientService.ShareMedicalCaseAsync(userId, caseId, model.Email, model.Message, action);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("unshare/{caseId}")]
        public async Task<IActionResult> StopSharingMedicalCaseAsync(string caseId, ShareModel model)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await PatientService.StopSharingMedicalCaseAsync(userId, caseId, model.Email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
