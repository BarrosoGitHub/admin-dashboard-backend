using OPTConfigurator.Models;
using Petrotec.Opt.Data.Models.Configuration.Opt;

namespace OPTConfigurator.Services.Interfaces
{
    public interface IOptConfigurationService
    {
        Task<OptConfiguration> GetCurrentOptConfiguration();
        AddOptConfigurationDTO AddOptConfiguration(AddOptConfigurationDTO optConfig);
        OptConfiguration GetOptConfigurationFromTemplate(GetOptConfigurationTemplateDTO optConfigFromTemplate);
        OptConfiguration UpdateOptConfiguration(UpdateOptConfigurationDTO updateOptConfig);
    }
}