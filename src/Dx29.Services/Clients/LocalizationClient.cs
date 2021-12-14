using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    public class LocalizationClient
    {
        public LocalizationClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        public async Task<IDictionary<string, string>> GetLiteralsAsync(string language)
        {
            return await HttpClient.GETAsync<IDictionary<string, string>>($"Localization/literals?lang={language}");
        }

        public async Task<string> GetLiteralAsync(string language, string key)
        {
            return await HttpClient.GETAsync<string>($"Localization/literal?lang={language}");
        }

        public async Task SetLiteralsAsync(string language, IDictionary<string, string> literals)
        {
            await HttpClient.PUTAsync($"Localization/literals?lang={language}", literals);
        }

        public async Task SetLiteralAsync(string language, KeyValuePair<string, string> literal)
        {
            await HttpClient.PUTAsync<string>($"Localization/literal?lang={language}", literal);
        }

        public async Task<string> RegisterLiteralAsync(string language, KeyValuePair<string, string> literal)
        {
            return await HttpClient.PUTAsync($"Localization/register?lang={language}", literal);
        }

        public async Task DeleteLiteralAsync(string language, string key)
        {
            await HttpClient.DELETEAsync($"Localization/literal?lang={language}&key={key}");
        }
    }
}
