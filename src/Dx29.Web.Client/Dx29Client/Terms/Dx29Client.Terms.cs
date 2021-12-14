using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        private readonly IList<TermDesc> EmptySearchResults = new TermDesc[] { };

        public async Task<IList<TermDesc>> SearchSymptomsAsync(string text, int rows = 10)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return EmptySearchResults;
            }
            if (text.Length > 100 || text.Trim().Contains('\r') || text.Trim().Contains('\n'))
            {
                return EmptySearchResults;
            }
            return await HttpClientPublic.GETAsync<IList<TermDesc>>($"TermSearch/symptoms?text={text}&lang={Language}&rows={rows}");
        }

        public async Task<IList<TermDesc>> SearchDiseasesAsync(string text, int rows = 10)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return EmptySearchResults;
            }
            if (text.Length > 100 || text.Trim().Contains('\r') || text.Trim().Contains('\n'))
            {
                return EmptySearchResults;
            }
            return await HttpClientPublic.GETAsync<IList<TermDesc>>($"TermSearch/diseases?text={text}&lang={Language}&rows={rows}");
        }

        public async Task<IList<TermDesc>> SearchDiseasesIdAndMondoAsync(string id, int rows = 10)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return EmptySearchResults;
            }
            if (id.Length > 100 || id.Trim().Contains('\r') || id.Trim().Contains('\n'))
            {
                return EmptySearchResults;
            }
            IList<TermDesc> result = await HttpClientPublic.GETAsync<IList<TermDesc>>($"TermSearch/diseases?text={id}&lang={Language}&rows={rows}");
            if ((result?.Where(r=>r.Id==id).FirstOrDefault() != null)&&(result != null))
            {
                return result;
            }
            else
            {
                IList<TermDesc> result2 = new List<TermDesc>();
                var listMondos = (await GetMondoTermsAsync(id));
                if (listMondos[id] != null)
                {
                    var mondo = listMondos[id]?.FirstOrDefault();
                    if (mondo != null)
                    {
                        string mondoId = mondo.Id;
                        if (mondoId != null)
                        {
                            var listTerms2 = await GetTermsAsync(mondoId);
                            if (listTerms2[mondoId] != null)
                            {
                                foreach (var term in listTerms2[mondoId])
                                {
                                    TermDesc termDesc = new TermDesc
                                    {
                                        Id = id,
                                        Name = term.Name,
                                        Desc = term.Desc,
                                        Categories = term.Categories,
                                        IsSelected = true
                                    };
                                    result2.Add(termDesc);
                                }
                            }
                        }
                    }
                    
                }
                
                return result2;
            }
        }

        public async Task<IList<TermDesc>> DeepSearchSymptomsAsync(string text)
        {
            var document = new { Text = text };
            return await HttpClientPublic.POSTAsync<IList<TermDesc>>($"TermSearch/symptoms?lang={Language}", document);
        }

        public async Task<IDictionary<string, IList<Term>>> GetTermsAsync(string id)
        {
            return await HttpClientPublic.POSTAsync<IDictionary<string, IList<Term>>>($"BioEntity/terms?lang={Language}", new string[] { id });
        }

        public async Task<IDictionary<string, IList<Term>>> GetHpoTermsAsync(string id)
        {
            return await HttpClientPublic.POSTAsync<IDictionary<string, IList<Term>>>($"BioEntity/symptoms?lang={Language}", new string[] { id });
        }

        public async Task<IDictionary<string, IList<Term>>> GetMondoTermsAsync(string id)
        {
            return await HttpClientPublic.POSTAsync<IDictionary<string, IList<Term>>>($"BioEntity/conditions?lang={Language}", new string[] { id });
        }

        public async Task<IDictionary<string, IList<Term>>> GetSymptomsOfDiseaseAsync(string id)
        {
            return await HttpClientPublic.POSTAsync<IDictionary<string, IList<Term>>>($"F29BioEntity/disease/phenotypes?lang={Language}", new string[] { id });
        }

        public async Task<IList<DiseaseGeneContent>> GetGenesOfDiseaseAsync(string id)
        {
            return await HttpClientPublic.POSTAsync<IList<DiseaseGeneContent>>($"F29BioEntity/disease/gene", new string[] { id });
        }
    }
}
