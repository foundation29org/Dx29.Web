using System;

namespace Dx29.Data
{
    public class JobNotification
    {
        public string UserId { get; set; }
        public string CaseId { get; set; }
        public string ResourceId { get; set; }

        public string Status { get; set; }
        public string Message { get; set; }
    }
}
