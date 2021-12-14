using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class AnnotationsClient
    {
        public AnnotationsClient(HttpClient httpClient, IConfiguration configuration)
        {
            HttpClient = httpClient;
            Authorization = configuration["Annotations:Authorization"];
        }

        public HttpClient HttpClient { get; }
        public string Authorization { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        // Async Process
        public async Task<JobStatus> ProcessFileAsync(string userId, string caseId, string resourceId, double threshold, Stream stream)
        {
            return await HttpClient.POSTAsync<JobStatus>($"Annotations/process/{userId}/{caseId}/{resourceId}?threshold={threshold}", stream);
        }

        // Sync Process
        public async Task<IList<DocAnnotations>> SyncProcessFileAsync(string text)
        {
            var document = new { Text = text };
            return await HttpClient.POSTAsync<IList<DocAnnotations>>($"SyncAnnotations/process", document);
        }
    }
}
