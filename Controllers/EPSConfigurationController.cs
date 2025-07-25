using OPTConfigurator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Services.Interfaces;

namespace EPSConfigurator.Controllers
{
    [ApiController]
    [Route("eps-configuration")]
    [Authorize]
    public class ConfigurationController : ControllerBase
    {
        private readonly IEpsConfigurationService _configService;

        public ConfigurationController(IEpsConfigurationService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public async Task<ActionResult<EpsConfiguration>> GetConfiguration()
        {
            var result = await _configService.GetConfigurationAsync();
            if (result == null)
                return NotFound("No configuration found.");

            var jsonSettings = new System.Text.Json.JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            return new JsonResult(result, jsonSettings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConfiguration([FromBody] EpsConfiguration config)
        {
            await _configService.SetConfigurationAsync(config);
            return Ok();
        }

        [HttpPost("new")]
        public async Task<IActionResult> AddConfiguration()
        {
            try
            {
                await _configService.CreateTemplateConfigurationFileAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("is-available")]
        public ActionResult GetConfiguratorType()
        {
            return Ok();
        }
    }
}
