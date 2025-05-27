using OPTConfigurator.Models;
using Petrotec.Opt.Data.Models.Configuration.Opt;

namespace OPTConfigurator.Services.Interfaces
{
    public interface IOptConfigurationService
    {
        Task<OptConfiguration> GetCurrentOptConfiguration();
        OptConfiguration AddOptConfiguration(AddOptConfigurationDTO addOptConfig);
        OptConfiguration UpdateOptConfiguration(UpdateOptConfigurationDTO updateOptConfig);
    }
}