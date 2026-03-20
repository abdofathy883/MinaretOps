using Application.Interfaces;
using Application.DTOs.Clients;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientServices clientService;
        public ClientController(IClientServices client)
        {
            clientService = client;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveAsync()
        {
            var clients = await clientService.GetAllActiveAsync();

            return Ok(clients);
        }
        
        [HttpGet("all/{status}")]
        public async Task<IActionResult> GetAllAsync(ClientStatus status)
        {
            var clients = await clientService.GetAllAsync(status);

            return Ok(clients);
        }

        [HttpGet("search/{clientName}")]
        public async Task<ActionResult<LightWieghtClientDTO>> Search(string clientName)
        {
            var result = await clientService.SearchAsync(clientName);
            return Ok(result);
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetByIdAsync(int clientId)
        {
            if (clientId == 0)
                return BadRequest();

            var client = await clientService.GetClientByIdAsync(clientId);
            return Ok(client);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> AddClientAsync(CreateClientDTO createClientDTO, string userId)
        {
            if (createClientDTO is null)
                return BadRequest();
            try
            {
                var result = await clientService.AddClientAsync(createClientDTO, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{clientId}")]
        public async Task<IActionResult> UpdateClientAsync(int clientId, UpdateClientDTO updateClientDTO)
        {
            if (clientId == 0 || updateClientDTO is null)
                return BadRequest();

            var result = await clientService.UpdateClientAsync(clientId, updateClientDTO);
            return Ok(result);
        }

        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteClientAsync(int clientId)
        {
            if (clientId == 0)
                return BadRequest();

            var result = await clientService.DeleteClientAsync(clientId);

            return Ok(result);
        }
    }
}
