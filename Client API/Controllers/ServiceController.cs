using Core.DTOs.Services;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService serviceService;
        public ServiceController(IServiceService _serviceService)
        {
            serviceService = _serviceService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServicesAsync()
        {
            var services = await serviceService.GetAllServicesAsync();
            if (services.Count < 0)
                return NotFound("لا توجد خدمات");

            return Ok(services);
        }

        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetServiceByIdAsync(int serviceId)
        {
            var service = await serviceService.GetServiceByIdAsync(serviceId);
            if (service != null)
            {
                return Ok(service);
            }
            return NotFound("الخدمة غير موجودة");
        }

        [HttpPost]
        public async Task<IActionResult> AddServiceAsync([FromBody] CreateServiceDTO newService)
        {
            if (newService == null)
            {
                return BadRequest("بيانات الخدمة غير صحيحة");
            }
            var service = await serviceService.AddServiceAsync(newService);
            return Ok(service);
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateServiceAsync(UpdateServiceDTO updateServiceDTO)
        {
            if (updateServiceDTO is null)
                return BadRequest();
            var result = await serviceService.UpdateServiceAsync(updateServiceDTO);
            return Ok(result);
        }

        [HttpPatch("toggle-visibility/{serviceId}")]
        public async Task<IActionResult> ToggleVisibilityAsync(int serviceId)
        {
            if (serviceId == 0)
                return BadRequest();
            var result = await serviceService.ToggleVisibilityAsync(serviceId);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteServiceAsync(int serviceId)
        {
            if (serviceId == 0)
                return BadRequest();
            var result = await serviceService.DeleteServiceAsync(serviceId);
            return Ok(result);
        }
    }
}
