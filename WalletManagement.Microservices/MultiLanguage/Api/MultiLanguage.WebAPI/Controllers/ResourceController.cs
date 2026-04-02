using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.Application.Managers;
using System.Globalization;

namespace MultiLanguage.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly IResourceManager _resourceManager;

        public ResourceController(IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        [HttpGet("get-translation")]
        public async Task<IActionResult> GetTranslation([FromQuery] string key)
        {
            var cultureCode = CultureInfo.CurrentCulture.Name;

            var result = await _resourceManager.GetStringAsync(key, cultureCode);
            return Ok(new
            {
                Language = cultureCode,
                Message = result
            });
        }

        [HttpGet("get-resourceBundle")]
        public async Task<IActionResult> GetResourceBundle([FromQuery] string culture)
        {
            var result = await _resourceManager.GetResourceBundleAsync(culture);
            return Ok(result);
        }
    }
}
