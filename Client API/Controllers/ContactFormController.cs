using Core.DTOs.ContactFormDTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        private readonly IContactFormService contactFormService;
        public ContactFormController(IContactFormService contactForm)
        {
            contactFormService = contactForm;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContactForm([FromBody] NewContactFormEntryDTO newEntry)
        {
            if (newEntry is null)
                return BadRequest("Invalid entry");
            try
            {
                var result = await contactFormService.SubmitContactFormAsync(newEntry);
                if (result)
                    return Ok("Contact form submitted successfully");
                else
                    return StatusCode(500, "Failed to submit contact form");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEntries()
        {
            try
            {
                var entries = await contactFormService.GetAllEntriesAsync();
                return Ok(entries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEntryById(int id)
        {
            try
            {
                var entry = await contactFormService.GetEntryByIdAsync(id);
                return Ok(entry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            try
            {
                var result = await contactFormService.DeleteEntryAsync(id);
                if (result)
                    return Ok("Entry deleted successfully");
                else
                    return StatusCode(500, "Failed to delete entry");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
