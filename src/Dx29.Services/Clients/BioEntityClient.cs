using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.Services
{
    public class BioEntityClient
    {
        public BioEntityClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public async Task<string> GetVersionAsync()
        {
            return await HttpClient.GETAsync($"About/version");
        }

        public async Task<IDictionary<string, IList<Term>>> DescribeSymptomsAsync(string[] ids, string lang)
        {
            if (ids != null && ids.Length > 0)
            {
                return await HttpClient.POSTAsync<IDictionary<string, IList<Term>>>($"Phenotype/describe?lang={lang}", ids);
            }
            else
            {
                return new Dictionary<string, IList<Term>>();
            }
        }

        public async Task<IDictionary<string, IList<Term>>> DescribeConditionsAsync(string[] ids, string lang)
        {
            if (ids != null && ids.Length > 0)
            {
                return await HttpClient.POSTAsync<IDictionary<string, IList<Term>>>($"Conditions/describe?lang={lang}", ids);
            }
            else
            {
                return new Dictionary<string, IList<Term>>();
            }
        }

        public async Task<IDictionary<string, IList<TermDesc>>> LiteDescribeTermsAsync(IList<string> ids, string lang)
        {
            if (ids != null && ids.Count > 0)
            {
                return await HttpClient.POSTAsync<IDictionary<string, IList<TermDesc>>>($"Terms/describe?lang={lang}", ids);
            }
            else
            {
                return new Dictionary<string, IList<TermDesc>>();
            }
        }

        public async Task<IDictionary<string, IList<Term>>> DescribeTermsAsync(string[] ids, string lang)
        {
            if (ids != null && ids.Length > 0)
            {
                return await HttpClient.POSTAsync<IDictionary<string, IList<Term>>>($"Terms/describe?lang={lang}", ids);
            }
            else
            {
                return new Dictionary<string, IList<Term>>();
            }
        }
    }
}
