using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class PhenSimilarityClient
    {
        public PhenSimilarityClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        public async Task<List<PhenSimilarity>> GetPhenSimilarityAsync(List<string> listHpoReference, List<string> listHpoCompare)
        {
            Dictionary<string, List<string>> body = new Dictionary<string, List<string>>();
            body.Add("list_reference", listHpoReference);
            body.Add("list_compare", listHpoCompare);
            return await HttpClient.POSTAsync<List<PhenSimilarity>>($"calculate", body);
        }
    }
}
