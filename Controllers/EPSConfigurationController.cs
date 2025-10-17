using OPTConfigurator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Services.Interfaces;
using FluentValidation;
using Petrotec.ZorPay.Models;

namespace EPSConfigurator.Controllers
{
    [ApiController]
    [Route("eps-configuration")]
    [Authorize]
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
                                        type = "string (IndoorTerminal/OutdoorTerminal)",
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
                                terminalId = "string",
                                allowedAcquirerIds = new[] { 0 },
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
                                @default = false,
                                allowedCardTypes = new[] { 0 },
                                messagesList = new[]
                                {
                                    new
                                    {
                                        epsMessageIdentifier = "string (enum)",
                                        language = "string (enum: pt, en, es)",
                                        label = "string"
                                    }
                                },
                                receiptLabels = new[]
                                {
                                    new
                                    {
                                        epsReceiptLabelIdentifier = "string (enum)",
                                        language = "string (enum: pt, en, es)",
                                        label = "string"
                                    }
                                }
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
                        messages = new[]
                        {
                            new
                            {
                                epsMessageIdentifier = "string (enum)",
                                language = "string (enum: pt, en, es)",
                                label = "string"
                            }
                        },
                        servicePort = 0,
                        defaultTransactionValue = 0.0,
                        defaultLanguage = "string (enum: Pt, En, Es)",
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
                // Return enum information as a generic structure
                var enumsInfo = new
                {
                    EpsMessageLanguage = new[]
                    {
                        new { Value = 0, Name = "Pt" },
                        new { Value = 1, Name = "En" },
                        new { Value = 2, Name = "Es" }
                    },
                    EpsMessageIdentifier = new[]
                    {
                        new { Value = 0, Name = "MsgWait" },
                        new { Value = 1, Name = "MsgInsertPin" },
                        new { Value = 2, Name = "MsgInsertKm" },
                        new { Value = 3, Name = "MsgInsertIdCode" },
                        new { Value = 4, Name = "MsgDiscountAmountApproved" },
                        new { Value = 5, Name = "OkText" },
                        new { Value = 6, Name = "CancelText" }
                    },
                    EpsReceiptLabelIdentifier = new[]
                    {
                        new { Value = 0, Name = "LblTerminal" },
                        new { Value = 1, Name = "LblSession" },
                        new { Value = 2, Name = "LblOperation" },
                        new { Value = 3, Name = "LblGalpFrota" },
                        new { Value = 4, Name = "LblGalpDiscount" },
                        new { Value = 5, Name = "LblCardFrota" },
                        new { Value = 6, Name = "LblCardDiscount" },
                        new { Value = 7, Name = "LblCustomer" },
                        new { Value = 8, Name = "LblDriverAndLicensePlate" },
                        new { Value = 9, Name = "LblExpireDate" },
                        new { Value = 10, Name = "LblMileage" }
                    },
                    CardTypes = new[]
                    {
                        new { Value = 0, Name = "Icc" },
                        new { Value = 1, Name = "Contactless" },
                        new { Value = 2, Name = "Magnetic" }
                    },
                    TerminalType = new[]
                    {
                        new { Value = 0, Name = "IPT_PETROTEC" },
                        new { Value = 1, Name = "OPT_PETROTEC" },
                        new { Value = 2, Name = "INGENICO" }
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
