using System;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    partial class MedicalHistoryClient
    {
        //
        //  SharedBy
        //
        public async Task<SharedBy> GetSharedByAsync(string userId, string caseId)
        {
            return await HttpClient.GETAsync<SharedBy>($"MedicalCaseShare/{userId}/{caseId}");
        }

        //
        //  Share
        //
        public async Task<MedicalCase> ShareMedicalCaseAsync(string userId, string caseId, string email, string action)
        {
            var model = new { Email = email, Action = action };
            return await HttpClient.POSTAsync<MedicalCase>($"MedicalCaseShare/{userId}/{caseId}", model);
        }

        //
        //  Stop Sharing
        //
        public async Task StopSharingMedicalCaseAsync(string userId, string caseId, string email)
        {
            var model = new { Email = email };
            await HttpClient.PATCHAsync($"MedicalCaseShare/{userId}/{caseId}", model);
        }
    }
}
