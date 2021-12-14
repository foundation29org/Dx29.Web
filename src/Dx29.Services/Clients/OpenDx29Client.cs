using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class OpenDx29Client
    {
        public OpenDx29Client(HttpClient httpClient, TermSearchClient termsearchClient, BioEntityClient bioEntityClient)
        {
            HttpClient = httpClient;
            TermSearchClient = termsearchClient;
            BioEntityClient = bioEntityClient;
        }

        public HttpClient HttpClient { get; }
        public TermSearchClient TermSearchClient { get; }
        public BioEntityClient BioEntityClient { get; }

        public async Task<DiagnosisMoreInfo> GetMoreInfoAsync(string diseaseId, string lang)
        {
            var listDiseasesResponse = await TermSearchClient.SearchDiseasesAsync(diseaseId, lang, 10);
            string diseaseName = listDiseasesResponse.Where(r=>r.Id==diseaseId).FirstOrDefault()?.Name;
            if (diseaseName != null)
            {
                var body = new DiagnosisMoreInfoBody
                {
                    text = diseaseName,
                    lang = lang
                };
                return await HttpClient.POSTAsync<DiagnosisMoreInfo>($"wiki", body);
            }

            else 
            {
                List<string> diseasesIds = new List<string>();
                diseasesIds.Add(diseaseId);
                var listDiseasesResponse2 = await BioEntityClient.DescribeConditionsAsync(diseasesIds.ToArray(), lang);
                if (listDiseasesResponse2[diseaseId] != null)
                {
                    string mondoId = listDiseasesResponse2[diseaseId].FirstOrDefault().Id;
                    List<string> mondoDiseasesIds = new List<string>();
                    mondoDiseasesIds.Add(mondoId);
                    var listTerms = await BioEntityClient.DescribeTermsAsync(mondoDiseasesIds.ToArray(), lang);
                    if (listTerms[mondoId] != null)
                    {
                        diseaseName = listTerms[mondoId].Where(r => r.Id == mondoId).FirstOrDefault()?.Name;
                        if (diseaseName != null)
                        {
                            var body = new DiagnosisMoreInfoBody
                            {
                                text = diseaseName,
                                lang = lang
                            };
                            return await HttpClient.POSTAsync<DiagnosisMoreInfo>($"wiki", body);
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
                else
                {
                    return null;
                }
                
            };
        }

        public async Task<DiagnosisMoreInfo> GetMoreInfoSelectedAsync (string text, string lang)
        {
            var body = new DiagnosisMoreInfoBody
            {
                text = text,
                lang = lang
            };
            return await HttpClient.POSTAsync<DiagnosisMoreInfo>($"wiki", body);
            
        }

        public async Task<DiagnosisMoreInfoList> GetMoreInfoListAsync(string diseaseId, string lang)
        {
            var listDiseasesResponse = await TermSearchClient.SearchDiseasesAsync(diseaseId, lang, 10);
            string diseaseName = listDiseasesResponse.Where(r => r.Id == diseaseId).FirstOrDefault()?.Name;
            if (diseaseName != null)
            {
                var body = new DiagnosisMoreInfoBody
                {
                    text = diseaseName,
                    lang = lang
                };
                return await HttpClient.POSTAsync<DiagnosisMoreInfoList>($"wikiSearch", body);
            }

            else return null;
        }

        public async Task<PatientGroups> GetPatientGroupsAsync(string diseaseId)
        {
            if (!diseaseId.Contains("OMIM"))
            {
                return await HttpClient.GETAsync<PatientGroups>($"patientgroups/{diseaseId.Split(":")[1]}");
            }
            else
            {
                return new PatientGroups();
            }
        }

    }
}
