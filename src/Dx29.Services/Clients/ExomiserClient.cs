using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Dx29.Data;
using System.Collections.Generic;
using System.Linq;

namespace Dx29.Web.Services
{
    public class ExomiserClient
    {
        public ExomiserClient(FileStorageClient2 fileStorageClient, HttpClient httpClient, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            FileStorageClient = fileStorageClient;
            HttpClient = httpClient;
            HttpClientFactory = httpClientFactory;
            httpClient.Timeout = TimeSpan.FromMinutes(20);
            NotificationUrl = String.Format("https://{0}/api/v1/GeneReports/Notification", configuration["Application:Host"].TrimEnd('/'));
        }

        public FileStorageClient2 FileStorageClient { get; }
        public HttpClient HttpClient { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        public string NotificationUrl { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync("About/version");
        }

        public async Task<JobStatus> ProcessFileAsync(string userId, string caseId, string resourceId, string vcfFilename, string pedFilename = null, string proband = null)
        {
            string vcfPath = $"genetic-reports/{resourceId}/{vcfFilename}";
            string vcfShare = await FileStorageClient.CreateFileShareAsync(userId, caseId, vcfPath);
            var request = CreateDefaultRequest(userId, caseId, resourceId, vcfFilename, pedFilename);
            request.GenomeAssembly = await DiscoverAssembly(vcfFilename, vcfShare);
            request.Proband = proband;
            if (pedFilename != null)
            {
                string pedPath = $"genetic-reports/{resourceId}/{pedFilename}";
                string pedShare = await FileStorageClient.CreateFileShareAsync(userId, caseId, pedPath);
                return await HttpClient.PUTAsync<JobStatus>("Exomiser/process", request, ("dx-source-vcf", vcfShare), ("dx-source-ped", pedShare));
            }
            else
            {
                return await HttpClient.PUTAsync<JobStatus>("Exomiser/process", request, ("dx-source-vcf", vcfShare));
            }
        }

        public async Task<JobStatus> GetStatusAsync(string token)
        {
            return await HttpClient.GETAsync<JobStatus>($"Exomiser/status?token={token}");
        }

        public async Task<string> GetResultsAsync(string token, string filename = "results.json")
        {
            return await HttpClient.GETAsync($"Exomiser/results?token={token}&filename={filename}");
        }

        private ExomiserRequest CreateDefaultRequest(string userId, string caseId, string resourceId, string vcfFilename, string pedFilename = null)
        {
            var json = File.ReadAllText("Assets/Exomiser/ExomiserRequest.json");
            var request = json.Deserialize<ExomiserRequest>();
            request.VcfFilename = vcfFilename;
            request.PedFilename = pedFilename;
            request.UserId = userId;
            request.CaseId = caseId;
            request.ResourceId = resourceId;
            request.NotificationUrl = NotificationUrl;
            return request;
        }

        //
        //  Discover VCF Assembly version
        //
        public async Task<string> DiscoverAssembly(string name, string url)
        {
            var http = HttpClientFactory.CreateClient();
            using (var stream = await http.GetStreamAsync(url))
            {
                if (name.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                {
                    return ParseGZStream(stream);
                }
                return ParseStream(stream);
            }
        }

        public async Task<string> DiscoverAssembly(string userId, string caseId, string path)
        {
            using (var stream = await FileStorageClient.DownloadAsync(userId, caseId, path))
            {
                if (path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                {
                    return ParseGZStream(stream);
                }
                return ParseStream(stream);
            }
        }

        private static string ParseGZStream(Stream stream)
        {
            using (var gzStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                return ParseStream(gzStream);
            }
        }

        private static string ParseStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.StartsWith("##"))
                    {
                        if (line.Contains("assembly=b37", StringComparison.OrdinalIgnoreCase))
                        {
                            return "hg19";
                        }
                        if (line.Contains("GRCh37", StringComparison.OrdinalIgnoreCase))
                        {
                            return "hg19";
                        }
                        if (line.Contains("hg19", StringComparison.OrdinalIgnoreCase))
                        {
                            return "hg19";
                        }

                        if (line.Contains("assembly=b38", StringComparison.OrdinalIgnoreCase))
                        {
                            return "hg38";
                        }
                        if (line.Contains("GRCh38", StringComparison.OrdinalIgnoreCase))
                        {
                            return "hg38";
                        }
                        if (line.Contains("hg38", StringComparison.OrdinalIgnoreCase))
                        {
                            return "hg38";
                        }
                        line = reader.ReadLine();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return null;
        }

        //
        //  Discover Individuals Assembly version
        //
        public async Task<List<string>> DiscoverIndividuals(string name, string url)
        {
            var http = HttpClientFactory.CreateClient();
            using (var stream = await http.GetStreamAsync(url))
            {
                if (name.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                {
                    return ParseIndividualsGZStream(stream);
                }
                return ParseIndividualsStream(stream);
            }
        }

        public async Task<List<string>> DiscoverIndividuals(string userId, string caseId, string path)
        {
            using (var stream = await FileStorageClient.DownloadAsync(userId, caseId, path))
            {
                if (path.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
                {
                    return ParseIndividualsGZStream(stream);
                }
                return ParseIndividualsStream(stream);
            }
        }

        private static List<string> ParseIndividualsGZStream(Stream stream)
        {
            using (var gzStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                return ParseIndividualsStream(gzStream);
            }
        }

        private static List<string> ParseIndividualsStream(Stream stream)
        {
            List<string> individuals = new List<string>();
            using (var reader = new StreamReader(stream))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if ((line.StartsWith("##")) || (line.StartsWith("#")))
                    {
                        if ((line.StartsWith("#")) && (line.StartsWith("##") == false))
                        {
                            string sep = "\t";
                            List<string> splitLineContent = line.Split(sep.ToCharArray()).ToList();
                            splitLineContent.Reverse();

                            foreach (var item in splitLineContent)
                            {
                                if (item.ToLower() != "format")
                                {
                                    individuals.Add(item);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        line = reader.ReadLine();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return individuals;
        }
    }
}
