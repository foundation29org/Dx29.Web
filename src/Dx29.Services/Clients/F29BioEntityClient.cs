using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class F29BioEntityClient
    {
        public F29BioEntityClient(HttpClient httpClient, BioEntityClient bioentityclient)
        {
            HttpClient = httpClient;
            BioEntityClient = bioentityclient;
        }

        public HttpClient HttpClient { get; }

        public BioEntityClient BioEntityClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        public async Task<IDictionary<string, IList<Term>>> GetSymptomsOfDiseaseAsync(string[] ids, string lang)
        {
            if (ids != null && ids.Length > 0)
            {
                var diseaseTerms = await HttpClient.POSTAsync<IDictionary<string, IList<TermDesc>>>($"Terms/symptoms", ids);
                var diseaseSymptoms = await DescribeTerms(diseaseTerms, lang);
                foreach (var diseaseId in diseaseTerms.Keys)
                {
                    var disease = diseaseTerms[diseaseId];
                    var diseaseSymp = diseaseSymptoms.TryGetValue(diseaseId);
                    if (diseaseSymp != null)
                    {
                        foreach (var term in diseaseSymp)
                        {
                            term.Frequency = "HP:9999999";
                            try
                            {
                                var symp = disease.Where(r => r.Id.EqualsNoCase(term.Id)).FirstOrDefault();
                                if (symp != null)
                                {
                                    term.Frequency = symp.Frequency;
                                }
                            }
                            catch { }
                        }
                    }
                }
                return diseaseSymptoms;
            }
            else
            {
                return new Dictionary<string, IList<Term>>();
            }
        }

        public async Task<IList<DiseaseGeneContent>> GetGenesOfDiseaseAsync(string[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                var response = await HttpClient.POSTAsync<IDictionary<string, IList<GeneDesc>>>($"Terms/genes/", ids);
                return DescribeGene(response);
            }
            else
            {
                return new List<DiseaseGeneContent>();
            }
        }

        private async Task<IDictionary<string, IList<Term>>> DescribeTerms(IDictionary<string, IList<TermDesc>> dictionary, string lang)
        {
            IDictionary<string, IList<Term>> result = new Dictionary<string, IList<Term>>();
            foreach (var disease in dictionary)
            {
                var diseaseId = disease.Key;
                var phenotypes = dictionary[diseaseId];
                List<string> listTermsIds = new List<string>();
                foreach (var symptom in phenotypes)
                {
                    var id = symptom.Id;
                    listTermsIds.Add(id);
                }
                string[] listTermsIdsStr = listTermsIds.ToArray();
                var termDesc = await BioEntityClient.DescribeTermsAsync(listTermsIdsStr, lang);

                IList<Term> resultListTermDesc = new List<Term>();
                foreach (var symptom in termDesc)
                {
                    var symptomId = symptom.Key;
                    foreach (var item in termDesc[symptomId])
                    {
                        resultListTermDesc.Add(item);
                    }
                }
                result.Add(diseaseId, resultListTermDesc);
            }
            return result;
        }

        private IList<DiseaseGeneContent> DescribeGene(IDictionary<string, IList<GeneDesc>> diseaseGeneInfo)
        {
            var result = new List<DiseaseGeneContent>();
            foreach (var genes in diseaseGeneInfo.Values)
            {
                foreach (var gene in genes)
                {
                    result.Add(new DiseaseGeneContent { Id = gene.Id, label = gene.Label });
                }
            }
            return result;
        }
    }
}
