﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class SymptomsController : ControllerBase
    {
        public SymptomsController(DataAnalysisService dataAnalysisService, SymptomsService symptomsService, UserServices userServices)
        {
            DataAnalysisService = dataAnalysisService;
            SymptomsService = symptomsService;
            UserServices = userServices;
        }

        public DataAnalysisService DataAnalysisService { get; }
        public SymptomsService SymptomsService { get; }
        public UserServices UserServices { get; }

        [HttpGet("{caseId}")]
        public async Task<IActionResult> GetSymptomsAsync(string caseId, string name = null, string lang = null)
        {
            try
            {
                string userId = UserServices.GetUserId();
                if (String.IsNullOrEmpty(lang))
                {
                    var symptoms = await SymptomsService.GetSymptomsAsync(userId, caseId, name);
                    return Ok(symptoms);
                }
                else
                {
                    var symptoms = await SymptomsService.GetSymptomsDescAsync(userId, caseId, name, lang);
                    return Ok(symptoms);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{caseId}")]
        public async Task<IActionResult> UpsertSymptomsAsync(string caseId, [FromBody] IDictionary<string, string> symptomStatuses)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await SymptomsService.UpsertSymptomsAsync(userId, caseId, symptomStatuses);
                var symptoms = await SymptomsService.GetSymptomsAsync(userId, caseId);
                var selected = symptoms.Where(r => r.IsKeySymptom).Select(r => r.Id).ToList();
                await DataAnalysisService.CreateAnalysisAsync(userId, caseId, selected);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
