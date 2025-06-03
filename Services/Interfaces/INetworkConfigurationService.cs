using OPTConfigurator.Models;

namespace OPTConfigurator.Services.Interfaces;

public interface INetworkConfigurationService
{
    Task<GetNetworkConfigurationDTO> GetNetworkConfigurationAsync();
    Task UpdateNetworkConfigurationAsync(UpdateNetworkConfigurationDTO configuration);

}