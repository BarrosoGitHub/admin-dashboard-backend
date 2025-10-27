using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;
using Petrotec.ZorPay.Common.Models;
using Petrotec.ZorPay.Common.Types;
using Petrotec.ZorPay.Models;
using Petrotec.ZorPay.Opi.Extension.Models.Types;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace EPSConfigurator.Services
{
    public class EpsConfigurationService : IEpsConfigurationService
    {
        private readonly string _filePath;

        public EpsConfigurationService()
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, "files", "eps_config.json");
        }

        public async Task<EpsConfiguration?> GetEpsConfigurationAsync()
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

        public EpsConfiguration AddEpsConfiguration(EpsConfiguration epsConfiguration)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            string optConfigJson = JsonSerializer.Serialize(epsConfiguration, options);

            File.WriteAllText(_filePath, optConfigJson);

            return epsConfiguration;
        }

        public EpsConfiguration UpdateEpsConfiguration(EpsConfiguration epsConfig)
        {
            // var validationResult = _updateOptConfigurationValidator.Validate(epsConfig);
            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

            string filePath = Path.Combine(AppContext.BaseDirectory, "files", "eps_config.json");

            EpsConfiguration existingConfig = null!;
            if (File.Exists(filePath))
            {
                var existingJson = File.ReadAllText(filePath);
                existingConfig = ConvertJsonToEpsConfiguration(existingJson);
            }
            else
            {
                existingConfig = new EpsConfiguration();
            }

            // Use a safer approach for updating properties
            UpdateConfigurationProperties(existingConfig, epsConfig);

            // Save updated configuration
            string updatedJson = JsonSerializer.Serialize(epsConfig, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            File.WriteAllText(filePath, updatedJson);

            return epsConfig;
        }

        private void UpdateConfigurationProperties(EpsConfiguration target, EpsConfiguration source)
        {
            var properties = typeof(EpsConfiguration).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var sourceValue = property.GetValue(source);
                if (sourceValue == null)
                    continue;

                try
                {
                    if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                    {
                        // Simple types (int, string, bool, etc.)
                        property.SetValue(target, sourceValue);
                    }
                    else if (property.PropertyType.IsClass)
                    {
                        // Complex objects
                        var targetValue = property.GetValue(target);
                        if (targetValue == null)
                        {
                            // Create new instance if target is null
                            var newInstance = Activator.CreateInstance(property.PropertyType);
                            property.SetValue(target, newInstance);
                            targetValue = newInstance;
                        }

                        // Recursively update properties of complex objects
                        UpdateComplexProperty(targetValue, sourceValue, property.PropertyType);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and continue with other properties
                    Console.WriteLine($"Error updating property {property.Name}: {ex.Message}");
                }
            }
        }

        private void UpdateComplexProperty(object target, object source, Type propertyType)
        {
            if (target == null || source == null)
                return;

            var properties = propertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                // Skip indexed properties (properties with parameters)
                if (property.GetIndexParameters().Length > 0)
                    continue;

                try
                {
                    var sourceValue = property.GetValue(source);
                    if (sourceValue != null)
                    {
                        if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                        {
                            property.SetValue(target, sourceValue);
                        }
                        else if (property.PropertyType.IsClass)
                        {
                            // Handle nested complex objects
                            var targetValue = property.GetValue(target);
                            if (targetValue == null)
                            {
                                var newInstance = Activator.CreateInstance(property.PropertyType);
                                property.SetValue(target, newInstance);
                                targetValue = newInstance;
                            }
                            UpdateComplexProperty(targetValue, sourceValue, property.PropertyType);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error and continue with other properties
                    Console.WriteLine($"Error updating nested property {property.Name}: {ex.Message}");
                }
            }
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

        public bool IsEpsServiceEnabled()
        {
            return true;
        }

        public EpsConfiguration GetEpsConfigurationFromTemplate(GetEpsConfigurationTemplateDTO epsConfigFromTemplate)
        {
            EpsConfiguration optConfig = ConvertJsonToEpsConfiguration(CreateOptConfigurationTemplate(epsConfigFromTemplate));

            return optConfig;
        }

        private EpsConfiguration ConvertJsonToEpsConfiguration(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return JsonSerializer.Deserialize<EpsConfiguration>(json, options)
                   ?? throw new InvalidOperationException("Failed to deserialize configuration.");
        }

        private string CreateOptConfigurationTemplate(GetEpsConfigurationTemplateDTO epsConfigurationDTO)
        {
            string templatePath = Path.Combine(AppContext.BaseDirectory, "templates", "eps_configuration_template.json");
            
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"EPS configuration template file not found at: {templatePath}");
            }

            return File.ReadAllText(templatePath);
        }
    }
}
