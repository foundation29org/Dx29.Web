using System;

namespace Dx29.Web.Models
{
    public class FileInfo
    {
        public FileInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public int Index { get; set; }

        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }

        public long Loaded { get; set; }
        public long Total { get; set; }

        public int ReadyState { get; set; }
        public int Status { get; set; }
        public string Response { get; set; }

        public int Progress => (int)((Loaded / Math.Max(1.0, Total)) * 100);
    }
}
