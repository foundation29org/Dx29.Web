using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Web.Models;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<IList<PatientModel>> GetPatientsAsync()
        {
            return await HttpClient.GETAsync<IList<PatientModel>>($"Patients");
        }

        public async Task<PatientModel> GetPatientAsync(string caseId)
        {
            try
            {
                return await HttpClient.GETAsync<PatientModel>($"Patients/{caseId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<PatientModel> CreatePatientAsync(CreatePatitentModel model)
        {
            return await HttpClient.POSTAsync<PatientModel>($"Patients", model);
        }

        public async Task<PatientModel> UpdatePatientAsync(string caseId, PatientInfoModel model)
        {
            return await HttpClient.PATCHAsync<PatientModel>($"Patients/{caseId}", model);
        }

        public async Task DeletePatientAsync(string caseId)
        {
            await HttpClient.DELETEAsync($"Patients/{caseId}");
        }

        public async Task<MedicalCaseSummary> GetCaseSummaryAsync(string caseId)
        {
            return await HttpClient.GETAsync<MedicalCaseSummary>($"Patients/summary/{caseId}");
        }
    }
}
