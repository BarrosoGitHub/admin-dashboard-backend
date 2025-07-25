using OPTConfigurator.Models;
using Petrotec.Opt.Data.Models.Configuration.Opt;

namespace OPTConfigurator.Services.Interfaces
{
    public interface IOptConfigurationService
    {
        Task<OptConfiguration> GetOptConfigurationAsync();
        AddOptConfigurationDTO AddOptConfiguration(AddOptConfigurationDTO optConfig);
        OptConfiguration GetOptConfigurationFromTemplate(GetOptConfigurationTemplateDTO optConfigFromTemplate);
        OptConfiguration UpdateOptConfiguration(UpdateOptConfigurationDTO updateOptConfig);
        bool IsOptServiceEnabled();
    }
}