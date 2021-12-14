using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Web.Models;
using Dx29.Data;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<IList<ReportModel>> GetGeneReportsAsync(string caseId)
        {
            return await HttpClient.GETAsync<IList<ReportModel>>($"GeneReports/all/{caseId}");
        }

        public async Task<List<GenotypeInfo>> GetGeneResultsAsync(string caseId, string resourceId)
        {
            return await HttpClient.GETAsync<List<GenotypeInfo>>($"GeneReports/results/{caseId}/{resourceId}");
        }

        public async Task<List<string>> GetIndividualsAsync(string caseId, FileItem ItemVCF)
        {
            return await HttpClient.POSTAsync<List<string>>($"GeneReports/individuals/{caseId}",ItemVCF);
        }

        public async Task<List<GenotypeInfo>> GetCompareGeneResultsDiseaseAsync(string caseId, string resourceId, string diseaseId)
        {
            return await HttpClient.GETAsync<List<GenotypeInfo>>($"GeneReports/compareResultsDisease/{caseId}/{resourceId}?diseaseId={diseaseId}");
        }

        public async Task<List<GenotypeInfo>> GetGeneResultsAsync(string caseId, string resourceId, List<string> selectedGenes)
        {
            return await HttpClient.GETAsync<List<GenotypeInfo>>($"GeneReports/results/{caseId}/{resourceId}?selectedGenes={string.Join(",", selectedGenes)}");
        }

        public async Task<JobStatus> ProcessGenotypeAsync(string caseId, FileItem item)
        {
            var model = new FileModel(item.Response, item.Name, item.Size);
            return await HttpClient.PUTAsync<JobStatus>($"GeneReports/process/{caseId}", model);
        }

        public async Task<JobStatus> ProcessGenotypePedAsync(string caseId, FileItem item, FileItem item2, string proband)
        {
            var model = new FileModel(item.Response, item.Name, item.Size);
            var model2 = new FileModel(item2.Response, item2.Name, item2.Size);
            var modelBody = new FileModelPed
            {
                Vcf = model,
                Ped = model2,
                Proband = proband
            };
            return await HttpClient.PUTAsync<JobStatus>($"GeneReports/process/ped/{caseId}", modelBody);
        }

        public async Task DeleteGeneReport(string caseId, string reportId)
        {
            await HttpClient.DeleteAsync($"GeneReports/delete/{caseId}/{reportId}");
        }
    }
}
