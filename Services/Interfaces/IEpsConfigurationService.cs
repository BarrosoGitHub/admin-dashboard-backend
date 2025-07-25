using OPTConfigurator.Models;

namespace OPTConfigurator.Services.Interfaces;

public interface IEpsConfigurationService
{
    Task<EpsConfiguration?> GetConfigurationAsync();
    Task SetConfigurationAsync(EpsConfiguration config);
    Task CreateTemplateConfigurationFileAsync();
}

