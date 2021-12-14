using System;
using System.IO;

namespace Dx29.Web.Models
{
    public class FileModel
    {
        private string _extension = null;

        public FileModel()
        {
        }
        public FileModel(string resourceId, string fileName, long size)
        {
            ResourceId = resourceId;
            FileName = fileName;
            Size = size;
            Threshold = 1.0;
        }

        public string ResourceId { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }

        public double Threshold { get; set; }

        public string Extension => _extension ?? (_extension = GetExtension());

        public string FileType => GetFileType(Extension);
        public bool IsPhenotype => FileType == "Medical";
        public bool IsGenotype => FileType == "Genetic";

        private string GetFileType(string extension)
        {
            if (Extension == ".vcf" || Extension == ".vcf.gz")
            {
                return "Genetic";
            }
            if (Extension == ".ped")
            {
                return "Ped";
            }
            return "Medical";
        }

        private string GetExtension()
        {
            if (FileName.ToLower().EndsWith(".vcf.gz"))
            {
                return ".vcf.gz";
            }
            return Path.GetExtension(FileName).ToLower();
        }
    }
}
