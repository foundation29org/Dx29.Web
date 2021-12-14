using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Web.Models;
using Dx29.Tools;
using Dx29.Services;
using System.Text.RegularExpressions;

namespace Dx29.Web.Services
{
    public class DiagnosisService
    {

        public DiagnosisService(HttpClient httpClient, SymptomsService symptomsService, MedicalHistoryClient medicalHistoryClient, BioEntityClient bioEntityClient, TermSearchClient termSearchClient, F29BioEntityClient F29bioEntityClient, PhenSimilarityClient phenSimilarityClient, OpenDx29Client openDx29Client)
        {
            SymptomsService = symptomsService;
            MedicalHistoryClient = medicalHistoryClient;
            F29BioEntityClient = F29bioEntityClient;
            PhenSimilarityClient = phenSimilarityClient;
            BioEntityClient = bioEntityClient;
            HttpClient = httpClient;
            OpenDx29Client = openDx29Client;
            TermSearchClient = termSearchClient;
        }

        public MedicalHistoryClient MedicalHistoryClient { get; }

        public SymptomsService SymptomsService { get; }
        public F29BioEntityClient F29BioEntityClient { get; }
        public BioEntityClient BioEntityClient { get; }
        public TermSearchClient TermSearchClient { get; }
        public PhenSimilarityClient PhenSimilarityClient { get; }

        public OpenDx29Client OpenDx29Client { get; }
        public HttpClient HttpClient { get; }

        public async Task<List<PhenSimilarity>> GetPhenSimilarityAsync(string userId, string caseId, string lang, string diseaseId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }
            
            // Get symptoms of the user
            var listSymptomsPatientRequest = await SymptomsService.GetSymptomsAsync(userId, caseId, null);
            List<string> listSymptomsPatient = listSymptomsPatientRequest.Where(r => r.IsSelected).Select(r => r.Id.ToUpper()).ToList();

            
            // Get symptoms of the disease
            List<string> diseasesIds = new List<string>();
            diseasesIds.Add(diseaseId);
            var listSymptomsDiseaseResponse = await F29BioEntityClient.GetSymptomsOfDiseaseAsync(diseasesIds.ToArray(), lang);

            if (listSymptomsPatient.Count > 0)
            {
                if (listSymptomsDiseaseResponse.ContainsKey(diseaseId))
                {
                    List<string> listSymptomsDisease = listSymptomsDiseaseResponse[diseaseId].Select(r => r.Id.ToUpper()).ToList();
                    // Compare with phensimilarity
                    var result = await PhenSimilarityClient.GetPhenSimilarityAsync(listSymptomsPatient, listSymptomsDisease);
                    var resultDesc = await DescribeSymptomsAsync(result, lang);
                    return resultDesc;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (listSymptomsDiseaseResponse.ContainsKey(diseaseId))
                {
                    List<string> listSymptomsDisease = listSymptomsDiseaseResponse[diseaseId].Select(r => r.Id.ToUpper()).ToList();
                    if (listSymptomsDisease.Count > 0)
                    {
                        // El paciente no tiene sintomas - solo los de la disease 
                        List<PhenSimilarity> result = new List<PhenSimilarity>();
                        foreach(var symptomId in listSymptomsDisease)
                        {
                            result.Add(new PhenSimilarity
                            {
                                Id = symptomId,
                                Name = null,
                                Desc = null,
                                HasPatient = false,
                                HasDisease = true,
                                Relationship = null,
                                Score = 0.0,
                                RelatedId = null,
                                RelatedDesc = null,
                            });
                        }
                        var resultDesc = await DescribeSymptomsAsync(result, lang);
                        return resultDesc;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            
        }

        public async Task<DiagnosisMoreInfo> GetMoreInfoAsync(string diseaseId, string lang)
        {
            var response = await OpenDx29Client.GetMoreInfoAsync(diseaseId, lang);
            return TransformDiagnosisMoreInfo(response);
        }

        public async Task<DiagnosisMoreInfo> GetMoreInfoSelectedAsync(string text, string lang)
        {
            var response = await OpenDx29Client.GetMoreInfoSelectedAsync(text, lang);
            return TransformDiagnosisMoreInfo(response);
        }

        public async Task<DiagnosisMoreInfoList> GetMoreInfoListAsync(string diseaseId, string lang)
        {
            return await OpenDx29Client.GetMoreInfoListAsync(diseaseId, lang);
        }

        public async Task<ClinicalTrials> GetClinicalTrialsAsync(string diseaseId)
        {
            List<string> listIds = new List<string>();
            listIds.Add(diseaseId);
            var terms = await TermSearchClient.SearchDiseasesAsync(diseaseId,"en",10);
            string diseaseName = terms.FirstOrDefault()?.Name;
            if (diseaseName != null)
            {
                var result = await HttpClient.GETAsync<ClinicalTrials>("https://clinicaltrials.gov/api/query/full_studies?expr=" + diseaseName + "&fmt=json&max_rnk=50");
                var resultValidated = ValidateCLinicalTrials(result);
                return resultValidated;
            }
            else return null;
        }


        public async Task<PatientGroups> GetPatientGroupsAsync(string diseaseId)
        {
            return await OpenDx29Client.GetPatientGroupsAsync(diseaseId);
        }

        private async Task<List<PhenSimilarity>> DescribeSymptomsAsync(List<PhenSimilarity> symptomsList, string lang)
        {
            var ids = symptomsList.Select(r=>r.Id).ToArray();
            var dic = await BioEntityClient.DescribeSymptomsAsync(ids, lang);
            foreach (var symptom in symptomsList)
            {
                var terms = dic[symptom.Id];
                if (terms.Count > 0)
                {
                    var term = terms.First();
                    symptom.Name = term.Name;
                    symptom.Desc = term.Desc;
                    symptom.Categories = term.Categories;
                }
                if (symptom.RelatedId != null)
                {
                    List<string> relatedIdsList = new List<string>();
                    relatedIdsList.Add(symptom.RelatedId);
                    var dic2 = await BioEntityClient.DescribeSymptomsAsync(relatedIdsList.ToArray(), lang);
                    var term2 = dic2[symptom.RelatedId].First();
                    symptom.RelatedName = term2.Name;
                    symptom.RelatedDesc = term2.Desc;
                }
            }
            return symptomsList;
        }

        private DiagnosisMoreInfo TransformDiagnosisMoreInfo(DiagnosisMoreInfo diagnosisMoreInfo)
        {
            foreach (var diagnosis in diagnosisMoreInfo)
            {
                if (diagnosis.title == "Enlaces externos" || diagnosis.title == "External links")
                {
                    var urls = diagnosis.content.Split("\n");
                    diagnosis.urls = urls.ToList();
                }
                else
                {
                    diagnosis.content = Regex.Replace(diagnosis.content, @"(\[\d+)\]", "");
                    if (diagnosis.items.Count > 0)
                    {
                        foreach(var item in diagnosis.items)
                        {
                            item.content = Regex.Replace(item.content, @"(\[\d+)\]", "");
                        }
                    }
                }
            }
            return diagnosisMoreInfo;
        }

        private ClinicalTrials ValidateCLinicalTrials(ClinicalTrials clinicalTrials)
        {
            ClinicalTrials result = new ClinicalTrials();
            result.FullStudiesResponse = new ClinicalTrialsInfo();

            if (clinicalTrials.FullStudiesResponse.FullStudies.Count > 0)
            {
                foreach (var fullstudy in clinicalTrials.FullStudiesResponse.FullStudies)
                {
                    if ((fullstudy.Study.ProtocolSection.StatusModule.OverallStatus == "Available") || (fullstudy.Study.ProtocolSection.StatusModule.OverallStatus == "Recruiting"))
                    {
                        // Delete country duplicates
                        if(fullstudy.Study.ProtocolSection.ContactsLocationsModule != null)
                        {
                            if (fullstudy.Study.ProtocolSection.ContactsLocationsModule.LocationList?.Location.Count > 0)
                            {
                                fullstudy.Study.ProtocolSection.ContactsLocationsModule.LocationList.Location = fullstudy.Study.ProtocolSection.ContactsLocationsModule.LocationList?.Location.GroupBy(x => x.LocationCountry).Select(g => g.First()).ToList();
                            }                        
                        }
                        result.FullStudiesResponse.FullStudies.Add(fullstudy);
                    }
                }
            }

            return result;
        }
    
    }

   
}
