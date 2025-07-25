using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;
using Petrotec.Opt.Data.Models.Configuration.Opt;

namespace OPTConfigurator.Services;

public class UnavailableService : IOptConfigurationService, IEpsConfigurationService
{
    public AddOptConfigurationDTO AddOptConfiguration(AddOptConfigurationDTO optConfig)
    {
        return null!;
    }

    public Task CreateTemplateConfigurationFileAsync()
    {
        return null!;
    }

    public Task<EpsConfiguration?> GetEpsConfigurationAsync()
    {
        return Task.FromResult<EpsConfiguration?>(null);
    }

    public Task<OptConfiguration> GetOptConfigurationAsync()
    {
        return Task.FromResult(new OptConfiguration());
    }

    public OptConfiguration GetOptConfigurationFromTemplate(GetOptConfigurationTemplateDTO optConfigFromTemplate)
    {
        return null!;
    }

    public EpsConfiguration GetEpsConfigurationFromTemplate(GetEpsConfigurationTemplateDTO epsConfigFromTemplate)
    {
        return null!;
    }

    public bool IsEpsServiceEnabled()
    {
        return false;
    }

    public bool IsOptServiceEnabled()
    {
        return false;
    }

    public Task SetConfigurationAsync(EpsConfiguration config)
    {
        return Task.CompletedTask;
    }

    public OptConfiguration UpdateOptConfiguration(UpdateOptConfigurationDTO updateOptConfig)
    {
        return null!;
    }

    public EpsConfiguration AddEpsConfiguration(EpsConfiguration epsConfiguration)
    {
        return null!;
    }

    public EpsConfiguration UpdateEpsConfiguration(EpsConfiguration epsConfig)
    {
        return null!;
    }
}