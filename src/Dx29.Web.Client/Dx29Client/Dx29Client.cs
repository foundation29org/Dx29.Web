using System;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Services;
using Dx29.Web.Services;

namespace Dx29.Web
{
    public partial class Dx29Client
    {
        public Dx29Client(LocalizationService localizationService, IHttpClientFactory httpClientFactory, ILogService logService)
        {
            logService.Info("Dx29Client ctor");
            LocalizationService = localizationService;

            HttpClient = httpClientFactory.CreateClient("Dx29.Web.API.Authenticated");
            HttpClientPublic = httpClientFactory.CreateClient("Dx29.Web.API");
        }

        public LocalizationService LocalizationService { get; }
        public HttpClient HttpClient { get; }
        public HttpClient HttpClientPublic { get; }

        public string Language => LocalizationService.Language;
        public string Culture => LocalizationService.Culture.ToString();

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync("About/version");
        }
    }
}
