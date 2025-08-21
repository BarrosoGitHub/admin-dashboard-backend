using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;
using OPTConfigurator.Types;
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
            _filePath = Path.Combine(AppContext.BaseDirectory, "files", "eps_configuration.json");
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

            string filePath = Path.Combine(AppContext.BaseDirectory, "files", "eps_configuration.json");

            EpsConfiguration existingConfig = null;
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
            string updatedJson = JsonSerializer.Serialize(existingConfig, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            File.WriteAllText(filePath, updatedJson);

            return existingConfig;
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

        public bool IsEpsServiceEnabled()
        {
            return true;
        }

        public EpsConfiguration GetEpsConfigurationFromTemplate(GetEpsConfigurationTemplateDTO epsConfigFromTemplate)
        {
            // var validationResult = _getOptConfigurationTemplateValidator.Validate(epsConfigTemplate);
            // if (!validationResult.IsValid)
            // {
            //     throw new ValidationException(validationResult.Errors);
            // }

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
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            var optConfig = new EpsConfiguration
            {
                RegisteredTerminals = new List<EpsPosClient>
            {
                new EpsPosClient
                {
                    PointOfInteractionAddress = "172.16.55.151",
                    Address = "172.16.55.18",
                    ApplicationSender = "IPT Petrotec",
                    DeviceProxyPort = 45000,
                    SerialNumber = "017002",
                    WorkstationId = "1",
                    TerminalId = "900",
                    AllowedAcquirerIds = new List<int> { 9200, 9202 },
                    Type = "IPT_PETROTEC"
                },
                new EpsPosClient
                {
                    PointOfInteractionAddress = "127.0.0.1",
                    Address = "127.0.0.1",
                    ApplicationSender = "OPT Petrotec",
                    DeviceProxyPort = 20101,
                    SerialNumber = "017002",
                    WorkstationId = "1",
                    TerminalId = "901",
                    AllowedAcquirerIds = new List<int> { 9200, 9202 },
                    Type = "OPT_PETROTEC"
                }
            },
                InstanceId = "7ad5fc5f-8bce-489e-b8ec-d8e83682502d",
                InstanceName = "PetrotecIfsfEps",
                CountryId = "PT",
                Acquirers = new List<EpsAcquirer>
            {
                new EpsAcquirer
                {
                    Description = "SIBS",
                    Type = "Banking",
                    MerchantId = string.Empty,
                    Id = 9200,
                    IssuerIdentifierRangeList = new List<IssuerRange>
                    {
                        new IssuerRange { First = 454702, Last = 454705, AllowDiscount = false, RebateLabel = new Dictionary<EpsMessageLanguage, string>() }
                    },
                    ServiceAddress = string.Empty,
                    ServicePort = 3321,
                    ForceRequestPin = true,
                    ForceRequestOdometer = false,
                    ForceRequestRegistration = false,
                    ForceRequestDriverId = false,
                    CanPerformDiscountOperation = true,
                    CanPerformLoyaltyOperation = true,
                    ConnectTimeoutSeconds = 5,
                    ReadTimeoutSeconds = 30,
                    WriteTimeoutSeconds = 5,
                    MaxTransactionValue = 150,
                    AllowedCardTypes = new List<CardTypes> { CardTypes.Icc },
                    MessagesList = new List<EpsMessagesConfig>(),
                    ReceiptLabels = new List<EpsReceiptLabelsConfig>()
                },
                new EpsAcquirer
                {
                    Description = "Galp WMCard",
                    Type = "GALP",
                    MerchantId = "92004900",
                    Id = 9202,
                    IssuerIdentifierRangeList = new List<IssuerRange>
                    {
                        new IssuerRange { First = 708257, Last = 708257, AllowDiscount = true, RebateLabel = new Dictionary<EpsMessageLanguage, string>{{EpsMessageLanguage.En,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Pt,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Es,"DESC. galpfrota BUSINESS"}} },
                        new IssuerRange { First = 700582, Last = 700582, AllowDiscount = true, RebateLabel = new Dictionary<EpsMessageLanguage, string>{{EpsMessageLanguage.En,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Pt,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Es,"DESC. galpfrota BUSINESS"}} },
                        new IssuerRange { First = 708276, Last = 708276, AllowDiscount = true, RebateLabel = new Dictionary<EpsMessageLanguage, string>{{EpsMessageLanguage.En,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Pt,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Es,"DESC. galpfrota BUSINESS"}} },
                        new IssuerRange { First = 708417, Last = 708417, AllowDiscount = false, RebateLabel = new Dictionary<EpsMessageLanguage, string>{{EpsMessageLanguage.En,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Pt,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Es,"DESC. galpfrota BUSINESS"}} },
                        new IssuerRange { First = 708422, Last = 708422, AllowDiscount = true, RebateLabel = new Dictionary<EpsMessageLanguage, string>{{EpsMessageLanguage.En,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Pt,"DESC. galpfrota BUSINESS"},{EpsMessageLanguage.Es,"DESC. galpfrota BUSINESS"}} }
                    },
                    ServiceAddress = "127.0.0.1",
                    ServicePort = 3321,
                    ForceRequestPin = true,
                    ForceRequestOdometer = false,
                    ForceRequestRegistration = false,
                    ForceRequestDriverId = false,
                    CanPerformDiscountOperation = false,
                    CanPerformLoyaltyOperation = false,
                    ConnectTimeoutSeconds = 5,
                    ReadTimeoutSeconds = 30,
                    WriteTimeoutSeconds = 5,
                    MaxTransactionValue = 9999,
                    AllowedCardTypes = new List<CardTypes> { CardTypes.Contactless },
                    MessagesList = new List<EpsMessagesConfig>
                    {
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertPin, Language = EpsMessageLanguage.Pt, Label = "DIGITE CÓDIGO SECRETO" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertPin, Language = EpsMessageLanguage.En, Label = "INSERT PIN" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertPin, Language = EpsMessageLanguage.Es, Label = "INSERTAR PIN" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgWait, Language = EpsMessageLanguage.Pt, Label = "AGUARDE POR FAVOR" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgWait, Language = EpsMessageLanguage.En, Label = "PLEASE WAIT" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgWait, Language = EpsMessageLanguage.Es, Label = "ESPERA POR FAVOR" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertKm, Language = EpsMessageLanguage.Pt, Label = "DIGITE OS QUILÓMETROS" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertKm, Language = EpsMessageLanguage.En, Label = "INSERT MILEAGE" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertKm, Language = EpsMessageLanguage.Es, Label = "INSERTAR KILOMETRAJE" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertIdCode, Language = EpsMessageLanguage.Pt, Label = "DIGITE DENTIFICAÇÃO" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertIdCode, Language = EpsMessageLanguage.En, Label = "INSERT ID" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgInsertIdCode, Language = EpsMessageLanguage.Es, Label = "INSERTAR ID" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgDiscountAmountApproved, Language = EpsMessageLanguage.Pt, Label = "APROVADO DESCONTO DE {AMOUNT} {CURRENCY}" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgDiscountAmountApproved, Language = EpsMessageLanguage.En, Label = "APPROVED DISCOUNT {AMOUNT} {CURRENCY}" },
                        new EpsMessagesConfig { EpsMessageIdentifier = EpsMessageIdentifier.MsgDiscountAmountApproved, Language = EpsMessageLanguage.Es, Label = "DESCUENTO APROBADO {AMOUNT} {CURRENCY}" }
                    },
                    ReceiptLabels = new List<EpsReceiptLabelsConfig>
                    {
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblGalpFrota, Language = EpsMessageLanguage.Pt, Label = "GALP FROTA" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblGalpDiscount, Language = EpsMessageLanguage.Pt, Label = "DESCONTO GALP" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblTerminal, Language = EpsMessageLanguage.Pt, Label = "Terminal Id." },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblSession, Language = EpsMessageLanguage.Pt, Label = "Sessão" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblOperation, Language = EpsMessageLanguage.Pt, Label = "Operação" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblCardFrota, Language = EpsMessageLanguage.Pt, Label = "Cartão Frota" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblCardDiscount, Language = EpsMessageLanguage.Pt, Label = "DESC. GALP" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblCustomer, Language = EpsMessageLanguage.Pt, Label = "Cliente" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblDriverAndLicensePlate, Language = EpsMessageLanguage.Pt, Label = "Utente/Matrícula" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblExpireDate, Language = EpsMessageLanguage.Pt, Label = "Caducidade" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblMileage, Language = EpsMessageLanguage.Pt, Label = "Quilómetros" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblGalpFrota, Language = EpsMessageLanguage.Es, Label = "GALP FLOTA" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblGalpDiscount, Language = EpsMessageLanguage.Es, Label = "DESCUENTO GALP" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblTerminal, Language = EpsMessageLanguage.Es, Label = "Terminal Id." },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblSession, Language = EpsMessageLanguage.Es, Label = "Sesion" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblOperation, Language = EpsMessageLanguage.Es, Label = "Operacion" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblCardFrota, Language = EpsMessageLanguage.Es, Label = "Tarjeta Flota" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblCardDiscount, Language = EpsMessageLanguage.Pt, Label = "DESC. GALP" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblCustomer, Language = EpsMessageLanguage.Es, Label = "Cliente" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblDriverAndLicensePlate, Language = EpsMessageLanguage.Es, Label = "Usuario/Matricula" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblExpireDate, Language = EpsMessageLanguage.Es, Label = "Caducidad" },
                        new EpsReceiptLabelsConfig { EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblMileage, Language = EpsMessageLanguage.Es, Label = "Kilómetros" }
                    }
                }
            },
                Languages = new List<EpsLanguage>
            {
                new EpsLanguage { Description = "PT", Id = 1 },
                new EpsLanguage { Description = "EN", Id = 2 }
            },
                Messages = new List<EpsMessage>
            {
                new EpsMessage { EpsMessageIdentifier = EpsMessageIdentifier.OkText, Language = EpsMessageLanguage.Pt, Label = "Confirmar" },
                new EpsMessage { EpsMessageIdentifier = EpsMessageIdentifier.CancelText, Language = EpsMessageLanguage.Pt, Label = "Cancelar" },
                new EpsMessage { EpsMessageIdentifier = EpsMessageIdentifier.OkText, Language = EpsMessageLanguage.Es, Label = "Confirme" },
                new EpsMessage { EpsMessageIdentifier = EpsMessageIdentifier.CancelText, Language = EpsMessageLanguage.Es, Label = "Cancelar" }
            },
                ServicePort = 20901,
                DefaultAuthorizationValue = 150,
                DefaultLanguage = EpsMessageLanguage.Pt
            };
            return JsonSerializer.Serialize(optConfig, options);
        }
    }
}
