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
        private readonly IArchivedTaskService archivedTaskService;
        public TasksController(ITaskService service, IArchivedTaskService archivedTaskService)
        {
            taskService = service;
            this.archivedTaskService = archivedTaskService;
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

        [HttpPut("archive-task/{taskId}")]
        public async Task<IActionResult> ArchiveTask(int taskId)
        {
            if (taskId == 0)
                return BadRequest("Task Id Can't Be Zero");
            try
            {
                var task = await archivedTaskService.ArchiveTaskAsync(taskId);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("restore-task/{taskId}")]
        public async Task<IActionResult> RestoreTask(int taskId)
        {
            if (taskId == 0)
                return BadRequest("Task Id Can't Be Zero");
            try
            {
                var task = await archivedTaskService.RestoreTaskAsync(taskId);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("archived-task/{taskId}")]
        public async Task<IActionResult> GetArchivedTaskById(int taskId)
        {
            if (taskId == 0)
                return BadRequest("Task Id Can't Be Zero");
            try
            {
                var task = await archivedTaskService.GetArchivedTaskByIdAsync(taskId);
                return Ok(task);
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

        [HttpGet("paginated")]
        public async Task<IActionResult> GetPaginatedTasks(
            [FromQuery] string? fromDate,
            [FromQuery] string? toDate,
            [FromQuery] string? employeeId,
            [FromQuery] int? clientId,
            [FromQuery] int? status,
            [FromQuery] string? priority,
            [FromQuery] string? onDeadline,
            [FromQuery] string? team,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? currentUserId = null)
        {
            try
            {
                var filter = new TaskFilterDTO
                {
                    FromDate = string.IsNullOrEmpty(fromDate) ? null : DateTime.Parse(fromDate),
                    ToDate = string.IsNullOrEmpty(toDate) ? null : DateTime.Parse(toDate),
                    EmployeeId = employeeId,
                    ClientId = clientId,
                    Status = status,
                    Priority = priority,
                    OnDeadline = onDeadline,
                    Team = team,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                // If no currentUserId provided, try to get from claims
                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = User.FindFirst("sub")?.Value;
                }

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return BadRequest("User ID is required");
                }

                var result = await taskService.GetPaginatedTasksAsync(filter, currentUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("task-unified/{taskId}/{isArchived}")]
        public async Task<IActionResult> GetTaskUnified(int taskId, bool isArchived)
        {
            if (taskId == 0)
                return BadRequest("Task Id Can't Be Zero");

            if (!isArchived)
            {
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
            else
            {
                try
                {
                    var task = await archivedTaskService.GetArchivedTaskByIdAsync(taskId);
                    return Ok(task);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost("new-comment")]
        public async Task<IActionResult> AddCommentAsync(CreateTaskCommentDTO taskCommentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var result = await taskService.AddCommentAsync(taskCommentDTO);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}