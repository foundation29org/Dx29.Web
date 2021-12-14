using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class TermSearchClient
    {
        public TermSearchClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        public async Task<IList<TermDesc>> SearchSymptomsAsync(string text, string lang, int rows)
        {
            Console.WriteLine(text);
            return await HttpClient.GETAsync<IList<TermDesc>>($"search/symptoms?q={text}&lang={lang}&rows={rows}");
        }

        public async Task<IList<TermDesc>> SearchDiseasesAsync(string text, string lang, int rows)
        {
            Console.WriteLine(text);
            return await HttpClient.GETAsync<IList<TermDesc>>($"search/diseases?q={text}&lang={lang}&rows={rows}");
        }
    }
}
