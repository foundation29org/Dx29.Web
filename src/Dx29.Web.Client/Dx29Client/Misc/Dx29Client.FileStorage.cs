using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Web.Models;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<byte[]> FileDownloadBytesAsync(string caseId, ResourceGroupType groupType, string groupName, string resourceId)
        {
            using (var memStream = new MemoryStream())
            {
                var stream = await HttpClient.DownloadAsync($"FileStorage/{caseId}/{groupType}/{groupName}/{resourceId}");
                await stream.CopyToAsync(memStream);
                var buffer = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(buffer, 0, (int)memStream.Length);
                return buffer;
            }
        }
    }
}
