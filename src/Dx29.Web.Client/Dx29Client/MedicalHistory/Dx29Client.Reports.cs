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
        public async Task<IList<ReportModel>> GetReportsAsync(string caseId)
        {
            return await HttpClient.GETAsync<IList<ReportModel>>($"PhenReports/all/{caseId}");
        }

        public async Task<IList<DocAnnotations>> GetAnnotationsAsync(string caseId, string reportId)
        {
            return await HttpClient.GETAsync<IList<DocAnnotations>>($"PhenReports/annotations/{caseId}/{reportId}");
        }

        public async Task<JobStatus> ProcessFileAsync(string caseId, FileItem item)
        {
            var model = new FileModel(item.Response, item.Name, item.Size);
            return await HttpClient.PUTAsync<JobStatus>($"PhenReports/process/{caseId}", model);
        }

        public async Task DeleteReportAsync(string caseId, string reportId)
        {
            await HttpClient.DeleteAsync($"PhenReports/delete/{caseId}/{reportId}");
        }
    }
}
