using System.Collections.Generic;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfigurator.Services
{
    public class ServicesInfoService : IServicesInfoService
    {
        public List<AppInfo> GetAllServicesInfo()
        {
            // Mock modules for demonstration
            return new List<AppInfo>
            {
                new AppInfo("fm-footer-web", "9827b96a")
                {
                    Status = "Healthy",
                    Message = "Module 1 is operational.",
                    StartUpTime = DateTime.Now.AddMinutes(-10),
                    Version = "1.4.5.0"
                },
                new AppInfo("fm-display-web", "9827b96a")
                {
                    Status = "Stopped",
                    Message = "Module 2 stopped for maintenance.",
                    StartUpTime = DateTime.Now.AddHours(-1),
                    Version = "1.7.5.0"
                },
                new AppInfo("emc-calculator", "9827b96a")
                {
                    Status = "Healthy",
                    Message = "Module 3 is starting up.",
                    StartUpTime = DateTime.Now.AddMinutes(-2),
                    Version = "2.4.5.0"
                },
                new AppInfo("emc-display-web", "9827b96a")
                {
                    Status = "Healthy",
                    Message = "Module 1 is operational.",
                    StartUpTime = DateTime.Now.AddMinutes(-10),
                    Version = "0.0.5.0"
                },
                new AppInfo("pfs-electron", "9827b96a")
                {
                    Status = "Stopped",
                    Message = "Module 2 stopped for maintenance.",
                    StartUpTime = DateTime.Now.AddHours(-1),
                    Version = "1.1.5.0"
                }
            };
        }
    }
}
