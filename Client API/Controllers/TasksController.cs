using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService taskService;
        public TasksController(ITaskService service)
        {
            taskService = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTasksAsync()
        {
            var tasks = await taskService.GetAllTasksAsync();
            return Ok(tasks);
        }
        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var task = await taskService.GetTaskByIdAsync(taskId);
            return Ok(task);
        }

        [HttpGet("emp-tasks/{empId}")]
        public async Task<IActionResult> GetTasksByEmployeeId(string empId)
        {
            var task = await taskService.GetTasksByEmployeeIdAsync(empId);
            return Ok(task);
        }

        [HttpPut("update-task/{taskId}")]
        public async Task<IActionResult> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTaskDTO)
        {
            try
            {
                await taskService.UpdateTaskAsync(taskId, updateTaskDTO);
                return Ok(new { message = "Task updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to update task", error = ex.Message });
            }
        }

        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTaskAsync(CreateTaskDTO createTaskDTO)
        {
            try
            {
                var createdTask = await taskService.CreateTaskAsync(createTaskDTO);
                return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTask.Id }, createdTask);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create task", error = ex.Message });
            }
        }

        [HttpPost("create-task-group")]
        public async Task<IActionResult> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroupDTO)
        {
            try
            {
                var createdTaskGroup = await taskService.CreateTaskGroupAsync(createTaskGroupDTO);
                return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTaskGroup.Id }, createdTaskGroup);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create task group", error = ex.Message });
            }
        }

        [HttpGet("client-service/{clientServiceId}/task-groups")]
        public async Task<IActionResult> GetTaskGroupsByClientService(int clientServiceId)
        {
            try
            {
                var taskGroups = await taskService.GetTaskGroupsByClientServiceAsync(clientServiceId);
                return Ok(taskGroups);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to get task groups", error = ex.Message });
            }
        }
    }
}
