using OPTConfigurator.Models;

namespace OPTConfigurator.Services.Interfaces;

public interface IServicesInfoService
{
    List<AppInfo> GetAllServicesInfo();
    string GetTimeZone();
    string SetTimeZone(string timeZone);
    List<string> GetValidTimeZones();
}