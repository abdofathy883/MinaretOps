using Core.DTOs.Seo;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeoController : ControllerBase
    {
        private readonly ISeoService seoService;

        public SeoController(ISeoService seoService)
        {
            this.seoService = seoService;
        }

        [HttpGet("{*route}")]
        public async Task<IActionResult> Get(string route, [FromQuery] string language = "en")
        {
            // Decode the route if necessary, though ASP.NET Core usually handles it
            var content = await seoService.GetContentByRoute(route, language);
            if (content == null)
            {
                 // Return empty object or default
                 return Ok(new SeoContentDTO { Route = route, Language = language });
            }
            return Ok(content);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromForm] CreateSeoContentDTO model)
        {
            if (string.IsNullOrEmpty(model.Route))
            {
                 return BadRequest("Route is required.");
            }

            var result = await seoService.UpdateSeoContent(model);
            return Ok(result);
        }
    }
}
