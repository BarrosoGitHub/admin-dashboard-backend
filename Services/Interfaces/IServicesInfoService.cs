using System.Collections.Generic;
using OPTConfigurator.Models;

namespace OPTConfigurator.Services.Interfaces
{
    public interface IServicesInfoService
    {
        List<AppInfo> GetAllServicesInfo();
    }
}
