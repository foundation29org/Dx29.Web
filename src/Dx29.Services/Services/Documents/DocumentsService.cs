using System;

using System.Net.Http;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    public class DocumentsService
    {
        public DocumentsService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> Download(string documentType, string documentName, string language, string version = null)
        {
            return await HttpClient.GETAsync($"Document/{documentType}/{documentName}/{language}?version={version}");
        }
    }
}
