using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;
using OPTConfigurator.Types;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace EPSConfigurator.Services
{
    public class EpsConfigurationService : IEpsConfigurationService
    {
        private readonly string _filePath;

        public EpsConfigurationService()
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, "files", "eps_configuration.json");
        }

        public async Task<EpsConfiguration?> GetConfigurationAsync()
        {
            if (!File.Exists(_filePath))
                return null;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            string configJson = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<EpsConfiguration>(configJson, options);
        }

        public async Task SetConfigurationAsync(EpsConfiguration config)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                WriteIndented = true
            };
            string configJson = JsonSerializer.Serialize(config, options);

            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await File.WriteAllTextAsync(_filePath, configJson);
        }

        public async Task CreateTemplateConfigurationFileAsync()
        {
            if (File.Exists(_filePath))
                return;

            var template = new EpsConfiguration
            {
                RegisteredTerminals = new List<EpsPosClient>(),
                InstanceId = string.Empty,
                InstanceName = string.Empty,
                CountryId = string.Empty,
                Acquirers = new List<EpsAcquirer>(),
                Languages = new List<EpsLanguage>(),
                Messages = new List<EpsMessage>(),
                ServicePort = 0,
                DefaultAuthorizationValue = 100,
                DefaultLanguage = EpsMessageLanguage.Pt,
                TimeWaitCheckCardPresenceInSeconds = 30,
                TimeWaitMagneticStripeDataInSeconds = 30,
                TimeWaitGetKeyboardStringDataInSeconds = 30,
                TimeWaitDisplayCustomerMessageInSeconds = 30
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                WriteIndented = true
            };
            string configJson = JsonSerializer.Serialize(template, options);

            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await File.WriteAllTextAsync(_filePath, configJson);
        }
    }
}
