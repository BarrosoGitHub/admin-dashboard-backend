using OPTConfigurator.Models;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Services.Interfaces;
using FluentValidation;
using Petrotec.ZorPay.Models;

namespace EPSConfigurator.Controllers
{
    [ApiController]
    [Route("eps-configuration")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IEpsConfigurationService _configService;

        public ConfigurationController(IEpsConfigurationService configService)
        {
            _configService = configService;
        }

        [HttpGet]
        public async Task<ActionResult<EpsConfiguration>> GetConfiguration()
        {
            var result = await _configService.GetEpsConfigurationAsync();
            if (result == null)
                return NotFound("No configuration found.");

            var jsonSettings = new System.Text.Json.JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            return new JsonResult(result, jsonSettings);
        }

        [HttpPut]
        public IActionResult UpdateOptConfiguration([FromBody] EpsConfiguration config)
        {
            try
            {
                var updatedConfig = _configService.UpdateEpsConfiguration(config);
                var jsonSettings = new System.Text.Json.JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                return new JsonResult(updatedConfig, jsonSettings);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
        }

        [HttpPost("new")]
        public IActionResult AddConfiguration([FromBody] EpsConfiguration epsConfiguration)
        {
            try
            {
                var optConfiguration = _configService.SetConfigurationAsync(epsConfiguration);
                return CreatedAtAction(nameof(GetConfiguration), epsConfiguration);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
        }

        [HttpGet("template")]
        public IActionResult GetConfigurationTemplate([FromQuery] GetEpsConfigurationTemplateDTO template)
        {
            try
            {
                var optConfiguration = _configService.GetEpsConfigurationFromTemplate(template);
                var jsonSettings = new System.Text.Json.JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                return new JsonResult(optConfiguration, jsonSettings);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }
        }

        [HttpGet("is-available")]
        public ActionResult GetConfiguratorType()
        {
            return _configService.IsEpsServiceEnabled()
                ? Ok(new { available = true })
                : NotFound(new { available = false });
        }

        [HttpGet("schema")]
        public ActionResult GetEpsConfigurationSchema()
        {
            try
            {
                var schemaObject = new
                {
                    schema = new
                    {
                        registeredTerminals = new[]
                        {
                            new
                            {
                                pointOfPayments = new[]
                                {
                                    new
                                    {
                                        popId = "string",
                                        address = "string",
                                        port = 0,
                                        cardAcceptorId = "string",
                                        cardAcceptorTerminalId = "string",
                                        type = "enum (IndoorTerminal/OutdoorTerminal)",
                                        bypassMessageAuthentication = false,
                                        printReceiptOnTerminal = false,
                                        printLogoOnTerminal = false,
                                        printerLogoPath = "string",
                                        printerColumns = 0,
                                        printShiftCloseOnTerminal = false
                                    }
                                },
                                address = "string",
                                applicationSender = "string",
                                reportProgress = false,
                                deviceProxyPort = 0,
                                serialNumber = "string",
                                workstationId = "string",
                                epsClientId = "string",
                                allowedAcquirerIds = new[] { 0 },
                                macKeyBankIndex = 0,
                                pinBlockKeyBankIndex = 0,
                                dataKeyBankIndex = 0,
                                currencySymbol = "string",
                                allowOfflineAuthorization = false
                            }
                        },
                        instanceId = "string",
                        instanceName = "string",
                        countryId = "string",
                        currencyCode = "string",
                        acquirers = new[]
                        {
                            new
                            {
                                description = "string",
                                type = "string",
                                id = 0,
                                applicationId = "string",
                                merchantId = "string",
                                issuerIdentifierRangeList = new[]
                                {
                                    new
                                    {
                                        first = 0,
                                        last = 0,
                                        allowDiscount = false,
                                        rebateLabel = new Dictionary<string, string> { { "languageKey", "string" } }
                                    }
                                },
                                serviceAddress = "string",
                                servicePort = 0,
                                forceRequestPin = false,
                                forceRequestOdometer = false,
                                forceRequestRegistration = false,
                                forceRequestDriverId = false,
                                canPerformDiscountOperation = false,
                                canPerformLoyaltyOperation = false,
                                connectTimeoutSeconds = 0,
                                readTimeoutSeconds = 0,
                                writeTimeoutSeconds = 0,
                                maxTransactionValue = 0.0,
                                defaultTransactionValue = 0.0,
                                allowOfflineProcessing = false,
                                maxOfflineTransactionValue = 0.0,
                                allowedCardTypes = new[] { 0 },
                                registeredProducts = new Dictionary<string, string> { { "string", "enum (None, Petrol, Diesel, LPG)" } }
                            }
                        },
                        languages = new[]
                        {
                            new
                            {
                                description = "string",
                                id = 0
                            }
                        },
                        servicePort = 0,
                        defaultAuthorizationValue = 0.0,
                        defaultLanguage = "enum (Pt, En, Es)",
                        maxClients = 0,
                        timeWaitCheckCardPresenceInSeconds = 0,
                        timeWaitMagneticStripeDataInSeconds = 0,
                        timeWaitGetKeyboardStringDataInSeconds = 0,
                        timeWaitDisplayCustomerMessageInSeconds = 0,
                        timeWaitGetWaitCardReaderSelectionInSeconds = 0,
                        timeWaitGoToIdleInSeconds = 0,
                        timeWaitGetRfidReadInSeconds = 0,
                        offlineTransactionsBatchLimit = 0
                    }
                };

                var jsonSettings = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                };

                return new JsonResult(schemaObject, jsonSettings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        [HttpGet("enums")]
        public ActionResult GetEpsConfigurationEnums()
        {
            try
            {
                var enumsInfo = new
                {
                    DefaultLanguage = new[]
                    {
                        new { Value = 0, Name = "Pt" },
                        new { Value = 1, Name = "En" },
                        new { Value = 2, Name = "Es" }
                    },
                    AllowedCardTypes = new[]
                    {
                        new { Value = 0, Name = "Icc" },
                        new { Value = 1, Name = "MagneticStripe" },
                        new { Value = 2, Name = "Contactless" },
                        new { Value = 3, Name = "ContactlessEmv" },
                        new { Value = 4, Name = "Mifare" },
                        new { Value = 5, Name = "QrCode" }
                    },
                    PointOfPaymentsType = new[]
                    {
                        new { Value = 0, Name = "OutdoorTerminal" },
                        new { Value = 1, Name = "IndoorTerminal" }
                    },
                    RegisteredProductsFuelTypes = new[]
                    {
                        new { Value = 0, Name = "None" },
                        new { Value = 1, Name = "Petrol" },
                        new { Value = 2, Name = "Diesel" },
                        new { Value = 3, Name = "LPG" }
                    }
                };

                return Ok(enumsInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
