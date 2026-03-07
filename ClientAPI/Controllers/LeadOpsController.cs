using Core.DTOs.Leads.Notes;
using Core.Interfaces;
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await noteService.CreateNote(createNote, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            try
            {
                var fileContent = await fileService.GenerateImportTemplateAsync();
                return File(fileContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "Leads Template.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Template generation failed: {ex.Message}");
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportLeads(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("File is empty.");

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                using var stream = file.OpenReadStream();
                var result = await fileService.ImportLeadsFromExcelAsync(stream, userId);
                return Ok(result);
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
