using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

using Dx29.Data;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class FileStorageController : ControllerBase
    {
        const long SIZE_LIMIT = 2000 * 1024 * 1024; // 2 Gb

        public FileStorageController(FileStorageService2 fileStorageService, UserServices userServices)
        {
            FileStorageService = fileStorageService;
            UserServices = userServices;
        }

        public FileStorageService2 FileStorageService { get; }
        public UserServices UserServices { get; }

        [HttpPost("{caseId}")]
        [RequestSizeLimit(SIZE_LIMIT)]
        [RequestFormLimits(MultipartBodyLengthLimit = SIZE_LIMIT)]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadMultipartAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();

                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), 128);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = await reader.ReadNextSectionAsync();

                while (section != null)
                {
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                    if (hasContentDispositionHeader)
                    {
                        // This check assumes that there's a file present without form data. If form data is present, this method immediately fails and returns the model error.
                        if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                        {
                            return BadRequest("Invalid content disposition.");
                        }

                        // Don't trust the file name sent by the client. To display the file name, HTML-encode the value.
                        var trustedFileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                        var fileModel = await FileStorageService.UploadTempFileAsync(userId, caseId, trustedFileName, Request.ContentLength, section.Body);
                        return Ok(fileModel.ResourceId);
                    }
                    section = await reader.ReadNextSectionAsync();
                }
                return BadRequest("Empty content");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [RequestSizeLimit(SIZE_LIMIT)]
        [HttpPost("{caseId}/{name}")]
        public async Task<IActionResult> UploadAsync(string caseId, string name)
        {
            if (name.Length > 128) return BadRequest("Name too large.");
            if (Request.ContentLength == 0) return BadRequest("File size is zero.");
            if (Request.ContentLength > SIZE_LIMIT) return BadRequest("File size is too large.");

            try
            {
                string userId = UserServices.GetUserId();
                var stream = Request.BodyReader.AsStream();
                var fileModel = await FileStorageService.UploadTempFileAsync(userId, caseId, name, Request.ContentLength, stream);
                return Ok(fileModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{caseId}/{groupType}/{groupName}/{resourceId}")]
        public async Task<IActionResult> DownloadResourceAsync(string caseId, string groupType, string groupName, string resourceId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                (var filename, var stream) = await FileStorageService.DownloadResourceAsync(userId, caseId, Enum.Parse<ResourceGroupType>(groupType), groupName, resourceId);
                if (stream != null)
                {
                    return File(stream, "application/octet-stream", filename);
                }
                return NotFound("Resource not found.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [RequestSizeLimit(SIZE_LIMIT)]
        [HttpGet("{caseId}/{resourcePath}")]
        public async Task<IActionResult> DownloadAsync(string caseId, string resourcePath)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var stream = await FileStorageService.DownloadAsync(userId, caseId, resourcePath);
                return File(stream, "application/octet-stream", resourcePath);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{caseId}/{resourcePath}")]
        public async Task<IActionResult> DeleteAsync(string caseId, string resourcePath)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await FileStorageService.DeleteFileAsync(userId, caseId, resourcePath);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
