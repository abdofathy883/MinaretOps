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

        [HttpGet]
        public async Task<IActionResult> GetAllTasksAsync()
        {
            try
            {
                var tasks = await taskService.GetAllTasksAsync();
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

        [HttpPut("update-task/{taskId}")]
        public async Task<IActionResult> UpdateTaskAsync(int taskId, UpdateTaskDTO updateTaskDTO)
        {
            if (taskId == 0 || updateTaskDTO is null)
                return BadRequest("Task Id is Null Or New Task Object Is Null");
            try
            {
                await taskService.UpdateTaskAsync(taskId, updateTaskDTO);
                return Ok(new { message = "Task updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("change-status/{taskId}")]
        public async Task<IActionResult> ChangeTaskStatusAsync(int taskId, [FromBody] CustomTaskStatus status)
        {
            var result = await taskService.ChangeTaskStatusAsync(taskId, status);
            return Ok(result);
        }

        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTaskAsync(CreateTaskDTO createTaskDTO)
        {
            if (createTaskDTO is null)
                return BadRequest("New Task Object Is Null");
            try
            {
                var createdTask = await taskService.CreateTaskAsync(createTaskDTO);
                return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTask.Id }, createdTask);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-task-group")]
        public async Task<IActionResult> CreateTaskGroupAsync(CreateTaskGroupDTO createTaskGroupDTO)
        {
            if (createTaskGroupDTO is null)
                return BadRequest("Task Group Object Is Null");
            try
            {
                var createdTaskGroup = await taskService.CreateTaskGroupAsync(createTaskGroupDTO);
                return CreatedAtAction(nameof(GetTaskById), new { taskId = createdTaskGroup.Id }, createdTaskGroup);
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
    }
}
