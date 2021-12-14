using System;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        //
        //  Share
        //
        public async Task<MedicalCase> ShareMedicalCaseAsync(string caseId, string email, string message)
        {
            var model = new { Email = email, Message = message};
            return await HttpClient.POSTAsync<MedicalCase>($"Patients/share/{caseId}?action=create", model);
        }

        public async Task<MedicalCase> AcceptSharingMedicalCaseAsync(string caseId, string email, string message)
        {
            var model = new { Email = email, Message = message };
            return await HttpClient.POSTAsync<MedicalCase>($"Patients/share/{caseId}?action=accept", model);
        }

        public async Task<MedicalCase> RevokeSharingMedicalCaseAsync(string caseId, string email, string message)
        {
            var model = new { Email = email, Message = message };
            return await HttpClient.POSTAsync<MedicalCase>($"Patients/share/{caseId}?action=revoke", model);
        }

        public async Task<MedicalCase> DeleteSharingMedicalCaseAsync(string caseId, string email, string message)
        {
            var model = new { Email = email, Message = message };
            return await HttpClient.POSTAsync<MedicalCase>($"Patients/share/{caseId}?action=delete", model);
        }

        //
        //  Stop Sharing
        //
        public async Task StopSharingMedicalCaseAsync(string caseId, string email)
        {
            var model = new { Email = email, Role = "null" };
            await HttpClient.PATCHAsync($"Patients/unshare/{caseId}", model);
        }
    }
}
