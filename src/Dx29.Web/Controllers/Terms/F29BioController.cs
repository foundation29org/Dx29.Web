using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Dx29.Data;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class F29BioController : ControllerBase
    {
        public F29BioController(F29BioEntityClient f29BioEntityClient, BioEntityClient bioEntityClient)
        {
            F29BioEntityClient = f29BioEntityClient;
            BioEntityClient = bioEntityClient;
        }

        public F29BioEntityClient F29BioEntityClient { get; }
        public BioEntityClient BioEntityClient { get; }

        [HttpGet("phenotypes/{lang}/{ids}")]
        public async Task<IActionResult> GetSymptomsAsync(string ids, string lang = "en")
        {
            try
            {
                var dic = await DescribeSymptomsAsync(ids.Split(',').Select(r => r.Trim()).ToArray(), lang);
                return Ok(dic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("phenotypes/{lang}")]
        public async Task<IActionResult> PostSymptomsAsync([FromBody] string[] ids, string lang = "en")
        {
            try
            {
                var dic = await DescribeSymptomsAsync(ids, lang);
                return Ok(dic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("diseases/{lang}/{ids}")]
        public async Task<IActionResult> GetDiseasesAsync(string ids, string lang = "en")
        {
            try
            {
                var dic = await DescribeDiseasesAsync(ids.Split(',').Select(r => r.Trim()).ToArray(), lang);
                return Ok(dic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("diseases/{lang}")]
        public async Task<IActionResult> PostDiseasesAsync([FromBody] string[] ids, string lang = "en")
        {
            try
            {
                var dic = await DescribeDiseasesAsync(ids, lang);
                return Ok(dic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("disease/phenotypes/{lang}/{ids}")]
        public async Task<IActionResult> GetDiseasePhenotypesAsync(string ids, string lang = "en")
        {
            try
            {
                var dic = await DescribeDiseasePhenotypesAsync(ids.Split(',').Select(r => r.Trim()).ToArray(), lang);
                return Ok(dic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("disease/phenotypes/{lang}")]
        public async Task<IActionResult> PostDiseasePhenotypesAsync([FromBody] string[] ids, string lang = "en")
        {
            try
            {
                var dic = await DescribeDiseasePhenotypesAsync(ids, lang);
                return Ok(dic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<Dictionary<string, F29Symptom>> DescribeSymptomsAsync(string[] ids, string lang)
        {
            var dic = new Dictionary<string, F29Symptom>();
            var items = await BioEntityClient.DescribeTermsAsync(ids, lang);
            foreach (var item in items)
            {
                var symp = item.Value.FirstOrDefault();
                if (symp != null)
                {
                    dic[item.Key] = F29Symptom.FromSymptom(symp);
                }
            }

            return dic;
        }

        private async Task<Dictionary<string, F29Disease>> DescribeDiseasesAsync(string[] ids, string lang)
        {
            var dic = new Dictionary<string, F29Disease>();
            var items = await BioEntityClient.DescribeTermsAsync(ids, lang);
            foreach (var item in items)
            {
                var dise = item.Value.FirstOrDefault();
                if (dise != null)
                {
                    dic[item.Key] = F29Disease.FromDisease(dise);
                }
            }

            return dic;
        }

        private async Task<Dictionary<string, F29Disease>> DescribeDiseasePhenotypesAsync(string[] ids, string lang)
        {
            var dic = new Dictionary<string, F29Disease>();
            var diseases = await BioEntityClient.DescribeTermsAsync(ids, lang);
            var diseaseSymptoms = await F29BioEntityClient.GetSymptomsOfDiseaseAsync(ToUpper(ids), lang);
            foreach (var item in diseases)
            {
                var dise = item.Value.FirstOrDefault();
                if (dise != null)
                {
                    var disease = F29Disease.FromDisease(dise);
                    var symptoms = diseaseSymptoms.TryGetValue(item.Key.ToUpper());
                    if (symptoms != null)
                    {
                        foreach (var symptom in symptoms)
                        {
                            disease.phenotypes[symptom.Id.ToUpper()] = F29Symptom.FromSymptom(symptom);
                        }
                    }
                    dic[item.Key] = disease;
                }
            }

            return dic;
        }

        private string[] ToUpper(string[] ids)
        {
            return ids?.Select(r => r.ToUpper())?.ToArray() ?? new string[] { };
        }
    }
}
