using System;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<SymptomTimeline> GetTimelineAsync(string caseId)
        {
            return await HttpClient.GETAsync<SymptomTimeline>($"Timeline/{caseId}?lang={Language}");
        }

        public async Task UpsertTimelineAsync(string caseId, SymptomTimeline timeline)
        {
            await HttpClient.PUTAsync($"Timeline/{caseId}", timeline);
        }
    }
}
