using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    public class LegacyClient
    {
        public LegacyClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        public async Task<LegacyResponse> LegacySignInAsync(string email, string password)
        {
            var body = new
            {
                email = email,
                password = password
            };
            return await HttpClient.POSTAsync<LegacyResponse>("check", body);

            // For testing
            //await Task.CompletedTask;
            //return new LegacyResponse
            //{
            //    Status = "200",
            //    UserName = email,
            //    Role = "Physician",
            //    Lang = "es-ES"
            //};
        }
    }

    public class LegacyResponse
    {
        public string Status { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Lang { get; set; }
    }
}
