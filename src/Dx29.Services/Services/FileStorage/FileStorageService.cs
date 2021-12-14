using System;
using System.IO;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Tools;
using Dx29.Web.Models;

namespace Dx29.Web.Services
{
    public class FileStorageService
    {
        public FileStorageService(FileStorageClient fileStorageClient, MedicalHistoryClient medicalHistoryClient)
        {
            FileStorageClient = fileStorageClient;
            MedicalHistoryClient = medicalHistoryClient;
        }

        public FileStorageClient FileStorageClient { get; }
        public MedicalHistoryClient MedicalHistoryClient { get; }

        //
        //  Upload
        //
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, string name, object obj)
        {
            await FileStorageClient.UploadFileAsync(userId, caseId, resourcePath, name, obj);
        }
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, string name, string str)
        {
            await FileStorageClient.UploadFileAsync(userId, caseId, resourcePath, name, str);
        }
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, string name, Stream stream)
        {
            await FileStorageClient.UploadFileAsync(userId, caseId, resourcePath, name, stream);
        }

        public async Task<FileModel> UploadTempFileAsync(string userId, string caseId, string filename, long? length, Stream stream)
        {
            var name = NameResolver.ConvertFilename(filename);
            var resourceId = IDGenerator.GenerateID('r');
            var resourcePath = resourceId;
            await UploadFileAsync(userId, caseId, resourcePath, name, stream);
            return new FileModel(resourceId, filename, length ?? 0);
        }

        //
        //  Download
        //
        public async Task<string> DownloadStringAsync(string userId, string caseId, string resourcePath, string name)
        {
            return await FileStorageClient.DownloadStringAsync(userId, caseId, resourcePath, name);
        }
        public async Task<TValue> DownloadAsync<TValue>(string userId, string caseId, string resourcePath, string name)
        {
            return await FileStorageClient.DownloadAsync<TValue>(userId, caseId, resourcePath, name);
        }
        public async Task<Stream> DownloadAsync(string userId, string caseId, string resourcePath, string name)
        {
            return await FileStorageClient.DownloadAsync(userId, caseId, resourcePath, name);
        }

        public async Task<(string, Stream)> DownloadResourceAsync(string userId, string caseId, ResourceGroupType groupType, string groupName, string resourcePath)
        {
            var resource = await MedicalHistoryClient.GetResourceByTypeNameIdAsync(userId, caseId, groupType, groupName, resourcePath);
            if (resource != null)
            {
                var filename = resource.Name;
                var name = NameResolver.ConvertFilename(filename);
                return (filename, await DownloadAsync(userId, caseId, resourcePath, name));
            }
            return (null, null);
        }

        //
        //  Delete
        //
        public async Task DeleteFileAsync(string userId, string caseId, string resourcePath, string name)
        {
            await FileStorageClient.DeleteFileAsync(userId, caseId, resourcePath, name);
        }

        //
        //  Move
        //
        public async Task MoveFileFromTempAsync(string userId, string caseId, string resourcePath, string name)
        {
            await FileStorageClient.MoveFileFromTempAsync(userId, caseId, resourcePath, name);
        }

        //
        //  FileShare
        //
        public async Task<string> CreateFileShareAsync(string userId, string caseId, string resourcePath, string name)
        {
            return await FileStorageClient.CreateFileShareAsync(userId, caseId, resourcePath, name);
        }
    }
}
