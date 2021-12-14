using System;
using System.Collections.Generic;
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
    public class DiagnosisController : ControllerBase
    {
        public DiagnosisController(DiagnosisService diagnosisService, UserServices userServices)
        {
            DiagnosisService = diagnosisService;
            UserServices = userServices;
        }

        public DiagnosisService DiagnosisService { get; }
        public UserServices UserServices { get; }


        [HttpGet("phensimilarity/{caseId}")]
        public async Task<IActionResult> GetPhenSimilarityAsync(string caseId, string diseaseId, string lang = "en")
        {
            try
            {
                string userId = UserServices.GetUserId();
                var item = await DiagnosisService.GetPhenSimilarityAsync(userId, caseId, lang, diseaseId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("moreinfo")]
        public async Task<IActionResult> GetMoreInfoAsync(string diseaseId, string lang = "en")
        {
            try
            {
                var item = await DiagnosisService.GetMoreInfoAsync(diseaseId, lang);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("moreinfoList")]
        public async Task<IActionResult> GetMoreInfoListAsync(string diseaseId, string lang = "en")
        {
            try
            {
                var item = await DiagnosisService.GetMoreInfoListAsync(diseaseId, lang);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("moreinfoSelected")]
        public async Task<IActionResult> GetMoreInfoSelectedAsync(string text, string lang = "en")
        {
            try
            {
                var item = await DiagnosisService.GetMoreInfoSelectedAsync(text, lang);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("clinicaltrials")]
        public async Task<IActionResult> GetClinicalTrialsAsync(string diseaseId)
        {
            try
            {
                var item = await DiagnosisService.GetClinicalTrialsAsync(diseaseId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("patientgroups")]
        public async Task<IActionResult> GetPatientGroupsAsync(string diseaseId)
        {
            try
            {
                var item = await DiagnosisService.GetPatientGroupsAsync(diseaseId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
