using System;

using Dx29.Web.Models;

namespace Dx29.Data
{
    public class GenotypeProcess
    {
        public string ResourceId { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }

        public string PedResourceId { get; set; }
        public string PedFileName { get; set; }
        public long PedSize { get; set; }

        public string Proband { get; set; }

        public static GenotypeProcess FromFileModels(FileModel vcf, FileModel ped, string proband)
        {
            return new GenotypeProcess
            {
                ResourceId = vcf.ResourceId,
                FileName = vcf.FileName,
                Size = vcf.Size,
                PedResourceId = ped.ResourceId,
                PedFileName = ped.FileName,
                PedSize = ped.Size,
                Proband = proband
            };
        }
    }
}
