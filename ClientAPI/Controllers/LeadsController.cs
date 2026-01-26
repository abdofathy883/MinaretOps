using Core.DTOs.Leads;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private readonly ILeadService leadService;

        public LeadsController(ILeadService leadService)
        {
            this.leadService = leadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            try
            {
                var leads = await leadService.GetAllLeadsAsync();
                return Ok(leads);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateLead(CreateLeadDTO createLeadDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Assign CreatedById from Claims if not present (although DTO has it required, usually frontend sends it or backend overrides it)
                // For now, relying on DTO or Service to handle logic better if needed.
                // But DTO says 'required string CreatedById'.
                
                var lead = await leadService.CreateLeadAsync(createLeadDTO);
                return CreatedAtAction(nameof(GetLeadById), new { id = lead.Id }, lead);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadById(int id)
        {
            try
            {
                var lead = await leadService.GetLeadByIdAsync(id);
                return Ok(lead);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateLeadField(int id, [FromBody] JsonElement payload)
        {
             // payload: { "fieldName": "BusinessName", "value": "New Name" }
             try 
             {
                 if (!payload.TryGetProperty("fieldName", out var fieldNameProp) || !payload.TryGetProperty("value", out var valueProp))
                 {
                     return BadRequest("Invalid payload. Expected 'fieldName' and 'value'.");
                 }

                 string fieldName = fieldNameProp.GetString();
                 // valueProp is a JsonElement. We pass it as object to service, which handles deserialization.
                 object value = valueProp;

                 var updatedLead = await leadService.UpdateLeadFieldAsync(id, fieldName, value);
                 return Ok(updatedLead);
             }
             catch (KeyNotFoundException)
             {
                 return NotFound();
             }
             catch (Exception ex)
             {
                 return BadRequest(ex.Message);
             }
        }
    }
}
