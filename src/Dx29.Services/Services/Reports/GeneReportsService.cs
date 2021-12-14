using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using Dx29.Data;
using Dx29.Services;
using Dx29.Web.Models;

namespace Dx29.Web.Services
{
    public partial class GeneReportsService
    {
        public GeneReportsService(ExomiserClient exomiserClient, MedicalHistoryClient medicalHistoryClient, FileStorageClient2 fileStorageClient, AccountHashService accountHashService, SignalRService signalRService, F29BioEntityClient f29BioEntityClient, DataAnalysisService dataAnalysisService, SymptomsService symptomsService)
        {
            ExomiserClient = exomiserClient;
            MedicalHistoryClient = medicalHistoryClient;
            FileStorageClient = fileStorageClient;
            AccountHashService = accountHashService;
            SignalRService = signalRService;
            F29BioEntityClient = f29BioEntityClient;
            DataAnalysisService = dataAnalysisService;
            SymptomsService = symptomsService;
        }

        public ExomiserClient ExomiserClient { get; }
        public MedicalHistoryClient MedicalHistoryClient { get; }
        public FileStorageClient2 FileStorageClient { get; }
        public AccountHashService AccountHashService { get; }
        public SignalRService SignalRService { get; }
        public F29BioEntityClient F29BioEntityClient { get;  }

        public DataAnalysisService DataAnalysisService { get; }
        public SymptomsService SymptomsService { get; }

        public async Task<IList<ReportModel>> GetReportsAsync(string userId, string caseId)
        {
            var models = new List<ReportModel>();

            var groups = await MedicalHistoryClient.GetResourcesByTypeNameAsync<ResourceReport>(userId, caseId, ResourceGroupType.Reports, "Genetic");
            var resources = groups.SelectMany(r => r.Value);
            foreach (var resource in resources)
            {
                var model = new ReportModel
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    Status = resource.Status,
                    ErrorDesc = resource.Properties.TryGetValue("errorDetails"),
                    ErrorCode = resource.Properties.TryGetValue("errorCode"),
                    Size = resource.Properties.TryGetValue("Size").AsInt64(),
                    CreatedOn = resource.CreatedOn
                };
                models.Add(model);
            }
            return models;
        }

        public async Task<IList<GenotypeInfo>> GetReportResultAsync(string userId, string caseId, string resourceId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            // TODO: File name -> Exomiser result
            string path = $"genetic-reports/{resourceId}/exomiser.json";
            var exomiser = await FileStorageClient.DownloadAsync<ExomiserJSON>(userId, caseId, path);
            var exomiserGenes = exomiser.Where(r => r.VariantScore > 0).ToArray();
            var items = DescribeGeneTerms(exomiserGenes);
            return items;
        }

        public async Task<ExomiserJSON> GetExomiserResultAsync(string userId, string caseId, string resourceId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            string path = $"genetic-reports/{resourceId}/exomiser.json";
            ExomiserJSON exomiser = await FileStorageClient.DownloadAsync<ExomiserJSON>(userId, caseId, path);
            return exomiser;
        }

        public async Task<List<string>> GetIndividualsAsync(string userId, string caseId, FileItem ItemVCF)
        {
            string resourceId = ItemVCF.Response;
            string fileName = ItemVCF.Name;

            string vcfName = NameResolver.ConvertFilename(fileName);
            string vcfSource = $"{resourceId}/{vcfName}";

            string vcfShare = await FileStorageClient.CreateFileShareAsync(userId, "temp", vcfSource);
            return await ExomiserClient.DiscoverIndividuals(vcfName, vcfShare.Replace("%2F","/"));
        }

        public async Task<IList<GenotypeInfo>> GetFilterReportResultAsync(string userId, string caseId, string resourceIds, IList<string> selectedGenes)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            var results = new List<GenotypeInfo>();
            foreach (var resourceId in resourceIds.Split(';'))
            {
                string path = $"genetic-reports/{resourceId}/exomiser.json";
                var json = await FileStorageClient.DownloadAsync<ExomiserJSON>(userId, caseId, path);
                var json2 = SelectItems(json, selectedGenes);
                var items = DescribeGeneTerms(json2);
                foreach (var item in items)
                {
                    results.Add(item);
                }
            }
            return results;
        }

        public async Task<IList<GenotypeInfo>> GetReportCompareResultDiseaseAsync(string userId, string caseId, string resourceId, string diseaseId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }
            
            string path = $"genetic-reports/{resourceId}/exomiser.json";
            var json = await FileStorageClient.DownloadAsync<ExomiserJSON>(userId, caseId, path);
            IList<GenotypeInfo> result = await CompareItems(json, diseaseId);
               
            return result;
        }

        private ExomiserJSON SelectItems(ExomiserJSON items, IList<string> selectedGenes)
        {
            ExomiserJSON json = new ExomiserJSON();
            foreach (var item in items)
            {
                if (selectedGenes.Contains(item.GeneSymbol))
                {
                    json.Add(item);
                }
            }
            return json;
        }

        private async Task<IList<GenotypeInfo>> CompareItems(ExomiserJSON items, string diseaseId)
        {
            List<string> geneLabels = items.Select(r => r.GeneSymbol).ToList();

            List<string> diseasesIds = new List<string>();
            diseasesIds.Add(diseaseId);
            IList<DiseaseGeneContent> diseaseGenes = await F29BioEntityClient.GetGenesOfDiseaseAsync(diseasesIds.ToArray());

            IList<GenotypeInfo> result = new List<GenotypeInfo>();
            ExomiserJSON json = new ExomiserJSON();
            foreach (var gen in diseaseGenes)
            {
                if (geneLabels.Contains(gen.label))
                {
                    json.Add(items.Where(r => r.GeneSymbol == gen.label).FirstOrDefault());
                }
            }
            if (json.Count > 0)
            {
                result = DescribeGeneTerms(json);
            }            
            return result;

        }
        public async Task<JobStatus> ProcessFileAsync(string userId, string caseId, FileModel model)
        {
            return await ProcessFileAsync(userId, caseId, model.ResourceId, model.FileName, model.Size);
        }

        public async Task<JobStatus> ProcessFilePedAsync(string userId, string caseId, FileModelPed model)
        {
            FileModel vcfFileModel = model.Vcf;
            FileModel pedFileModel = model.Ped;
            string proband = model.Proband;

            var process = GenotypeProcess.FromFileModels(vcfFileModel, pedFileModel, proband);

            return await ProcessFileAsync(userId, caseId, process);
        }

        public async Task<JobStatus> ProcessFileAsync(string userId, string caseId, string resourceId, string filename, long size)
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
            await MedicalHistoryClient.UpsertResourceGroupAsync(targetUserId, targetCaseId, ResourceGroupType.Reports, "Genetic", resource);

            var jobStatus = new JobStatus();
            try
            {
                var name = NameResolver.ConvertFilename(filename);

                string path = $"genetic-reports/{resourceId}/{name}";
                string source = $"temp/{resourceId}/{name}";
                string target = $"{targetCaseId}/{path}";

                await FileStorageClient.MoveFileAsync(userId, targetUserId, source, target);
                jobStatus = await ExomiserClient.ProcessFileAsync(targetUserId, targetCaseId, resourceId, name);
                resource.Status = jobStatus.Status;
            }
            catch (Exception ex)
            {
                resource.Status = "Error";
                resource.Properties.Add("errorMessage", ex.Message);
                jobStatus.Status = "Error";
            }
            await MedicalHistoryClient.UpsertResourceGroupAsync(targetUserId, targetCaseId, ResourceGroupType.Reports, "Genetic", resource);
            return jobStatus;
        }

        public async Task<JobStatus> ProcessFileAsync(string userId, string caseId, GenotypeProcess process)
        {
            string targetUserId = userId;
            string targetCaseId = caseId;
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                targetUserId = sharedBy.UserId;
                targetCaseId = sharedBy.CaseId;
            }

            string resourceId = process.ResourceId;
            string filename = process.FileName;
            long size = process.Size;

            string pedResourceId = process.PedResourceId;
            string pedFilename = process.PedFileName;
            string proband = process.Proband;

            var resource = new ResourceReport(resourceId, filename, size) { Status = "Pending" };
            if (pedResourceId != null && pedFilename != null)
            {
                resource.Properties.Add("pedFile", pedFilename);
                resource.Properties.Add("proband", proband);
            }
            await MedicalHistoryClient.UpsertResourceGroupAsync(targetUserId, targetCaseId, ResourceGroupType.Reports, "Genetic", resource);

            var jobStatus = new JobStatus();
            try
            {
                string vcfName = NameResolver.ConvertFilename(filename);
                string vcfPath = $"genetic-reports/{resourceId}/{vcfName}";
                string vcfSource = $"temp/{resourceId}/{vcfName}";
                string vcfTarget = $"{targetCaseId}/{vcfPath}";
                await FileStorageClient.MoveFileAsync(userId, targetUserId, vcfSource, vcfTarget);

                string pedName = NameResolver.ConvertFilename(pedFilename);
                string pedPath = $"genetic-reports/{resourceId}/{pedName}";
                string pedSource = $"temp/{pedResourceId}/{pedName}";
                string pedTarget = $"{targetCaseId}/{pedPath}";
                await FileStorageClient.MoveFileAsync(userId, targetUserId, pedSource, pedTarget);

                jobStatus = await ExomiserClient.ProcessFileAsync(targetUserId, targetCaseId, resourceId, vcfName, pedName, proband);
                resource.Status = jobStatus.Status;
            }
            catch (Exception ex)
            {
                resource.Status = "Error";
                resource.Properties.Add("errorMessage", ex.Message);
                jobStatus.Status = "Error";
            }
            await MedicalHistoryClient.UpsertResourceGroupAsync(targetUserId, targetCaseId, ResourceGroupType.Reports, "Genetic", resource);
            return jobStatus;
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
            await MedicalHistoryClient.DeleteResourcesAsync(userId, caseId, ResourceGroupType.Reports, "Genetic", resourceId);

            // Delete related ResourceGroups
            var resourceGroups = await MedicalHistoryClient.GetResourceGroupsByNameAsync(userId, caseId, resourceId);
            foreach (var resourceGroup in resourceGroups)
            {
                await MedicalHistoryClient.DeleteResourceGroupAsync(userId, caseId, resourceGroup.Id);
            }
        }

        public async Task HandleNotificationAsync(ExomiserNotification notification)
        {
            // TODO: Review
            try
            {
                var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(notification.UserId, notification.CaseId);
                if (medicalCase != null)
                {
                    var resourceGroup = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync(notification.UserId, notification.CaseId, ResourceGroupType.Reports, "Genetic");
                    var resource = resourceGroup.Resources.TryGetValue(notification.ResourceId);
                    if (resource != null)
                    {
                        var status = await ExomiserClient.GetStatusAsync(notification.Token);
                        if (status.Status == "Succeeded")
                        {
                            string path = $"genetic-reports/{notification.ResourceId}/exomiser.json";
                            var json = await ExomiserClient.GetResultsAsync(notification.Token);
                            await FileStorageClient.UploadFileAsync(notification.UserId, notification.CaseId, path, json);
                            await CreateGenotypeResourceGroupAsync(notification.UserId, notification.CaseId, notification.ResourceId, json);
                            List<string> symptomsPatient = (await SymptomsService.GetSymptomsAsync(notification.UserId, notification.CaseId)).Where(r=>r.IsSelected).Select(r=>r.Id).ToList();
                            await DataAnalysisService.CreateAnalysisAsync(notification.UserId, notification.CaseId, symptomsPatient);
                            resource.Status = "Ready";
                        }
                        else
                        {
                            resource.Status = "Error";
                            resource.Properties["errorCode"] = status.ErrorCode;
                            resource.Properties["errorMessage"] = status.Message;
                            resource.Properties["errorDetails"] = status.Details;
                        }
                        await MedicalHistoryClient.UpdateResourceGroupAsync(resourceGroup, false);

                        var info = new JobNotification
                        {
                            UserId = notification.UserId,
                            CaseId = notification.CaseId,
                            ResourceId = notification.ResourceId,
                            Status = status.Status,
                            Message = status.Message
                        };
                        await SignalRService.SendUserAsync(notification.UserId, "Genotype", info);
                        // Signal shared users
                        foreach (var sharedWith in medicalCase.SharedWith)
                        {
                            var userId = AccountHashService.GetHash(sharedWith.UserId);
                            await SignalRService.SendUserAsync(userId, "Genotype", info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async Task CreateGenotypeResourceGroupAsync(string userId, string caseId, string resourceId, string json)
        {
            var exomiser = json.Deserialize<ExomiserJSON>();
            var geneResources = exomiser.Where(r => r.VariantScore > 0)
                .Select(r => new ResourceGene(
                    r.GeneSymbol,
                    r.GeneIdentifier.HgncId,
                    r.VariantScore,
                    r.PriorityResults.Values.SelectMany(r => r.AssociatedDiseases).Select(r => r.DiseaseId).ToArray()))
                .Cast<Resource>().ToList();
            await MedicalHistoryClient.CreateResourceGroupAsync(userId, caseId, ResourceGroupType.Genotype, resourceId, geneResources);
        }
    }
}
