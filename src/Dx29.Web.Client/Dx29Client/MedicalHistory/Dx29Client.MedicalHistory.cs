using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<MedicalCase> GetMedicalCaseAsync(string caseId)
        {
            return await HttpClient.GETAsync<MedicalCase>($"MedicalHistory/medicalcase/{caseId}");
        }

        public async Task<IList<ResourceGroup>> GetResourceGroupsAsync(string caseId)
        {
            return await HttpClient.GETAsync<IList<ResourceGroup>>($"MedicalHistory/resourcegroups/{caseId}");
        }
    }
}
