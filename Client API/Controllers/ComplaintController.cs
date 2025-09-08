using Core.DTOs.Complaints;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService complaintService;
        public ComplaintController(IComplaintService complaint)
        {
            complaintService = complaint;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComplaints()
        {
            var complaints = await complaintService.GetAllComplaintsAsync();
            return Ok(complaints);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComplaint(CreateComplaintDTO createComplaint)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var complaint = await complaintService.CreateComplaintAsync(createComplaint);
                return Ok(complaint);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
