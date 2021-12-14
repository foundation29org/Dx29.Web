using System;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<UserPreferences> GetPreferencesAsync()
        {
            return await HttpClient.GETAsync<UserPreferences>("Preferences");
        }

        public async Task<UserPreferences> SetPreferencesAsync(UserPreferences preferences)
        {
            return await HttpClient.PUTAsync<UserPreferences>("Preferences", preferences);
        }
    }
}
