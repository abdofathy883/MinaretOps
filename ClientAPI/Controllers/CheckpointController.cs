using Core.DTOs.Checkpoints;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckpointController : ControllerBase
    {
        private readonly ICheckpointService checkpointService;
        private readonly ILogger<CheckpointController> logger;

        public CheckpointController(
            ICheckpointService _checkpointService,
            ILogger<CheckpointController> _logger)
        {
            checkpointService = _checkpointService;
            logger = _logger;
        }

        [HttpPost("service-checkpoint")]
        public async Task<ActionResult<ServiceCheckpointDTO>> AddServiceCheckpoint([FromBody] CreateServiceCheckpointDTO dto)
        {
            try
            {
                var result = await checkpointService.AddServiceCheckpointAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding service checkpoint");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("service/{serviceId}/checkpoints")]
        public async Task<ActionResult<List<ServiceCheckpointDTO>>> GetServiceCheckpoints(int serviceId)
        {
            try
            {
                var result = await checkpointService.GetServiceCheckpointsAsync(serviceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting service checkpoints");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("service-checkpoint/{checkpointId}")]
        public async Task<ActionResult<ServiceCheckpointDTO>> UpdateServiceCheckpoint(
            int checkpointId,
            [FromBody] UpdateServiceCheckpointDTO dto)
        {
            try
            {
                var result = await checkpointService.UpdateServiceCheckpointAsync(checkpointId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating service checkpoint");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("service-checkpoint/{checkpointId}")]
        public async Task<ActionResult> DeleteServiceCheckpoint(int checkpointId)
        {
            try
            {
                await checkpointService.DeleteServiceCheckpointAsync(checkpointId);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting service checkpoint");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("client-service/{clientServiceId}/checkpoints")]
        public async Task<ActionResult<List<ClientServiceCheckpointDTO>>> GetClientServiceCheckpoints(int clientServiceId)
        {
            try
            {
                var result = await checkpointService.GetClientServiceCheckpointsAsync(clientServiceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting client service checkpoints");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("client-service-checkpoint/{checkpointId}/complete")]
        public async Task<ActionResult<ClientServiceCheckpointDTO>> MarkCheckpointComplete(
            int checkpointId,
            [FromBody] MarkCheckpointCompleteDTO dto)
        {
            try
            {
                var result = await checkpointService.MarkCheckpointCompleteAsync(checkpointId, dto.EmployeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error marking checkpoint complete");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("client-service-checkpoint/{checkpointId}/incomplete")]
        public async Task<ActionResult<ClientServiceCheckpointDTO>> MarkCheckpointIncomplete(int checkpointId)
        {
            try
            {
                var result = await checkpointService.MarkCheckpointIncompleteAsync(checkpointId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error marking checkpoint incomplete");
                return BadRequest(ex.Message);
            }
        }
    }
}