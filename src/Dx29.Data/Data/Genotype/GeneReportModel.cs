using System;
using System.IO;
using System.Collections.Generic;

namespace Dx29.Web.Models
{
    public class GeneReportsModel : List<GeneReportModel>
    {
    }

    public class GeneReportModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ErrorDesc { get; set; }
        public long Size { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public bool IsSelected { get; set; }

        public string GetExtension() => Path.GetExtension(Name);
    }
}
