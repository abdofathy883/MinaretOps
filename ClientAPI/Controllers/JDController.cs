using Core.DTOs.JDs;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JDController : ControllerBase
    {
        private readonly IJobDescribtionService jobDescribtionService;

        public JDController(IJobDescribtionService jobDescribtionService)
        {
            this.jobDescribtionService = jobDescribtionService;
        }

        [HttpGet("jds")]
        public async Task<IActionResult> GetAllJDsAsync()
        {
            var result = await jobDescribtionService.GetAllJDsAsync();
            return Ok(result);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var result = await jobDescribtionService.GetAllRolesAsync();
            return Ok(result);
        }

        [HttpGet("{jdId}")]
        public async Task<IActionResult> GetJDByIdAsync(int jdId)
        {
            var result = await jobDescribtionService.GetJDById(jdId);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateJDAsync(CreateJDDTO createJDDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await jobDescribtionService.CreateJDAsync(createJDDTO);
            return Ok(result);
        }
    }
}
