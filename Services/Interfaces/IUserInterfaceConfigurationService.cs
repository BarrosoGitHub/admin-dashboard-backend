using OPTConfigurator.Models;

namespace OPTConfigurator.Services.Interfaces
{
    public interface IUserInterfaceConfigurationService
    {
        Task<UserInterfaceConfigurationDTO> GetCurrentUserInterfaceConfiguration();
        UserInterfaceConfigurationDTO AddUserInterfaceConfiguration(UserInterfaceConfigurationDTO config);
        UserInterfaceConfigurationDTO UpdateUserInterfaceConfiguration(UserInterfaceConfigurationDTO config);
    }
}