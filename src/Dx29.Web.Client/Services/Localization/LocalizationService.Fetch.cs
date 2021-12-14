using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Dx29.Web.Services
{
    partial class LocalizationService
    {
        public async Task SetCultureAsync(string name, bool reload = false)
        {
            if (name == "debug")
            {
                Culture = new CultureInfo("en-US");
                Literals = null;
            }
            else
            {
                Culture = new CultureInfo(name);
                Literals = await LoadLiteralsAsync(Culture.Name);
            }
            Language = Culture.TwoLetterISOLanguageName;
            CultureInfo.DefaultThreadCurrentCulture = Culture;
            CultureInfo.DefaultThreadCurrentUICulture = Culture;
            CultureChanged?.Invoke(this, EventArgs.Empty);
        }

        private string TryGetLiteral(string key)
        {
            key = key ?? "";

            if (Literals == null) return key;

            if (Literals.TryGetValue(key, out string value))
            {
                return value;
            }
            RegisterLiteral(key, Culture.Name);
            return key;
        }

        private async Task<Dictionary<string, string>> LoadLiteralsAsync(string cultureName)
        {
            return await HttpClient.GETAsync<Dictionary<string, string>>($"Localization/literals?lang={cultureName}");
        }

        private async void RegisterLiteral(string key, string cultureName)
        {
            Literals[key] = await RegisterLiteralAsync(key, cultureName);
        }
        private async Task<string> RegisterLiteralAsync(string key, string cultureName)
        {
            var keyValue = new KeyValuePair<string, string>(key, null);
            return await HttpClient.PUTAsync($"Localization/register?lang={cultureName}", keyValue);
        }
    }
}
