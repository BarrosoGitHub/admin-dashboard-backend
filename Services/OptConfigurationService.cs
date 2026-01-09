using System.Text.Json;
using System.Text.Json.Serialization;
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
        string filePath = Path.Combine(AppContext.BaseDirectory, "files", "opt-config.json");

        if (!File.Exists(filePath))
            return null!;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        string configJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<OptConfiguration>(configJson, options)!;
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

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        string optConfigJson = JsonSerializer.Serialize(optConfig, options);

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
        string updatedJson = JsonSerializer.Serialize(existingConfig, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        File.WriteAllText(filePath, updatedJson);

        return existingConfig;
    }

    private OptConfiguration ConvertJsonToOptConfiguration(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        return JsonSerializer.Deserialize<OptConfiguration>(json, options)
               ?? throw new InvalidOperationException("Failed to deserialize configuration.");
    }

    private string CreateOptConfigurationTemplate(GetOptConfigurationTemplateDTO addOptConfigurationDTO)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)); // Add this line

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

        return JsonSerializer.Serialize(optConfig, options);
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
                string configDirectory = Path.Combine(AppContext.BaseDirectory, "files");
                Directory.CreateDirectory(configDirectory);

                string flagPath = Path.Combine(configDirectory, "tech-mode.flag");
                using var _ = new FileStream(flagPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
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