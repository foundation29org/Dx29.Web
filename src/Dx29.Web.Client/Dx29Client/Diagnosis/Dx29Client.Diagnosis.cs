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
        public async Task<string> GetLastAnalysisIdAsync(string caseId)
        {
            return await HttpClient.GETAsync($"DataAnalysis/analysis/{caseId}/LastId");
        }

        public async Task<IList<DiffDisease>> GetLastAnalysisAsync(string caseId, int count)
        {
            return await HttpClient.GETAsync<IList<DiffDisease>>($"DataAnalysis/analysis/{caseId}/Last?lang={Language}&count={count}");
        }

        public async Task<IList<DiffDisease>> GetAnalysisAsync(string caseId, string resourceId)
        {
            return await HttpClient.GETAsync<IList<DiffDisease>>($"DataAnalysis/analysis/{caseId}/{resourceId}?lang={Language}");
        }

        public async Task<DiffDisease> GetAnalysisAsync(string caseId, string resourceId, string itemId)
        {
            return await HttpClient.GETAsync<DiffDisease>($"DataAnalysis/analysis/{caseId}/{resourceId}/{itemId}?lang={Language}");
        }

        public async Task<string> CreateAnalysisAsync(string caseId, IList<string> symptoms)
        {
            return await HttpClient.POSTAsync($"DataAnalysis/process/{caseId}", symptoms);
        }

        public async Task DeleteAnalysisAsync(string caseId, string resourceId)
        {
            await HttpClient.DELETEAsync($"DataAnalysis/analysis/{caseId}/{resourceId}");
        }

        public async Task<string> GetGeneReportId(string caseId, string resourceId)
        {
            try
            {
                return await HttpClient.GETAsync($"DataAnalysis/geneReport/{caseId}/{resourceId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<PhenSimilarity>> GetPhenSimilarityAsync(string caseId, string diseaseId)
        {
            try
            {
                return await HttpClient.GETAsync<List<PhenSimilarity>>($"Diagnosis/phensimilarity/{caseId}?diseaseId={diseaseId}&lang={Language}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<DiagnosisMoreInfo> GetMoreInfoAsync(string diseaseId)
        {
            try
            {
                return await HttpClient.GETAsync<DiagnosisMoreInfo>($"Diagnosis/moreinfo?diseaseId={diseaseId}&lang={Language}");
            }
            catch
            {
                return null;
            }
        }
        public async Task<DiagnosisMoreInfoList> GetMoreInfoListAsync(string diseaseId)
        {
            try
            {
                return await HttpClient.GETAsync<DiagnosisMoreInfoList>($"Diagnosis/moreinfoList?diseaseId={diseaseId}&lang={Language}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<DiagnosisMoreInfo> GetMoreInfoSelectedAsync(string text)
        {
            try
            {
                return await HttpClient.GETAsync<DiagnosisMoreInfo>($"Diagnosis/moreinfoSelected?text={text}&lang={Language}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<ClinicalTrials> GetClinicalTrialsASync(string diseaseId)
        {
            try
            {
                return await HttpClient.GETAsync<ClinicalTrials>($"Diagnosis/clinicaltrials?diseaseId={diseaseId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<PatientGroups> GetPatientGroupsASync(string diseaseId)
        {
            try
            {
                return await HttpClient.GETAsync<PatientGroups>($"Diagnosis/patientgroups?diseaseId={diseaseId}");
            }
            catch
            {
                return null;
            }
        }
    }
}
