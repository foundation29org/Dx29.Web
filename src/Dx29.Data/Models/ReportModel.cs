using System;

namespace Dx29.Web.Models
{
    public class ReportModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ErrorDesc { get; set; }

        public string ErrorCode { get; set; }
        public long Size { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string GetExtension() => NameResolver.GetExtension(Name);
        public bool IsReady() => Status.EqualsNoCase("Ready");
    }
}
