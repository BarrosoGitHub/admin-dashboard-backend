using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using FluentValidation;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;
using Petrotec.Opt.Data.Models.Configuration.Opt;
using Petrotec.Opt.Data.Types.Opt;
using Petrotec.Opt.Data.Types.Ped;

namespace OPTConfigurator.Services;

public class OptConfigurationService : IOptConfigurationService
{
    private readonly IValidator<GetOptConfigurationTemplateDTO> _getOptConfigurationTemplateValidator;
    private readonly IValidator<UpdateOptConfigurationDTO> _updateOptConfigurationValidator;

    public OptConfigurationService(
        IValidator<GetOptConfigurationTemplateDTO> addOptConfigurationValidator,
        IValidator<UpdateOptConfigurationDTO> updateOptConfigurationValidator)
    {
        _getOptConfigurationTemplateValidator = addOptConfigurationValidator;
        _updateOptConfigurationValidator = updateOptConfigurationValidator;
    }

    public async Task<OptConfiguration> GetOptConfigurationAsync()
    {
        Console.WriteLine("[GetOptConfigurationAsync] Starting configuration retrieval");
        string filePath = Path.Combine(AppContext.BaseDirectory, "files", "opt-config.json");
        Console.WriteLine($"[GetOptConfigurationAsync] Configuration file path: {filePath}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine("[GetOptConfigurationAsync] Configuration file does not exist");
            return null!;
        }

        Console.WriteLine("[GetOptConfigurationAsync] Configuration file found, setting up JSON options");
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = { new StringEnumConverter() }
        };

        Console.WriteLine("[GetOptConfigurationAsync] Reading configuration file");
        string configJson = await File.ReadAllTextAsync(filePath);
        Console.WriteLine($"[GetOptConfigurationAsync] Configuration JSON length: {configJson.Length} characters");
        
        Console.WriteLine("[GetOptConfigurationAsync] Attempting to deserialize configuration");
        var result = JsonConvert.DeserializeObject<OptConfiguration>(configJson, settings)!;
        Console.WriteLine("[GetOptConfigurationAsync] Configuration deserialized successfully");
        return result;
    }

    public OptConfiguration GetOptConfigurationFromTemplate(GetOptConfigurationTemplateDTO optConfigTemplate)
    {
        var validationResult = _getOptConfigurationTemplateValidator.Validate(optConfigTemplate);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        OptConfiguration optConfig = ConvertJsonToOptConfiguration(CreateOptConfigurationTemplate(optConfigTemplate));

        return optConfig;
    }

    public AddOptConfigurationDTO AddOptConfiguration(AddOptConfigurationDTO optConfig)
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files", "opt-config.json");

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        string optConfigJson = JsonConvert.SerializeObject(optConfig, settings);

        File.WriteAllText(filePath, optConfigJson);

        return optConfig;
    }

    public OptConfiguration UpdateOptConfiguration(UpdateOptConfigurationDTO updateOptConfig)
    {
        var validationResult = _updateOptConfigurationValidator.Validate(updateOptConfig);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        string filePath = Path.Combine(AppContext.BaseDirectory, "files", "opt-config.json");

        OptConfiguration? existingConfig = null;
        if (File.Exists(filePath))
        {
            var existingJson = File.ReadAllText(filePath);
            existingConfig = ConvertJsonToOptConfiguration(existingJson);
        }
        else
        {
            existingConfig = new OptConfiguration();
        }

        // Use reflection to update only non-null properties
        var dtoProps = typeof(UpdateOptConfigurationDTO).GetProperties();
        var configProps = typeof(OptConfiguration).GetProperties();

        foreach (var dtoProp in dtoProps)
        {
            var value = dtoProp.GetValue(updateOptConfig);
            if (value != null)
            {
                var configProp = configProps.FirstOrDefault(p => p.Name == dtoProp.Name);
                if (configProp != null && configProp.CanWrite)
                {
                    if (configProp.PropertyType.IsClass && configProp.PropertyType != typeof(string))
                    {
                        var configSubValue = configProp.GetValue(existingConfig);
                        if (configSubValue == null)
                        {
                            configSubValue = Activator.CreateInstance(configProp.PropertyType);
                            configProp.SetValue(existingConfig, configSubValue);
                        }

                        var subProps = dtoProp.PropertyType.GetProperties();
                        foreach (var subProp in subProps)
                        {
                            var subValue = subProp.GetValue(value);
                            if (subValue != null)
                            {
                                var configSubProp = configProp.PropertyType.GetProperty(subProp.Name);
                                if (configSubProp != null && configSubProp.CanWrite)
                                {
                                    configSubProp.SetValue(configSubValue, subValue);
                                }
                            }
                        }
                    }
                    else
                    {
                        configProp.SetValue(existingConfig, value);
                    }
                }
            }
        }

        // Save updated configuration
        string updatedJson = JsonConvert.SerializeObject(existingConfig, new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        });

        File.WriteAllText(filePath, updatedJson);

        return existingConfig;
    }

    private OptConfiguration ConvertJsonToOptConfiguration(string json)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Converters = { new StringEnumConverter() }
        };

        return JsonConvert.DeserializeObject<OptConfiguration>(json, settings)
               ?? throw new InvalidOperationException("Failed to deserialize configuration.");
    }

    private string CreateOptConfigurationTemplate(GetOptConfigurationTemplateDTO addOptConfigurationDTO)
    {
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        var company = Enum.Parse<Company>(addOptConfigurationDTO.Company);
        var countryCode = Enum.Parse<Country>(addOptConfigurationDTO.Country);

        string networkSegment;
        string pinpadHostname = string.Empty;
        string fdcHostname = string.Empty;
        if (addOptConfigurationDTO.NetworkSegment.Contains("."))
        {
            var pinpadParts = addOptConfigurationDTO.NetworkSegment.Split('.');
            var fdcParts = addOptConfigurationDTO.NetworkSegment.Split('.');
            if (pinpadParts.Length == 4 && int.TryParse(pinpadParts[3], out int lastOctet))
            {
                pinpadParts[3] = (lastOctet + 20 + addOptConfigurationDTO.WorkstationId).ToString();
                fdcParts[3] = (lastOctet + 5).ToString();

                networkSegment = string.Join('.', pinpadParts);
                pinpadHostname = string.Join('.', pinpadParts);
                fdcHostname = string.Join('.', fdcParts);
            }
            else
            {
                throw new FormatException("NetworkSegment must be a valid IPv4 address or integer.");
            }
        }
        else
        {
            networkSegment = (int.Parse(addOptConfigurationDTO.NetworkSegment) + 20).ToString();
            pinpadHostname = networkSegment;
        }

        Language primaryLanguage, secondaryLanguage, tertiaryLanguage, languageN4;

        if (countryCode == Country.PT)
        {
            primaryLanguage = Language.Portuguese;
            secondaryLanguage = Language.Spanish;
            tertiaryLanguage = Language.English;
            languageN4 = Language.French;
        }
        else
        {
            primaryLanguage = Language.Spanish;
            secondaryLanguage = Language.Catalan;
            tertiaryLanguage = Language.French;
            languageN4 = Language.English;
        }

        var optConfig = new OptConfiguration
        {
            OptMainConfiguration = new OptMainConfiguration
            {
                StationId = addOptConfigurationDTO.StationId!,
                WorkstationId = addOptConfigurationDTO.WorkstationId,
                Company = company,
                CountryCode = countryCode,
                PrimaryLanguage = primaryLanguage,
                SecondaryLanguage = secondaryLanguage,
                TertiaryLanguage = tertiaryLanguage,
                LanguageN4 = languageN4
            },
            PinpadConfiguration = new PinpadConfiguration
            {
                PedModel = Enum.Parse<Country>(addOptConfigurationDTO.Country) == Country.PT ? PedModel.Verifone : PedModel.Ingenico,
                HostName = pinpadHostname
            },
            FdcConfiguration = new ForecourtControllerConfiguration
            {
                EptId = addOptConfigurationDTO.WorkstationId,
                HostName = fdcHostname,
                AssignedPumps = new List<int>()
            },
            DisplayConfiguration = new DisplayConfiguration(),
            PrinterConfiguration = new PrinterConfiguration(),
            EpsClientConfiguration = new EpsClientConfiguration(),
            ViaVerdeConfiguration = new ViaVerdeConfiguration(),
            RemoteServicesConfiguration = new RemoteServicesConfiguration(),
            RegionalSettings = new RegionalSettingsConfiguration(),
            BnaConfiguration = new BnaConfiguration(),
            HeadOfficeConfiguration = new HoIntegrationConfiguration(),
            TimingsConfiguration = new TimingsConfiguration(),
            LocalCreditConfiguration = new LocalCreditConfiguration(),
            BankingCardPaymentConfiguration = new BankingCardPaymentConfiguration(),
            DiscountsConfiguration = new DiscountsConfiguration(),
            BarcodeReaderConfiguration = new BarcodeReaderConfiguration(),
            IngenicoConfiguration = new IngenicoConfiguration()
        };

        // Set company-specific configuration, others to null
        switch (company)
        {
            case Company.Prio:
                optConfig.PrioConfiguration = new PrioConfiguration();
                optConfig.GalpConfiguration = null;
                optConfig.BongasConfiguration = null;
                optConfig.IntermarcheConfiguration = null;
                break;
            case Company.Galp:
                optConfig.GalpConfiguration = new GalpConfiguration();
                optConfig.PrioConfiguration = null;
                optConfig.BongasConfiguration = null;
                optConfig.IntermarcheConfiguration = null;
                break;
            case Company.Intermarche:
                optConfig.IntermarcheConfiguration = new IntermarcheConfiguration();
                optConfig.PrioConfiguration = null;
                optConfig.BongasConfiguration = null;
                optConfig.GalpConfiguration = null;
                break;
            case Company.Bongas:
                optConfig.BongasConfiguration = new BongasConfiguration();
                optConfig.PrioConfiguration = null;
                optConfig.IntermarcheConfiguration = null;
                optConfig.GalpConfiguration = null;
                break;
            default:
                optConfig.PrioConfiguration = null;
                optConfig.GalpConfiguration = null;
                optConfig.BongasConfiguration = null;
                optConfig.IntermarcheConfiguration = null;
                break;
        }

        return JsonConvert.SerializeObject(optConfig, settings);
    }

    public bool IsOptServiceEnabled()
    {
        return true;
    }

    public bool ToggleTechMode()
    {
        if (IsTechModeEnabled())
        {
            try
            {
                File.Delete(Path.Combine(AppContext.BaseDirectory, "files", "tech-mode.flag"));
                ApplicationState.SetRebooting();
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    // SystemUtils.Reboot();
                    ApplicationState.ResetState();

                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        else
        {
            try
            {
                string filesFolder = Path.Combine(AppContext.BaseDirectory, "files");
                if (!Directory.Exists(filesFolder))
                {
                    Directory.CreateDirectory(filesFolder);
                }

                string flagPath = Path.Combine(AppContext.BaseDirectory, "files", "tech-mode.flag");

                using (var fs = new FileStream(flagPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                }

                // Set Unix file permissions (chmod 666) - works on .NET 6+ on Linux/Unix
                if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
                {
                    File.SetUnixFileMode(flagPath,
                        UnixFileMode.UserRead | UnixFileMode.UserWrite |
                        UnixFileMode.GroupRead | UnixFileMode.GroupWrite |
                        UnixFileMode.OtherRead | UnixFileMode.OtherWrite);
                }

                ApplicationState.SetRebooting();
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    ApplicationState.ResetState();
                    // SystemUtils.Reboot();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        return true;
    }

    public bool IsTechModeEnabled()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files", "tech-mode.flag");
        return File.Exists(filePath);
    }
}