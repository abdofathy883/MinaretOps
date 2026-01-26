using Core.DTOs.Announcements;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService announcementService;
        public AnnouncementController(IAnnouncementService announcement)
        {
            announcementService = announcement;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncementAsync(CreateAnnouncementDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newAnnouncement = await announcementService.CreateAnnouncementAsync(dto);
                return Ok(newAnnouncement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAnnouncementsAsync()
        {
            try
            {
                var announcements = await announcementService.GetAllAnnouncementsAsync();
                return Ok(announcements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await announcementService.GetById(id);
                return Ok(result);
            }
            catch(KeyNotFoundException knfEx)
            {
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncementAsync(int id)
        {
            try
            {
                var result = await announcementService.DeleteAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
