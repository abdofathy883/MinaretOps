using Core.DTOs.ContactForm;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactFormController : ControllerBase
    {
        private readonly IContactService contactService;

        public ContactFormController(IContactService contactService)
        {
            this.contactService = contactService;
        }

        [EnableRateLimiting("contact-limit")]
        [HttpPost]
        public async Task<IActionResult> Submit(NewEntryDTO newEntryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!string.IsNullOrEmpty(newEntryDTO.Website))
                return BadRequest("Spam Detected");

            var isHuman = await contactService.VerifyTokenAsync(newEntryDTO.RecaptchaToken);

            if (!isHuman)
                return BadRequest("Spam Detected");

            var result = await contactService.CreateContactEntry(newEntryDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await contactService.GetEntriesAsync();
            return Ok(result);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await contactService.GetByIdAsync(id);
            return Ok(result);
        }
    }
}
