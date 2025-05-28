using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfigurator.Controllers;

[ApiController]
[Route("configuration/ui")]
public class UserInterfaceConfigurationController : ControllerBase
{
    private readonly IUserInterfaceConfigurationService _userInterfaceConfigurationService;

    public UserInterfaceConfigurationController(IUserInterfaceConfigurationService userInterfaceConfigurationService)
    {
        _userInterfaceConfigurationService = userInterfaceConfigurationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserInterfaceConfiguration()
    {
        var result = await _userInterfaceConfigurationService.GetCurrentUserInterfaceConfiguration();
        if (result == null)
            return NotFound("No user interface configuration found.");

        var jsonSettings = new System.Text.Json.JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        return new JsonResult(result, jsonSettings);
    }

    [HttpPost]
    public IActionResult AddUserInterfaceConfiguration([FromBody] UserInterfaceConfigurationDTO config)
    {
        try
        {
            var userInterfaceConfig = _userInterfaceConfigurationService.AddUserInterfaceConfiguration(config);
            return CreatedAtAction(nameof(GetCurrentUserInterfaceConfiguration), userInterfaceConfig);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
    }
}
