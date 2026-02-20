using Core.DTOs.Leads.Notes;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Services.Leads;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadOpsController : ControllerBase
    {
        private readonly IleadFileService fileService;
        private readonly ILeadNoteService noteService;

        public LeadOpsController(IleadFileService fileService, ILeadNoteService noteService)
        {
            this.fileService = fileService;
            this.noteService = noteService;
        }

        [HttpGet("notes/{leadId}")]
        public async Task<IActionResult> GetNotes(int leadId)
        {
            var result = await noteService.GetAllNotes(leadId);
            return Ok(result);
        }

        [HttpPost("create-note")]
        public async Task<IActionResult> CreateNote(CreateLeadNoteDTO createNote)
        {
            var result = await noteService.CreateNote(createNote);
            return Ok(result);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportLeads(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is empty.");

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                using var stream = file.OpenReadStream();
                await fileService.ImportLeadsFromExcelAsync(stream, userId);
                return Ok(new { message = "Leads imported successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Import failed: {ex.Message}");
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportLeads()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return BadRequest("Current User Id is NULL");
            try
            {
                var fileContent = await fileService.ExportLeadsToExcelAsync(userId);
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Leads.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Export failed: {ex.Message}");
            }
        }
    }
}
