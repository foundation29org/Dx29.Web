using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dx29.Web.Models
{
    public class FileItem
    {
        public FileItem()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public long Size { get; set; }
        public int Progress { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }

        public bool IsReady => Status == "Ready";
        public bool IsCanceled { get; set; }
        public bool IsDeleted { get; set; }

        public void Merge(FileInfo info)
        {
            Size = info.Size;
            if (info.Status == 200)
            {
                if (info.ReadyState == 4)
                {
                    Status = "Ready";
                    Response = info.Response;
                }
                else
                {
                    Status = "Busy";
                    Progress = info.Progress;
                }
            }
            else if (info.Status >= 400)
            {
                Status = "Error";
                Error = info.Response;
            }
            else
            {
                if (info.ReadyState == 4)
                {
                    Status = "Canceled";
                }
                else
                {
                    Status = "Busy";
                    Progress = info.Progress;
                }
            }
        }

        static public FileItem FromFileInfo(FileInfo info, List<string> existingNames)
        {
            var item = new FileItem()
            {
                Id = info.Id,
                Name = GetUniqueName(existingNames, info.Name),
            };
            item.Merge(info);
            return item;
        }

        static private string GetUniqueName(List<string> existingNames, string filename)
        {
            string candidate = filename;
            int n = 1;
            while (existingNames.Contains(candidate, StringComparer.OrdinalIgnoreCase))
            {
                int index = filename.IndexOf('.');
                string left = filename.Substring(0, index);
                string right = filename.Substring(index + 1);
                candidate = $"{left} ({n++}).{right}";
            }
            existingNames.Add(candidate.ToLower());
            return candidate;
        }
    }
}
