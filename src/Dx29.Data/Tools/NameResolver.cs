using System;
using System.IO;

namespace Dx29.Web
{
    static public class NameResolver
    {
        static public string ConvertFilename(string filename)
        {
            string extension = GetExtension(filename);
            switch (extension)
            {
                case ".vcf":
                case ".vcf.gz":
                    return $"genotype{extension}";
                case ".ped":
                    return $"genotype.ped";
                case ".txt":
                case ".doc":
                case ".docx":
                case ".pdf":
                case ".jpg":
                case ".png":
                    return $"document{extension}";
                default:
                    return $"document.bin";
            }
        }

        static public string GetExtension(string filename)
        {
            filename = filename.ToLower();
            if (filename.EndsWith(".vcf.gz"))
            {
                return ".vcf.gz";
            }
            return Path.GetExtension(filename);
        }
    }
}
