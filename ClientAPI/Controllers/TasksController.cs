using Core.DTOs.Tasks;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
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

        [HttpGet("archived-tasks")]
        public async Task<IActionResult> GetAllArchivedTasksAsync()
        {
            try
            {
                var tasks = await taskService.GetAllArchivedTasksAsync();
                return Ok(tasks);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("task/{taskId}")]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            if (taskId == 0)
                return BadRequest("Task Id Can't Be Zero");
            try
            {
                var task = await taskService.GetTaskByIdAsync(taskId);
                return Ok(task);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("emp-tasks/{empId}")]
        public async Task<IActionResult> GetTasksByEmployeeId(string empId)
        {
            if (string.IsNullOrWhiteSpace(empId))
                return BadRequest("Employee Id is Null");
            try
            {
                var task = await taskService.GetTasksByEmployeeIdAsync(empId);
                return Ok(task);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-task/{taskId}/{empId}")]
        public async Task<IActionResult> UpdateTaskAsync(int taskId, string empId, UpdateTaskDTO updateTaskDTO)
        {
            if (taskId == 0 || updateTaskDTO is null)
                return BadRequest("Task Id is Null Or New Task Object Is Null");
            try
            {
                var result = await taskService.UpdateTaskAsync(taskId, updateTaskDTO, empId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("change-status/{taskId}/{empId}")]
        public async Task<IActionResult> ChangeTaskStatusAsync(int taskId, string empId, [FromBody] CustomTaskStatus status)
        {
            try
            {
                var result = await taskService.ChangeTaskStatusAsync(taskId, status, empId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("toggle-archive/{taskId}")]
        public async Task<IActionResult> ArchiveTaskAsync(int taskId)
        {
            if (taskId == 0)
                return BadRequest("Task Id Is Null");
            try
            {
                var result = await taskService.ToggleArchiveTaskAsync(taskId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-task/{userId}")]
        public async Task<IActionResult> CreateTaskAsync(string userId, CreateTaskDTO createTaskDTO)
        {
            if (createTaskDTO is null)
                return BadRequest("New Task Object Is Null");
            try
            {
                var createdTask = await taskService.CreateTaskAsync(userId, createTaskDTO);
                return Ok(createdTask);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-task-group/{userId}")]
        public async Task<IActionResult> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroupDTO, string userId)
        {
            if (createTaskGroupDTO is null)
                return BadRequest("Task Group Object Is Null");
            try
            {
                var createdTaskGroup = await taskService.CreateTaskGroupAsync(createTaskGroupDTO, userId);
                return Ok(createdTaskGroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("client-service/{clientServiceId}/task-groups")]
        public async Task<IActionResult> GetTaskGroupsByClientService(int clientServiceId)
        {
            if (clientServiceId == 0) 
                return BadRequest("Client Service Id Is Null");
            try
            {
                var taskGroups = await taskService.GetTaskGroupsByClientServiceAsync(clientServiceId);
                return Ok(taskGroups);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete-task/{taskId}")]
        public async Task<IActionResult> DeleteTaskAsync(int taskId)
        {
            if (taskId == 0)
                return BadRequest("Task Id Is Null");
            try
            {
                var result = await taskService.DeleteTaskAsync(taskId);
                if (result)
                    return Ok(new { message = "Task deleted successfully" });
                return NotFound("Task not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search/{query}/{currentUserId}")]
        public async Task<IActionResult> SearchTasksAsync(string query, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty");
            try
            {
                var tasks = await taskService.SearchTasks(query, currentUserId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("complete/{taskId}/{userId}")]
        public async Task<IActionResult> CompleteTaskAsync(int taskId, string userId, CreateTaskResourcesDTO createTaskResourcesDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = await taskService.CompleteTaskAsync(taskId, createTaskResourcesDTO, userId);
            return Ok(result);
        }
    }
}