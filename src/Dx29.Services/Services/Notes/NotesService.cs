using System;
using System.Linq;
using System.Threading.Tasks;

using Dx29.Data;
using Dx29.Web.Models;
using Dx29.Tools;

namespace Dx29.Web.Services
{
    public class NotesService
    {
        public NotesService(MedicalHistoryClient medicalHistoryClient, FileStorageClient2 fileStorageClient)
        {
            MedicalHistoryClient = medicalHistoryClient;
            FileStorageClient = fileStorageClient;
        }

        public MedicalHistoryClient MedicalHistoryClient { get; }
        public FileStorageClient2 FileStorageClient { get; }

        public async Task<NotesModel> GetNotesAsync(string userId, string caseId)
        {
            var models = new NotesModel();
            var groups = await MedicalHistoryClient.GetResourcesByTypeAsync(userId, caseId, ResourceGroupType.Notes);
            var resources = groups.SelectMany(r => r.Value);
            foreach (var resource in resources)
            {
                var model = new NoteModel
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    CreatedOn = resource.CreatedOn,
                    UpdatedOn = resource.UpdatedOn
                };
                models.Add(model);
            }
            return models;
        }

        public async Task<NoteModel> GetNoteByIdAsync(string userId, string caseId, string resourceId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            var resource = await MedicalHistoryClient.GetResourceByTypeNameIdAsync(userId, caseId, ResourceGroupType.Notes, "Notes", resourceId);
            string path = $"case-notes/{resourceId}/note.txt";
            var content = await FileStorageClient.DownloadStringAsync(userId, caseId, path);
            return new NoteModel
            {
                Id = resource.Id,
                Name = resource.Name,
                Content = content,
                CreatedOn = resource.CreatedOn,
                UpdatedOn = resource.UpdatedOn
            };
        }

        public async Task CreateNoteAsync(string userId, string caseId, NoteModel model)
        {
            await CreateNoteAsync(userId, caseId, model.Content);
        }
        public async Task CreateNoteAsync(string userId, string caseId, string content)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            var resourceId = IDGenerator.GenerateGuid();
            var resource = new ResourceNote(resourceId, BuildName(content)) { Status = "Ready" };
            string path = $"case-notes/{resourceId}/note.txt";
            await MedicalHistoryClient.UpsertResourceGroupAsync(userId, caseId, ResourceGroupType.Notes, "Notes", resource);
            await FileStorageClient.UploadFileAsync(userId, caseId, path, content);
        }

        public async Task UpdateNoteAsync(string userId, string caseId, NoteModel model)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            var resource = await MedicalHistoryClient.GetResourceByTypeNameIdAsync(userId, caseId, ResourceGroupType.Notes, "Notes", model.Id);
            if (resource != null)
            {
                resource.Name = BuildName(model.Content);
                string path = $"case-notes/{model.Id}/note.txt";
                await MedicalHistoryClient.UpsertResourceGroupAsync(userId, caseId, ResourceGroupType.Notes, "Notes", resource, false);
                await FileStorageClient.UploadFileAsync(userId, caseId, path, model.Content);
            }
        }

        public async Task DeleteNoteAsync(string userId, string caseId, string resourceId)
        {
            if (caseId.StartsWith("s", StringComparison.OrdinalIgnoreCase))
            {
                var sharedBy = await MedicalHistoryClient.GetSharedByAsync(userId, caseId);
                userId = sharedBy.UserId;
                caseId = sharedBy.CaseId;
            }

            string path = $"case-notes/{resourceId}/note.txt";
            await MedicalHistoryClient.DeleteResourcesAsync(userId, caseId, ResourceGroupType.Notes, "Notes", resourceId);
            await FileStorageClient.DeleteFileAsync(userId, caseId, path);
        }

        private string BuildName(string content)
        {
            string name = content ?? "";
            name = name.Substring(0, Math.Min(256, content.Length));
            if (content.Length > 256)
            {
                name = $"{name}...";
            }
            return name;
        }
    }
}
