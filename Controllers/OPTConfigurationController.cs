using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfigurator.Controllers;

[ApiController]
[Route("opt-configuration")]
[Authorize]
public class ConfigurationController : ControllerBase
{
    private IOptConfigurationService _optConfigurationService;

    public ConfigurationController(IOptConfigurationService optConfigurationService)
    {
        _optConfigurationService = optConfigurationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentOptConfiguration()
    {
        var result = await _optConfigurationService.GetOptConfigurationAsync();
        if (result == null)
        {
            Console.WriteLine("No configuration found.");
            return NotFound("No configuration found.");
        }

        var jsonSettings = new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        var resultJson = new JsonResult(result, jsonSettings);
        
        return resultJson;
    }

    [HttpGet("template")]
    public IActionResult GetOptConfigurationTemplate([FromQuery] GetOptConfigurationTemplateDTO template)
    {
        try
        {
            var optConfiguration = _optConfigurationService.GetOptConfigurationFromTemplate(template);
            var jsonSettings = new System.Text.Json.JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            return new JsonResult(optConfiguration, jsonSettings);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
    }

    [HttpPost("new")]
    public IActionResult AddOptConfiguration([FromBody] AddOptConfigurationDTO config)
    {
        try
        {
            var optConfiguration = _optConfigurationService.AddOptConfiguration(config);
            return CreatedAtAction(nameof(GetCurrentOptConfiguration), optConfiguration);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
    }


    [HttpPut]
    public IActionResult UpdateOptConfiguration([FromBody] UpdateOptConfigurationDTO config)
    {
        try
        {
            var updatedConfig = _optConfigurationService.UpdateOptConfiguration(config);
            var jsonSettings = new System.Text.Json.JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            return new JsonResult(updatedConfig, jsonSettings);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
    }

    [HttpGet("is-available")]
    public ActionResult GetConfiguratorType()
    {
        return _optConfigurationService.IsOptServiceEnabled()
            ? Ok(new { available = true })
            : NotFound(new { available = false });
    }

    [HttpPost("toggle-tech-mode")]
    public ActionResult ToggleTechMode()
    {
        bool success = _optConfigurationService.ToggleTechMode();
        return Ok(success);
    }

    [HttpGet("is-tech-mode-enabled")]
    public ActionResult IsTechModeEnabled()
    {
        return Ok(_optConfigurationService.IsTechModeEnabled());
    }
}

