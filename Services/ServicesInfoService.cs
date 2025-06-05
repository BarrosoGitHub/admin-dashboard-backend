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
                new AppInfo("module-1", "hash-abc123")
                {
                    Status = "Running",
                    Message = "Module 1 is operational.",
                    StartUpTime = DateTime.Now.AddMinutes(-10)
                },
                new AppInfo("module-2", "hash-def456")
                {
                    Status = "Stopped",
                    Message = "Module 2 stopped for maintenance.",
                    StartUpTime = DateTime.Now.AddHours(-1)
                },
                new AppInfo("module-3", "hash-ghi789")
                {
                    Status = "Starting",
                    Message = "Module 3 is starting up.",
                    StartUpTime = DateTime.Now.AddMinutes(-2)
                }
            };
        }
    }
}
