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
        public IActionResult GetOptConfigurationTemplate([FromQuery] GetEpsConfigurationTemplateDTO template)
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
                    registeredTerminals = new[]
                    {
                        new
                        {
                            pointOfPaymentAddress = "string",
                            pointOfPaymentPort = 0,
                            address = "string",
                            applicationSender = "string",
                            reportProgress = false,
                            deviceProxyPort = 0,
                            serialNumber = "string",
                            workstationId = "string",
                            terminalId = "string",
                            cardAcceptorId = "string",
                            allowedAcquirerIds = new[] { 0 },
                            macKeyBankIndex = 0,
                            pinBlockKeyBankIndex = 0,
                            dataKeyBankIndex = 0,
                            type = "TerminalType enum value",
                            bypassMessageAuthentication = false,
                            printerColumns = 32,
                            printReceiptOnTerminal = false,
                            printLogoOnTerminal = false,
                            printerLogoPath = "string",
                            printShiftCloseOnTerminal = false,
                            currencySymbol = "EUR",
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
                            merchantId = "string",
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
                            allowedCardTypes = new[] { "CardType enum value" },
                            messagesList = new[]
                            {
                                new
                                {
                                    epsMessageIdentifier = "EpsMessageIdentifier enum value",
                                    language = "EpsMessageLanguage enum value",
                                    label = "string"
                                }
                            },
                            receiptLabels = new[]
                            {
                                new
                                {
                                    epsReceiptLabelIdentifier = "EpsReceiptLabelIdentifier enum value",
                                    language = "EpsMessageLanguage enum value",
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
                            epsMessageIdentifier = "EpsMessageIdentifier enum value",
                            language = "EpsMessageLanguage enum value",
                            label = "string"
                        }
                    },
                    servicePort = 0,
                    defaultAuthorizationValue = 0.0,
                    defaultLanguage = "EpsMessageLanguage enum value",
                    timeWaitCheckCardPresenceInSeconds = 0,
                    timeWaitMagneticStripeDataInSeconds = 0,
                    timeWaitGetKeyboardStringDataInSeconds = 0,
                    timeWaitDisplayCustomerMessageInSeconds = 0,
                    timeWaitGetWaitCardReaderSelectionInSeconds = 0,
                    timeWaitGetRfidReadInSeconds = 0
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
