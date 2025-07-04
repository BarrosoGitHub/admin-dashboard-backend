using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfiguration.API.Application.Controllers;

[ApiController]
[Route("configuration/network")]
[Authorize]
public class NetworkController : ControllerBase
{
    private readonly INetworkConfigurationService _networkService;

    public NetworkController(INetworkConfigurationService networkService)
    {
        _networkService = networkService;
    }

    [HttpGet]
    public async Task<ActionResult<GetNetworkConfigurationDTO>> GetNetworkConfiguration()
    {
        var config = await _networkService.GetNetworkConfigurationAsync();
        return Ok(config);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateNetworkConfiguration([FromBody] UpdateNetworkConfigurationDTO configuration)
    {
        var success = await _networkService.UpdateNetworkConfigurationAsync(configuration);
        
        if (success)
        {
            return Ok(new { success = true, message = "Network configuration updated successfully." });
        }
        else
        {
            return BadRequest(new { success = false, message = "Failed to update network configuration." });
        }
    }
}