using System.Text.Json;
using FluentValidation;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfigurator.Services;

public class UserInterfaceConfigurationService : IUserInterfaceConfigurationService
{
    public readonly IValidator<UserInterfaceConfigurationDTO> _userInterfaceConfigurationValidator;

    public UserInterfaceConfigurationService(IValidator<UserInterfaceConfigurationDTO> userInterfaceConfigurationValidator)
    {
        _userInterfaceConfigurationValidator = userInterfaceConfigurationValidator;
    }

    public async Task<UserInterfaceConfigurationDTO> GetCurrentUserInterfaceConfiguration()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "user_interface_configuration.json");
        if (!File.Exists(filePath))
            return null!;

        var json = await File.ReadAllTextAsync(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Deserialize<UserInterfaceConfigurationDTO>(json, options)!;
    }

    public UserInterfaceConfigurationDTO AddUserInterfaceConfiguration(UserInterfaceConfigurationDTO userInterfaceConfig)
    {
        var validationResult = _userInterfaceConfigurationValidator.Validate(userInterfaceConfig);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        string filePath = Path.Combine(AppContext.BaseDirectory, "user_interface_configuration.json");
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string userInterfaceConfigJson = JsonSerializer.Serialize(userInterfaceConfig, options);
        File.WriteAllText(filePath, userInterfaceConfigJson);

        return userInterfaceConfig;
    }

    public UserInterfaceConfigurationDTO UpdateUserInterfaceConfiguration(UserInterfaceConfigurationDTO userInterfaceConfig)
    {
        var validationResult = _userInterfaceConfigurationValidator.Validate(userInterfaceConfig);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        string filePath = Path.Combine(AppContext.BaseDirectory, "user_interface_configuration.json");
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string userInterfaceConfigJson = JsonSerializer.Serialize(userInterfaceConfig, options);
        File.WriteAllText(filePath, userInterfaceConfigJson);

        return userInterfaceConfig;
    }
}