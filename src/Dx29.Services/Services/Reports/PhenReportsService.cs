using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Web.Models;

namespace Dx29.Web.Services
{
    public class PhenReportsService
    {
        public PhenReportsService(AnnotationsClient annotationsClient, MedicalHistoryClient medicalHistoryClient, FileStorageClient2 fileStorageClient)
        {
            MedicalHistoryClient = medicalHistoryClient;
            FileStorageClient = fileStorageClient;
            AnnotationsClient = annotationsClient;
        }

        public AnnotationsClient AnnotationsClient { get; }
        public MedicalHistoryClient MedicalHistoryClient { get; }
        public FileStorageClient2 FileStorageClient { get; }

        public async Task<IList<ReportModel>> GetReportsAsync(string userId, string caseId)
        {
            var models = new List<ReportModel>();

            var groups = await MedicalHistoryClient.GetResourcesByTypeNameAsync<ResourceReport>(userId, caseId, ResourceGroupType.Reports, "Medical");
            var resources = groups.SelectMany(r => r.Value);
            foreach (var resource in resources)
            {
                var model = new ReportModel
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    Status = resource.Status,
                    ErrorDesc = resource.Properties.TryGetValue("errorMessage"),
                    Size = resource.Properties.TryGetValue("Size").AsInt64(),
                    CreatedOn = resource.CreatedOn
                };
                models.Add(model);
            }
            return models;
        }

        public async Task<IList<DocAnnotations>> ProcessTextAsync(string text)
        {
            return await AnnotationsClient.SyncProcessFileAsync(text);
        }

        public async Task<JobStatus> ProcessFileAsync(string userId, string caseId, FileModel model)
        {
            return await ProcessFileAsync(userId, caseId, model.ResourceId, model.FileName, model.Size, model.Threshold);
        }
        public async Task<JobStatus> ProcessFileAsync(string userId, string caseId, string resourceId, string filename, long size, double threshold)
        {
            string targetUserId = userId;
            string targetCaseId = caseId;
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                targetUserId = sharedBy.UserId;
                targetCaseId = sharedBy.CaseId;
            }

            var resource = new ResourceReport(resourceId, filename, size) { Status = "Pending" };
            await MedicalHistoryClient.UpsertResourceGroupAsync(targetUserId, targetCaseId, ResourceGroupType.Reports, "Medical", resource);

            var jobStatus = new JobStatus();
            try
            {
                var name = NameResolver.ConvertFilename(filename);
                string path = $"medical-reports/{resourceId}/{name}";
                string source = $"temp/{resourceId}/{name}";
                string target = $"{targetCaseId}/{path}";

                await FileStorageClient.MoveFileAsync(userId, targetUserId, source, target);
                using (var stream = await FileStorageClient.DownloadAsync(targetUserId, targetCaseId, path))
                {
                    jobStatus = await AnnotationsClient.ProcessFileAsync(targetUserId, targetCaseId, resourceId, threshold, stream);
                    resource.Status = jobStatus.Status;
                }
            }
            catch (Exception ex)
            {
                resource.Status = "Error";
                resource.Properties.Add("errorMessage", ex.Message);
                jobStatus.Status = "Error";
            }
            await MedicalHistoryClient.UpsertResourceGroupAsync(targetUserId, targetCaseId, ResourceGroupType.Reports, "Medical", resource);
            return jobStatus;
        }

        public async Task<IList<DocAnnotations>> GetAnnotationsAsync(string userId, string caseId, string resourceId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            string path = $"medical-reports/{resourceId}/annotations.json";
            return await FileStorageClient.DownloadAsync<IList<DocAnnotations>>(userId, caseId, path);
        }

        public async Task DeleteReportAsync(string userId, string caseId, string resourceId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            // Delete from Reports
            await MedicalHistoryClient.DeleteResourcesAsync(userId, caseId, ResourceGroupType.Reports, "Medical", resourceId);

            // Delete related ResourceGroups
            var resourceGroups = await MedicalHistoryClient.GetResourceGroupsByNameAsync(userId, caseId, resourceId);
            foreach (var resourceGroup in resourceGroups)
            {
                await MedicalHistoryClient.DeleteResourceGroupAsync(userId, caseId, resourceGroup.Id);
            }
        }
    }
}
