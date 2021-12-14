using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.JSInterop;

using Dx29.Services;

namespace Dx29.Web.UI.Services
{
    public class LogService: ILogService
    {
        private bool _busy = false;

        public LogService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        public IJSRuntime JsRuntime { get; }

        public async void Info(string message, string description = null)
        {
            await InfoAsync(message, description);
        }
        public async Task InfoAsync(string message, string description = null, params object[] args)
        {
            while (_busy)
            {
                await Task.Delay(250);
            }
            _busy = true;

            description = args.Length == 0 ? description : String.Format(description ?? "", args);
            var items = await GetLogsAsync();
            items.Add(new LogItem
            {
                DateTime = DateTimeOffset.UtcNow,
                Type = "Info",
                Message = message,
                Description = description
            });
            await SetLogsAsync(items);

            _busy = false;
        }

        public async Task<List<LogItem>> GetLogsAsync(int take = -1)
        {
            try
            {
                var json = await JsRuntime.InvokeAsync<string>("localStorage.getItem", "logs");
                if (json != null)
                {
                    var items = json.Deserialize<List<LogItem>>().OrderByDescending(r => r.DateTime).ToList();
                    if (take > 0)
                    {
                        items = items.Take(take).ToList();
                    }
                    return items;
                }
            }
            catch { }
            return new List<LogItem>();
        }

        public async void Clear()
        {
            await ClearAsync();
        }
        public async Task ClearAsync()
        {
            await RemoveLogsAsync();
        }

        private async Task SetLogsAsync(List<LogItem> items)
        {
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", "logs", items.Serialize());
        }

        private async Task RemoveLogsAsync()
        {
            await JsRuntime.InvokeVoidAsync("localStorage.removeItem", "logs");
        }
    }
}
