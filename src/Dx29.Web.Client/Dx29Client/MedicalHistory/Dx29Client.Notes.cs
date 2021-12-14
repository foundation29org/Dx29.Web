using System;
using System.Net.Http;
using System.Threading.Tasks;

using Dx29.Web.Models;

namespace Dx29.Web
{
    partial class Dx29Client
    {
        public async Task<NotesModel> GetNotesAsync(string caseId)
        {
            return await HttpClient.GETAsync<NotesModel>($"Notes/all/{caseId}");
        }

        public async Task<NoteModel> GetNoteByIdAsync(string caseId, string noteId)
        {
            return await HttpClient.GETAsync<NoteModel>($"Notes/get/{caseId}/{noteId}");
        }

        public async Task CreateNoteAsync(string caseId, NoteModel note)
        {
            await HttpClient.POSTAsync($"Notes/create/{caseId}", note);
        }

        public async Task UpdateNoteAsync(string caseId, NoteModel note)
        {
            await HttpClient.PATCHAsync($"Notes/update/{caseId}", note);
        }

        public async Task DeleteNoteAsync(string caseId, string noteId)
        {
            await HttpClient.DELETEAsync($"Notes/delete/{caseId}/{noteId}");
        }
    }
}
