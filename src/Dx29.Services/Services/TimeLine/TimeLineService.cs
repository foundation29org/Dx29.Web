using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class TimeLineService
    {
        public TimeLineService(SymptomsService symptomsService, MedicalHistoryClient medicalHistoryClient, BioEntityClient bioEntityClient)
        {
            SymptomsService = symptomsService;
            MedicalHistoryClient = medicalHistoryClient;
            BioEntityClient = bioEntityClient;
        }

        public SymptomsService SymptomsService { get; }
        public MedicalHistoryClient MedicalHistoryClient { get; }
        public BioEntityClient BioEntityClient { get; }

        //
        //  Get
        //
        public async Task<SymptomTimeline> GetTimeLineAsync(string userId, string caseId, string lang = "en")
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }
            var medicalCase = await MedicalHistoryClient.GetMedicalCaseAsync(userId, caseId);
            var birthdate = medicalCase.PatientInfo.BirthDate;

            var timeline = new SymptomTimeline();

            var symptoms = await SymptomsService.GetSymptomsAsync(userId, caseId);

            var group = await MedicalHistoryClient.GetResourceGroupByTypeNameAsync(userId, caseId, ResourceGroupType.TimeLine, "Symptoms");
            if (group == null)
            {
                group = new ResourceGroup();
            }
            var resources = group.Resources.Values;

            foreach (var symptom in symptoms)
            {
                if (symptom.IsSelected)
                {
                    var timelineItem = CreateSymptomTimelineItem(symptom, resources);
                    timelineItem.BirthDate = birthdate;
                    timeline.Items.Add(timelineItem);
                }
            }

            await DescribeSymptomsAsync(timeline.Items, lang);

            return timeline;
        }

        // Update
        public async Task UpsertTimeLineAsync(string userId, string caseId, SymptomTimeline timeline)
        {
            var resources = timeline.Items.Select(r => CreateResource(r)).ToArray();
            await MedicalHistoryClient.UpsertResourceGroupAsync(userId, caseId, ResourceGroupType.TimeLine, "Symptoms", resources, replace: true);
        }

        // Helpers
        private SymptomTimelineItem CreateSymptomTimelineItem(Symptom symptom, ICollection<Resource> resources)
        {
            var resource = resources.Where(r => r.Name.EqualsNoCase(symptom.Id)).FirstOrDefault();
            var item = new SymptomTimelineItem { Id = symptom.Id };
            if (resource != null)
            {
                item.StartDate = GetDateTime(resource.Properties, "startDate");
                item.EndDate = GetDateTime(resource.Properties, "endDate");
                item.IsCurrent = GetBool(resource.Properties, "isCurrent");
                item.Notes = resource.Properties.TryGetValue("notes");
            }
            return item;
        }

        private Resource CreateResource(SymptomTimelineItem lineItem)
        {
            var resource = new Resource(Guid.NewGuid().ToString(), lineItem.Id)
            {
                Status = "Ready"
            };
            if (lineItem.StartDate != null)
            {
                resource.Properties.Add("startDate", lineItem.StartDate.Value.ToString("yyyy/MM/dd"));
            }
            if (lineItem.EndDate != null)
            {
                resource.Properties.Add("endDate", lineItem.EndDate.Value.ToString("yyyy/MM/dd"));
            }
            resource.Properties.Add("isCurrent", lineItem.IsCurrent.ToString().ToLower());
            resource.Properties.Add("notes", lineItem.Notes);
            return resource;
        }

        private bool IsSymptomSelected(IList<Symptom> symptoms, string id)
        {
            foreach (var symptom in symptoms)
            {
                if (symptom.Id.EqualsNoCase(id))
                {
                    return symptom.IsSelected;
                }
            }
            return false;
        }

        private DateTimeOffset? GetDateTime(IDictionary<string, string> properties, string name)
        {
            if (properties.TryGetValue(name, out string value))
            {
                if (DateTimeOffset.TryParse(value, out DateTimeOffset date))
                {
                    return date;
                }
            }
            return null;
        }

        private bool GetBool(IDictionary<string, string> properties, string name)
        {
            if (properties.TryGetValue(name, out string value))
            {
                if (Boolean.TryParse(value, out bool b))
                {
                    return b;
                }
            }
            return false;
        }

        private Task DescribeSymptomsAsync(IList<SymptomTimelineItem> items, string lang)
        {
            var dic = items.ToDictionary(r => r.Id);
            return DescribeSymptomsAsync(dic, lang);
        }

        private async Task DescribeSymptomsAsync(IDictionary<string, SymptomTimelineItem> lineItems, string lang)
        {
            var ids = lineItems.Keys.ToArray();
            var dic = await BioEntityClient.DescribeSymptomsAsync(ids, lang);
            foreach (var symptom in lineItems.Values)
            {
                var terms = dic[symptom.Id];
                if (terms.Count > 0)
                {
                    var term = terms.First();
                    symptom.Name = term.Name;
                    symptom.Desc = term.Desc;
                }
            }
        }
    }
}
