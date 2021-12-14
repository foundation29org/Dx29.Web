using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dx29.Services
{
    public interface ILogService
    {
        void Info(string message, string description = null);
        Task InfoAsync(string message, string description = null, params object[] args);

        void Clear();
        Task ClearAsync();

        Task<List<LogItem>> GetLogsAsync(int take = -1);
    }

    public class LogItem
    {
        public DateTimeOffset DateTime { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
    }
}
