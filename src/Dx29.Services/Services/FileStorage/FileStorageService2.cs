using System;
using System.IO;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Tools;
using Dx29.Web.Models;

namespace Dx29.Web.Services
{
    public class FileStorageService2
    {
        public FileStorageService2(FileStorageClient2 fileStorageClient, MedicalHistoryClient medicalHistoryClient)
        {
            FileStorageClient = fileStorageClient;
            MedicalHistoryClient = medicalHistoryClient;
        }

        public FileStorageClient2 FileStorageClient { get; }
        public MedicalHistoryClient MedicalHistoryClient { get; }

        //
        //  Download
        //
        public async Task<string> DownloadStringAsync(string userId, string caseId, string resourcePath)
        {
            return await FileStorageClient.DownloadStringAsync(userId, caseId, resourcePath);
        }
        public async Task<TValue> DownloadAsync<TValue>(string userId, string caseId, string resourcePath)
        {
            return await FileStorageClient.DownloadAsync<TValue>(userId, caseId, resourcePath);
        }
        public async Task<Stream> DownloadAsync(string userId, string caseId, string resourcePath)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            return await FileStorageClient.DownloadAsync(userId, caseId, resourcePath);
        }

        public async Task<(string, Stream)> DownloadResourceAsync(string userId, string caseId, ResourceGroupType groupType, string groupName, string resourceId)
        {
            var resource = await MedicalHistoryClient.GetResourceByTypeNameIdAsync(userId, caseId, groupType, groupName, resourceId);
            if (resource != null)
            {
                var filename = resource.Name;
                var name = NameResolver.ConvertFilename(filename);
                string path = GetResourcePath(groupType, groupName, resourceId, name);
                return (filename, await DownloadAsync(userId, caseId, path));
            }
            return (null, null);
        }

        private string GetResourcePath(ResourceGroupType groupType, string groupName, string resourceId, string name)
        {
            if (groupType == ResourceGroupType.Reports)
            {
                if (groupName.EqualsNoCase("Medical"))
                {
                    return $"medical-reports/{resourceId}/{name}";
                }
                if (groupName.EqualsNoCase("Genetic"))
                {
                    return $"genetic-reports/{resourceId}/{name}";
                }
            }
            return $"{resourceId}/{name}";
        }

        //
        //  Upload
        //
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, object obj)
        {
            await FileStorageClient.UploadFileAsync(userId, caseId, resourcePath, obj);
        }
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, string str)
        {
            await FileStorageClient.UploadFileAsync(userId, caseId, resourcePath, str);
        }
        public async Task UploadFileAsync(string userId, string caseId, string resourcePath, Stream stream)
        {
            await FileStorageClient.UploadFileAsync(userId, caseId, resourcePath, stream);
        }

        public async Task<FileModel> UploadTempFileAsync(string userId, string caseId, string filename, long? length, Stream stream)
        {
            var name = NameResolver.ConvertFilename(filename);
            var resourceId = IDGenerator.GenerateID('r');
            var resourcePath = $"{resourceId}/{name}";
            await UploadFileAsync(userId, caseId, resourcePath, stream);
            return new FileModel(resourceId, filename, length ?? 0);
        }

        //
        //  Delete
        //
        public async Task DeleteFileAsync(string userId, string caseId, string resourcePath)
        {
            await FileStorageClient.DeleteFileAsync(userId, caseId, resourcePath);
        }
    }
}
