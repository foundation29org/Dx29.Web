using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class SymptomsService
    {
        public SymptomsService(MedicalHistoryClient medicalHistoryClient, BioEntityClient bioEntityClient)
        {
            MedicalHistoryClient = medicalHistoryClient;
            BioEntityClient = bioEntityClient;
        }

        public MedicalHistoryClient MedicalHistoryClient { get; }
        public BioEntityClient BioEntityClient { get; }

        //
        //  Get
        //
        public async Task<IList<Symptom>> GetSymptomsAsync(string userId, string caseId, string groupName = null)
        {
            var symptoms = await GetSymptomsAsync<Symptom>(userId, caseId, groupName = null);
            return symptoms.Values.ToList();
        }

        public async Task<IList<SymptomDesc>> GetSymptomsDescAsync(string userId, string caseId, string groupName, string lang)
        {
            var symptoms = await GetSymptomsAsync<SymptomDesc>(userId, caseId, groupName);
            await DescribeSymptomsAsync(symptoms, lang);
            return symptoms.Values.ToList();
        }

        private async Task<IDictionary<string, TSymptom>> GetSymptomsAsync<TSymptom>(string userId, string caseId, string groupName = null) where TSymptom : Symptom, new()
        {
            var symptomStatuses = await GetSymptomsStatusesAsync(userId, caseId);
            var symptoms = new Dictionary<string, TSymptom>(StringComparer.OrdinalIgnoreCase);
            var groups = await MedicalHistoryClient.GetResourcesAsync(userId, caseId, ResourceGroupType.Phenotype, groupName);
            foreach (var group in groups)
            {
                var parts = group.Key.Split('.');
                string groupId = parts.First();
                groupName = parts.LastOrDefault();
                foreach (var resource in group.Value)
                {
                    if (!symptoms.ContainsKey(resource.Id))
                    {
                        symptoms.Add(resource.Id, new TSymptom { Id = resource.Id, Status = symptomStatuses[resource.Id], LastUpdate = resource.UpdatedOn });
                    }
                    var symptom = symptoms[resource.Id];
                    symptom.MergeLastUpdate(resource.UpdatedOn);
                    symptom.Sources.Add(new SymptomSource(groupId, groupName, resource.Properties.TryGetValue("Segments")));
                }
            }
            return symptoms;
        }

        private async Task<IDictionary<string, string>> GetSymptomsStatusesAsync(string userId, string caseId)
        {
            var symptomStatuses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
            var symptomGroups = medicalCase.ResourceGroups.Select(r => r.Value).Where(r => r.Type == ResourceGroupType.Phenotype.ToString()).ToArray();
            var generalSymptoms = symptomGroups.Where(r => !r.Name.EqualsNoCase("Manual")).SelectMany(r => r.Resources);
            foreach (var symptom in generalSymptoms)
            {
                if (!symptomStatuses.ContainsKey(symptom.Key))
                {
                    symptomStatuses.Add(symptom.Key, symptom.Value);
                }
                symptomStatuses[symptom.Key] = Symptom.MergeStatus(symptomStatuses[symptom.Key], symptom.Value);
            }
            var manualSymptoms = symptomGroups.Where(r => r.Name.EqualsNoCase("Manual")).SelectMany(r => r.Resources);
            foreach (var symptom in manualSymptoms)
            {
                symptomStatuses[symptom.Key] = symptom.Value;
            }
            return symptomStatuses;
        }

        //
        //  Update
        //
        public async Task<IDictionary<string, IList<Resource>>> UpsertSymptomsAsync(string userId, string caseId, IDictionary<string, string> symptomStatuses)
        {
            var date = DateTimeOffset.UtcNow;
            var grouped = await MedicalHistoryClient.GetResourcesByTypeAsync(userId, caseId, ResourceGroupType.Phenotype);
            var manualGroup = grouped.Where(r => r.Key.EndsWith(".Manual")).First().Value;

            foreach (var symptomStatus in symptomStatuses)
            {
                var status = symptomStatus.Value;
                var matches = grouped.SelectMany(r => r.Value).Where(r => r.Id.EqualsNoCase(symptomStatus.Key)).ToArray();
                if (matches.Length > 0)
                {
                    foreach (var item in matches)
                    {
                        if (!item.Status.EqualsNoCase(status))
                        {
                            item.Status = status;
                            item.UpdatedOn = date;
                        }
                    }
                }
                else
                {
                    manualGroup.Add(new ResourceSymptom(symptomStatus.Key) { Status = status, CreatedOn = date, UpdatedOn = date });
                }
            }

            // Send only updated symptoms
            foreach (var group in grouped.Values)
            {
                foreach (var symptom in group.Where(r => r.UpdatedOn != date).ToArray())
                {
                    group.Remove(symptom);
                }
            }
            return await MedicalHistoryClient.UpsertResourcesAsync(userId, caseId, grouped);
        }


        #region Describe Symptoms
        private async Task DescribeSymptomsAsync(IDictionary<string, SymptomDesc> symptoms, string lang)
        {
            var ids = symptoms.Keys.ToArray();
            var dic = await BioEntityClient.DescribeSymptomsAsync(ids, lang);
            foreach (var symptom in symptoms.Values)
            {
                var terms = dic[symptom.Id];
                if (terms.Count > 0)
                {
                    var term = terms.First();
                    symptom.Name = term.Name;
                    symptom.Desc = term.Desc;
                    symptom.Categories = term.Categories;
                }
            }
        }
        #endregion
    }
}
