using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Helpers;
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
    }
}
