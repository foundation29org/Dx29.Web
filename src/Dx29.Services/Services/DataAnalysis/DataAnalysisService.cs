using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Web.Models;
using Dx29.Tools;
using Dx29.Services;

namespace Dx29.Web.Services
{
    public class DataAnalysisService
    {
        const string GROUP_NAME = "Analysis";

        public DataAnalysisService(MedicalHistoryClient medicalHistoryClient, FileStorageClient2 fileStorageClient, BioEntityClient bioEntityClient, SignalRService signalRService, HttpClient httpClient)
        {
            MedicalHistoryClient = medicalHistoryClient;
            FileStorageClient = fileStorageClient;
            BioEntityClient = bioEntityClient;
            SignalRService = signalRService;
            HttpClient = httpClient;
        }

        public MedicalHistoryClient MedicalHistoryClient { get; }
        public FileStorageClient2 FileStorageClient { get; }
        public BioEntityClient BioEntityClient { get; }
        public SignalRService SignalRService { get; }
        public HttpClient HttpClient { get; }

        public async Task<IList<DiffDisease>> GetOrCreateAnalysisAsync(string userId, string caseId, IList<string> hpos)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            string id = null;
            var diagnostic = await GetAnalysisAsync(userId, caseId, hpos);
            if (diagnostic == null)
            {
                (id, diagnostic) = await CreateAnalysisAsync(userId, caseId, hpos);
            }
            return diagnostic;
        }

        public async Task<List<AnalysisModel>> GetAnalysisListAsync(string userId, string caseId, string lang)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            try
            {
                var resourceGroup = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync<ResourceAnalysis>(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME);
                var items = resourceGroup.Resources.Values.Where(r => r.Status.EqualsNoCase("Ready")).Select(r => AnalysisModel.CreateFromResource(r)).OrderByDescending(r => r.CreatedOn).ToList();
                await DescribeTermsAsync(items, lang);
                return items;
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new List<AnalysisModel>();
                }
                throw;
            }
        }

        public async Task<string> GetLastAnalysisIdAsync(string userId, string caseId)
        {
            var resourceGroup = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME);
            var lastAnalysis = resourceGroup.Resources.Where(r => r.Value.Status.EqualsNoCase("Ready")).OrderByDescending(r => r.Value.UpdatedOn).Select(r => r.Value).FirstOrDefault();
            return lastAnalysis?.Id;
        }

        public async Task<IList<DiffDisease>> GetLastAnalysisAsync(string userId, string caseId, string lang, int count)
        {
            var resourceGroup = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME);
            var lastAnalysis = resourceGroup.Resources.Where(r => r.Value.Status.EqualsNoCase("Ready")).OrderByDescending(r => r.Value.UpdatedOn).Select(r => r.Value).FirstOrDefault();
            if (lastAnalysis != null)
            {
                return await GetAnalysisAsync(userId, caseId, lastAnalysis.Id, lang, count);
            }
            return new List<DiffDisease>();
        }

        public async Task<IList<DiffDisease>> GetAnalysisAsync(string userId, string caseId, string resourceId, string lang, int count)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            string path = $"data-analysis/{resourceId}/data-analysis.json";
            var items = await FileStorageClient.DownloadAsync<IList<DiffDisease>>(userId, caseId, path);
            items = items.Where(r => r.Symptoms.Count > 0).OrderByDescending(r => r.ScoreDx29).Take(count).ToList();
            await DescribeTermsLiteAsync(items, lang);
            return items;
        }

        public async Task<DiffDisease> GetAnalysisAsync(string userId, string caseId, string resourceId, string itemId, string lang, int count)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            string path = $"data-analysis/{resourceId}/data-analysis.json";
            var items = await FileStorageClient.DownloadAsync<IList<DiffDisease>>(userId, caseId, path);
            var item = items.Where(r => r.Id == itemId).FirstOrDefault();
            if (item != null)
            {
                await DescribeTermsAsync(item, lang);
            }
            item = item ?? new DiffDisease();
            return item;
        }

        public async Task<IList<DiffDisease>> GetAnalysisAsync(string userId, string caseId, IList<string> hpos)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            var symptoms = String.Join(';', hpos.OrderBy(r => r)).ToLower();
            try
            {
                var resourceGroup = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME);
                var lastResource = resourceGroup.Resources.Values
                    .Where(r => MatchSymptoms(r, symptoms))
                    .OrderByDescending(r => r.UpdatedOn).FirstOrDefault();
                if (lastResource != null)
                {
                    string path = $"data-analysis/{lastResource.Id}/data-analysis.json";
                    return await FileStorageClient.DownloadAsync<IList<DiffDisease>>(userId, caseId, path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public async Task<(string, IList<DiffDisease>)> CreateAnalysisAsync(string userId, string caseId, IList<string> symptoms)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            var resourceId = IDGenerator.GenerateID('d');
            var resource = new ResourceAnalysis(resourceId);
            await MedicalHistoryClient.UpsertResourceGroupAsync(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME, resource);

            var grouped = await MedicalHistoryClient.GetResourcesByTypeAsync<ResourceGene>(userId, caseId, ResourceGroupType.Genotype);
            var genotypeIds = GetGenotypeIds(grouped.Keys).ToArray();
            var genotype = grouped.SelectMany(r => r.Value).OrderByDescending(r => r.GetScore()).Distinct(new ResourceComparer<ResourceGene>()).Select(r => DataAnalysisGene.FromResourceGene(r)).ToArray();
            var genes = genotype.Select(r => r.name).ToList();

            var analysis = await CalculateAsync(symptoms, genotype);
            var diseases = analysis.OrderByDescending(r => r.ScoreDx29).Select(r => r.Id).ToList();

            string path = $"data-analysis/{resourceId}/data-analysis.json";
            await FileStorageClient.UploadFileAsync(userId, caseId, path, analysis);

            resource.SetData(symptoms, genes, genotypeIds, diseases);
            resource.Status = "Ready";
            await MedicalHistoryClient.UpsertResourceGroupAsync(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME, resource);
            await SignalRService.SendUserAsync(userId, "DataAnalysis", "Ready");

            return (resourceId, analysis);
        }

        private static IEnumerable<string> GetGenotypeIds(IEnumerable<string> keys)
        {
            foreach (var item in keys)
            {
                var id = item.Split('.').Last();
                if (!id.EqualsNoCase("Manual"))
                {
                    yield return id;
                }
            }
        }

        public async Task DeleteAnalysisAsync(string userId, string caseId, string resourceId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            var resourceGroup = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME);
            if (resourceGroup.Resources.ContainsKey(resourceId))
            {
                resourceGroup.Resources.Remove(resourceId);
                string path = $"data-analysis/{resourceId}/data-analysis.json";
                await FileStorageClient.DeleteFileAsync(userId, caseId, path);
            }
            await MedicalHistoryClient.UpdateResourceGroupAsync(resourceGroup, true);
        }

        public async Task<IList<DiffDisease>> CalculateAsync(IList<string> hpos, IList<DataAnalysisGene> genes = null)
        {
            var body = new DataAnalysisInfo
            {
                symptoms = hpos.Select(r => r.ToUpper()).ToList(),
                genes = genes
            };
            return await HttpClient.POSTAsync<IList<DiffDisease>>("Diagnosis/calculate", body);
        }

        public async Task<IList<DataAnalysisGene>> GetGeneInputAsync(ExomiserJSON exomiserJSON)
        {
            return await HttpClient.POSTAsync<IList<DataAnalysisGene>>("getInputCalculate_fromExomiserJSON", exomiserJSON);
        }


        private async Task DescribeTermsAsync(List<AnalysisModel> items, string lang)
        {
            var ids = items.SelectMany(r => r.Symptoms.Concat(r.Diseases)).Select(r => r.Key).ToArray();
            var dic = await BioEntityClient.LiteDescribeTermsAsync(ids, lang);
            foreach (var item in items)
            {
                foreach (var symptom in item.Symptoms.Keys)
                {
                    item.Symptoms[symptom] = dic.TryGetValue(symptom)?.FirstOrDefault() ?? TermDesc.CreateUnknwon(symptom);
                }
                foreach (var disease in item.Diseases.Keys)
                {
                    item.Diseases[disease] = dic.TryGetValue(disease)?.FirstOrDefault() ?? TermDesc.CreateUnknwon(disease);
                }
            }
        }

        private async Task DescribeTermsLiteAsync(IList<DiffDisease> items, string lang)
        {
            var ids = items.Select(r => r.Id).ToArray();
            var descs = await HttpClient.POSTAsync<IDictionary<string, DiseaseDesc>>($"Diagnosis/describe?lang={lang}", ids);
            foreach (var item in items)
            {
                var desc = descs[item.Id];
                item.Name = desc.Name;
                item.Desc = desc.Desc;
            }

            var mondos = items.Where(r => !r.Id.ToLower().StartsWith("orpha")).ToArray();
            if (mondos.Length > 0)
            {
                var dic = await BioEntityClient.LiteDescribeTermsAsync(mondos.Select(r => r.Id).ToArray(), lang);
                foreach (var item in mondos)
                {
                    var desc = dic[item.Id].FirstOrDefault();
                    if (desc != null)
                    {
                        item.Name = desc.Name ?? item.Id;
                        item.Desc = desc.Desc;
                    }
                }
            }
        }

        private async Task DescribeTermsAsync(IList<DiffDisease> items, string lang)
        {
            foreach (var item in items)
            {
                await DescribeTermsAsync(item, lang);
            }
        }
        private async Task DescribeTermsAsync(DiffDisease item, string lang)
        {
            var id = item.Id;
            var descs = await HttpClient.POSTAsync<IDictionary<string, DiseaseDesc>>($"Diagnosis/describe?lang={lang}", new string[] { id });
            var desc = descs[id];
            item.Name = desc.Name;
            item.Desc = desc.Desc;

            var ids = item.Symptoms.Select(r => r.Id).ToHashSet();
            ids.Add(id);
            foreach (var course in item.Courses)
            {
                ids.Add(course.Id);
            }
            foreach (var modifier in item.Modifiers)
            {
                ids.Add(modifier.Id);
            }
            foreach (var symptom in item.Symptoms)
            {
                ids.Add(symptom.Frequency.Id);
                if (symptom.RelatedId != null)
                {
                    ids.Add(symptom.RelatedId);
                }
            }

            var dic = await BioEntityClient.LiteDescribeTermsAsync(ids.ToArray(), lang);
            if (String.IsNullOrEmpty(item.Name) || !item.Id.ToLower().StartsWith("orpha"))
            {
                item.Name = dic[id].FirstOrDefault()?.Name ?? id;
                item.Desc = dic[id].FirstOrDefault()?.Desc;
            }

            foreach (var symptom in item.Symptoms)
            {
                symptom.Name = dic[symptom.Id].FirstOrDefault()?.Name ?? symptom.Id;
                symptom.Desc = dic[symptom.Id].FirstOrDefault()?.Desc ?? "";
                symptom.Categories = dic[symptom.Id].FirstOrDefault()?.Categories;
            }
            foreach (var course in item.Courses)
            {
                course.Name = dic[course.Id].FirstOrDefault()?.Name ?? course.Id;
                course.Desc = dic[course.Id].FirstOrDefault()?.Desc ?? "";
            }
            foreach (var modifier in item.Modifiers)
            {
                modifier.Name = dic[modifier.Id].FirstOrDefault()?.Name ?? modifier.Id;
                modifier.Desc = dic[modifier.Id].FirstOrDefault()?.Desc ?? "";
            }
            foreach (var symptom in item.Symptoms)
            {
                symptom.Frequency.Name = dic[symptom.Frequency.Id].FirstOrDefault()?.Name;
                symptom.Frequency.Desc = dic[symptom.Frequency.Id].FirstOrDefault()?.Desc ?? "";
                if (symptom.RelatedId != null)
                {
                    symptom.RelatedName = dic[symptom.RelatedId].FirstOrDefault()?.Name ?? symptom.RelatedId;
                    symptom.RelatedDesc = dic[symptom.RelatedId].FirstOrDefault()?.Desc ?? "";
                }
            }
        }

        private bool MatchSymptoms(Resource resource, string symptoms)
        {
            var hpos = resource.Properties.TryGetValue("symptoms");
            if (hpos != null)
            {
                var parts = hpos.Split(';');
                hpos = String.Join(';', parts.OrderBy(r => r)).ToLower();
                return String.Equals(hpos, symptoms);
            }
            return false;
        }

        public async Task<string> GetGeneReportIdAsync(string userId, string caseId, string resourceId)
        {
            try
            {
                var resourceGroup = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync<ResourceAnalysis>(userId, caseId, ResourceGroupType.Analysis, GROUP_NAME);
                if (resourceGroup != null)
                {
                    var item = resourceGroup.Resources.Values.Where(r => r.Id == resourceId).FirstOrDefault();
                    if (item != null)
                    {
                        return item.Properties.TryGetValue("GenotypeIds");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    throw;
                }
            }
            return null;
        }
    }
}
