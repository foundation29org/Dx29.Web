using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    public class FileStorageClient
    {
        public FileStorageClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        //
        //  Upload
        //
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, string name, object obj)
        {
            await HttpClient.POSTAsync($"FileStorage/file/{userId}/{caseId}/{resourcePath}/{name}", obj);
        }
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, string name, string str)
        {
            await HttpClient.POSTAsync($"FileStorage/file/{userId}/{caseId}/{resourcePath}/{name}", str);
        }
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, string name, Stream stream)
        {
            await HttpClient.POSTAsync($"FileStorage/file/{userId}/{caseId}/{resourcePath}/{name}", stream);
        }

        //
        //  Download
        //
        public async Task<string> DownloadStringAsync(string userId, string caseId, string resourcePath, string name)
        {
            return await HttpClient.GETAsync($"FileStorage/file/{userId}/{caseId}/{resourcePath}/{name}");
        }
        public async Task<TValue> DownloadAsync<TValue>(string userId, string caseId, string resourcePath, string name)
        {
            return await HttpClient.GETAsync<TValue>($"FileStorage/file/{userId}/{caseId}/{resourcePath}/{name}");
        }
        public async Task<Stream> DownloadAsync(string userId, string caseId, string resourcePath, string name)
        {
            return await HttpClient.DownloadAsync($"FileStorage/file/{userId}/{caseId}/{resourcePath}/{name}");
        }

        //
        //  Delete
        //
        public async Task DeleteFileAsync(string userId, string caseId, string resourcePath, string name)
        {
            await HttpClient.DeleteAsync($"FileStorage/file/{userId}/{caseId}/{resourcePath}/{name}");
        }

        //
        //  Move
        //
        public async Task MoveFileFromTempAsync(string userId, string caseId, string resourcePath, string name)
        {
            await HttpClient.PATCHAsync($"FileStorage/move/{userId}/temp/{caseId}/{resourcePath}/{name}", "");
        }

        //
        //  FileShare
        //
        public async Task<string> CreateFileShareAsync(string userId, string caseId, string resourcePath, string name)
        {
            return await HttpClient.GETAsync($"FileStorage/share/{userId}/{caseId}/{resourcePath}/{name}");
        }
    }
}
