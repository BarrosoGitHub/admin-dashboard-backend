using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OPTConfigurator.Controllers
{
    [Route("[controller]")]
    public class LogsController : Controller
    {

        public LogsController()
        {

        }

        public IActionResult Get()
        {
            var logFiles = new[]
            {
                "eps_configurator.log",
                "eps_configurator_error.log"
            };

            
            return Ok();
        }
    }
}