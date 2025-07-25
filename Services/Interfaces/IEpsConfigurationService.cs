using OPTConfigurator.Models;

namespace OPTConfigurator.Services.Interfaces;

public interface IEpsConfigurationService
{
    Task<EpsConfiguration?> GetEpsConfigurationAsync();
    EpsConfiguration AddEpsConfiguration(EpsConfiguration epsConfiguration);
    EpsConfiguration UpdateEpsConfiguration(EpsConfiguration epsConfig);
    Task SetConfigurationAsync(EpsConfiguration config);
    Task CreateTemplateConfigurationFileAsync();
    EpsConfiguration GetEpsConfigurationFromTemplate(GetEpsConfigurationTemplateDTO epsConfigFromTemplate);
    bool IsEpsServiceEnabled();
}

