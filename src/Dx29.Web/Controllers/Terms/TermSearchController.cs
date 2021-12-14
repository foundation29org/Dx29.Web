using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Dx29.Data;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class TermSearchController : ControllerBase
    {
        public TermSearchController(TermSearchClient termSearchClient, AnnotationsClient annotationsClient, BioEntityClient bioEntityClient)
        {
            TermSearchClient = termSearchClient;
            AnnotationsClient = annotationsClient;
            BioEntityClient = bioEntityClient;
        }

        public TermSearchClient TermSearchClient { get; }
        public AnnotationsClient AnnotationsClient { get; }
        public BioEntityClient BioEntityClient { get; }

        [HttpGet("symptoms")]
        public async Task<IActionResult> SearchSymptomsAsync(string text, string lang = "en", int rows = 10)
        {
            try
            {
                var items = await TermSearchClient.SearchSymptomsAsync(text, lang, rows);
                return Ok(items);
            }
            catch
            {
                return Ok(new TermDesc[] { });
            }
        }

        [HttpGet("diseases")]
        public async Task<IActionResult> SearchDiseasesAsync(string text, string lang = "en", int rows = 10)
        {
            try
            {
                var items = await TermSearchClient.SearchDiseasesAsync(text, lang, rows);
                return Ok(items);
            }
            catch
            {
                return Ok(new TermDesc[] { });
            }
        }

        [HttpPost("symptoms")]
        public async Task<IActionResult> SearchSymptomsAsync([FromBody] TextDocument document, [FromQuery] string lang = "en")
        {
            var resp = await AnnotationsClient.SyncProcessFileAsync(document.Text);
            var items = await ExtractSymptoms(resp, lang);
            return Ok(items);
        }

        private async Task<IList<TermDesc>> ExtractSymptoms(IList<DocAnnotations> documents, string lang)
        {
            var dic = new Dictionary<string, TermDesc>();

            foreach (var document in documents)
            {
                foreach (var segment in document.Segments)
                {
                    foreach (var annotation in segment.Annotations.Where(r => r.Links != null))
                    {
                        foreach (var link in annotation.Links.Where(r => r.DataSource.EqualsNoCase("HPO")))
                        {
                            dic[link.Id] = new TermDesc { Id = link.Id };
                        }
                    }
                }
            }
            await DescribeSymptomsAsync(dic, lang);
            return dic.Values.ToArray();
        }

        #region Describe Symptoms
        private async Task DescribeSymptomsAsync(IDictionary<string, TermDesc> symptoms, string lang)
        {
            var ids = symptoms.Keys.ToArray();
            var dic = await BioEntityClient.DescribeSymptomsAsync(ids, lang);
            foreach (var symptom in symptoms.Values)
            {
                var terms = dic[symptom.Id];
                if (terms.Count > 0)
                {
                    var term = terms.First();
                    symptom.Name = term.Name;
                    symptom.Desc = term.Desc;
                    symptom.Categories = term.Categories;
                }
            }
        }
        #endregion
    }
}
