using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<IList<Symptom>> GetSymptomsAsync(string caseId, string name = null)
        {
            string query = name == null ? "" : $"?name={name}";
            return await HttpClient.GETAsync<IList<Symptom>>($"Symptoms/{caseId}{query}");
        }

        public async Task<IList<SymptomDesc>> GetSymptomsDescAsync(string caseId, string name = null)
        {
            string query = name == null ? $"?lang={Language}" : $"?name={name}&lang={Language}";
            return await HttpClient.GETAsync<IList<SymptomDesc>>($"Symptoms/{caseId}{query}");
        }

        public async Task UpsertSymptomsAsync(string caseId, IList<SymptomDesc> symptoms)
        {
            await UpsertSymptomsAsync(caseId, symptoms.Cast<Symptom>().ToList());
        }
        public async Task UpsertSymptomsAsync(string caseId, IList<Symptom> symptoms)
        {
            var symptomStatuses = symptoms.ToDictionary(r => r.Id, r => r.Status);
            await HttpClient.PUTAsync($"Symptoms/{caseId}", symptomStatuses);
        }
    }
}
