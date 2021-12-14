using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using Dx29.Web.Models;
using Dx29.Web.Services;

namespace Dx29.Web.Controllers
{
    [Authorize]

    [ApiController]
    [Route("api/v1/[controller]")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NotesController : ControllerBase
    {
        public NotesController(NotesService notesService, UserServices userServices)
        {
            NotesService = notesService;
            UserServices = userServices;
        }

        public NotesService NotesService { get; }
        public UserServices UserServices { get; }

        [HttpGet("all/{caseId}")]
        public async Task<IActionResult> GetNotesAsync(string caseId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var items = await NotesService.GetNotesAsync(userId, caseId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get/{caseId}/{noteId}")]
        public async Task<IActionResult> GetNoteByIdAsync(string caseId, string noteId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                var item = await NotesService.GetNoteByIdAsync(userId, caseId, noteId);
                if (item != null)
                {
                    return Ok(item);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SALE KO");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create/{caseId}")]
        public async Task<IActionResult> CreateNoteAsync(string caseId, string noteId, [FromBody] NoteModel note)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await NotesService.CreateNoteAsync(userId, caseId, note);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update/{caseId}")]
        public async Task<IActionResult> UpdateNotesAsync(string caseId, string noteId, [FromBody] NoteModel note)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await NotesService.UpdateNoteAsync(userId, caseId, note);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{caseId}/{noteId}")]
        public async Task<IActionResult> DeleteNotesAsync(string caseId, string noteId)
        {
            try
            {
                string userId = UserServices.GetUserId();
                await NotesService.DeleteNoteAsync(userId, caseId, noteId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
