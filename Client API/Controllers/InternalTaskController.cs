using Core.DTOs.InternalTasks;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalTaskController : ControllerBase
    {
        private readonly IInternalTaskService internalTaskService;
        public InternalTaskController(IInternalTaskService internalTask)
        {
            internalTaskService = internalTask;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInternalTasks()
        {
            try
            {
                var tasks = await internalTaskService.GetAllInternalTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInternalTaskById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid task ID.");
            }
            try
            {
                var task = await internalTaskService.GetInternalTaskById(id);
                if (task is null)
                {
                    return NotFound();
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateInternalTask(CreateInternalTaskDTO createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTask = await internalTaskService.CreateInternalTaskAsync(createTaskDto);
            return Ok(createdTask);
        }

        [HttpPatch("{internalTaskId}")]
        public async Task<IActionResult> ChangeTaskStatusAsync(int internalTaskId, [FromBody] CustomTaskStatus newStatus)
        {
            if (internalTaskId == 0)
                return BadRequest();
            try
            {
                var result = await internalTaskService.ChangeTaskStatusAsync(internalTaskId, newStatus);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
