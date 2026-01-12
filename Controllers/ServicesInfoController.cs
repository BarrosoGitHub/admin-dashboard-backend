using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Helpers;
using OPTConfigurator.Services;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfigurator.Controllers
{
    [ApiController]
    [Route("info/services")]
    [Authorize]
    public class ServicesInfoController : ControllerBase
    {
        private readonly IServicesInfoService _servicesInfoService;

        public ServicesInfoController(IServicesInfoService servicesInfoService)
        {
            _servicesInfoService = servicesInfoService;
        }

        [HttpGet]
        public ActionResult<List<AppInfo>> GetAllServicesInfo()
        {
            var result = _servicesInfoService.GetAllServicesInfo();
            return Ok(result);
        }

        [HttpGet("boardtype")]
        public ActionResult<string> GetBoardType()
        {
            var boardType = BoardHelper.GetBoardType();
            return Ok(boardType);
        }

        [HttpGet("timezone")]
        public ActionResult<string> GetTimeZone()
        {
            try
            {
                var timeZone = _servicesInfoService.GetTimeZone();
                return Ok(timeZone);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("timezone")]
        public ActionResult<string> SetTimeZone([FromBody] SetTimeZoneRequest request)
        {
            try
            {
                var timeZone = _servicesInfoService.SetTimeZone(request.TimeZone);
                return Ok(new { TimeZone = timeZone, Message = "Timezone updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("timezone/valid")]
        public ActionResult<List<string>> GetValidTimeZones()
        {
            var validTimeZones = _servicesInfoService.GetValidTimeZones();
            return Ok(validTimeZones);
        }

        [HttpGet("isalive")]
        [AllowAnonymous]
        public ActionResult IsAlive()
        {
            if (ApplicationState.IsRebooting)
            {
                return StatusCode(503, new { status = "rebooting", message = "System is rebooting" });
            }
            return Ok(new { status = "alive", timestamp = DateTime.UtcNow });
        }
    }

    public class SetTimeZoneRequest
    {
        public required string TimeZone { get; set; }
    }
}
