using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class JobStatus
    {
        public string Name { get; set; }
        public string Token { get; set; }

        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdate { get; set; }

        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

        public List<JobStatusLog> Logs { get; set; }
    }

    public class JobStatusLog
    {
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
