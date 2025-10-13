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
        var filePath = Path.Combine(AppContext.BaseDirectory, "files", "ui_config.json");
        if (!File.Exists(filePath))
        {
            // Create and save default configuration
            var defaultConfig = CreateDefaultUserInterfaceConfiguration();
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string defaultConfigJson = JsonSerializer.Serialize(defaultConfig, options);
            
            // Ensure the files directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, defaultConfigJson);
            
            return defaultConfig;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var deserializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Deserialize<UserInterfaceConfigurationDTO>(json, deserializeOptions)!;
    }

    public UserInterfaceConfigurationDTO AddUserInterfaceConfiguration(UserInterfaceConfigurationDTO userInterfaceConfig)
    {
        var validationResult = _userInterfaceConfigurationValidator.Validate(userInterfaceConfig);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        string filePath = Path.Combine(AppContext.BaseDirectory, "files", "ui_config.json");
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

        string filePath = Path.Combine(AppContext.BaseDirectory, "files", "ui_config.json");
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        string userInterfaceConfigJson = JsonSerializer.Serialize(userInterfaceConfig, options);
        File.WriteAllText(filePath, userInterfaceConfigJson);

        return userInterfaceConfig;
    }

    private UserInterfaceConfigurationDTO CreateDefaultUserInterfaceConfiguration()
    {
        return new UserInterfaceConfigurationDTO
        {
            Client = "default",
            WebsocketServerUrl = "ws://localhost:8088/ws/opt",
            ShowDebugButton = false,
            LoadLastOPTRequestOnStartup = false,
            ReloadWhenGoingToIdleScreen = false,
            ShowVideo = false,
            AxonMultimediaIframe = false,
            AxonMultimediaUrl = "",
            GradeColors = new List<Dictionary<string, string>>()
        };
    }
}