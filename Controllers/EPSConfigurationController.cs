using OPTConfigurator.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OPTConfigurator.Services.Interfaces;
using FluentValidation;
using Petrotec.ZorPay.Models;
using Petrotec.ZorPay.Common.Models;
using Petrotec.ZorPay.Common.Types;

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
                // Create a sample/empty EpsConfiguration object to show the structure
                var schemaObject = new EpsConfiguration
                {
                    RegisteredTerminals = new List<EpsPosClient>
                    {
                        new EpsPosClient
                        {
                            PointOfPaymentAddress = "string",
                            Address = "string", 
                            ApplicationSender = "string",
                            DeviceProxyPort = 0,
                            SerialNumber = "string",
                            WorkstationId = "string",
                            TerminalId = "string",
                            AllowedAcquirerIds = new List<int> { 0 },
                            Type = "string"
                        }
                    },
                    InstanceId = "string",
                    InstanceName = "string",
                    CountryId = "string",
                    CurrencyCode = "978", // Default value
                    Acquirers = new List<EpsAcquirer>
                    {
                        new EpsAcquirer
                        {
                            Description = "string",
                            Type = "string",
                            Id = 0,
                            ApplicationId = "string",
                            IssuerIdentifierRangeList = new List<IssuerRange>
                            {
                                new IssuerRange 
                                { 
                                    First = 0, 
                                    Last = 0, 
                                    AllowDiscount = false,
                                    RebateLabel = new Dictionary<EpsMessageLanguage, string>
                                    {
                                        { EpsMessageLanguage.Pt, "string" },
                                        { EpsMessageLanguage.En, "string" },
                                        { EpsMessageLanguage.Es, "string" }
                                    }
                                }
                            },
                            MerchantId = "string",
                            ServiceAddress = "string", 
                            ServicePort = 0,
                            ForceRequestPin = false,
                            ForceRequestOdometer = false,
                            ForceRequestRegistration = false,
                            ForceRequestDriverId = false,
                            CanPerformDiscountOperation = false,
                            CanPerformLoyaltyOperation = false,
                            ConnectTimeoutSeconds = 0,
                            ReadTimeoutSeconds = 0,
                            WriteTimeoutSeconds = 0,
                            MaxTransactionValue = 0.0m,
                            DefaultTransactionValue = 0.0m,
                            AllowedCardTypes = new List<CardTypes>(),
                            MessagesList = new List<EpsMessagesConfig>
                            {
                                new EpsMessagesConfig
                                {
                                    EpsMessageIdentifier = EpsMessageIdentifier.MsgWait,
                                    Language = EpsMessageLanguage.Pt,
                                    Label = "string"
                                }
                            },
                            ReceiptLabels = new List<EpsReceiptLabelsConfig>
                            {
                                new EpsReceiptLabelsConfig
                                {
                                    EpsReceiptLabelIdentifier = EpsReceiptLabelIdentifier.LblTerminal,
                                    Language = EpsMessageLanguage.Pt,
                                    Label = "string"
                                }
                            }
                        }
                    },
                    Languages = new List<EpsLanguage>
                    {
                        new EpsLanguage
                        {
                            Description = "string",
                            Id = 0
                        }
                    },
                    Messages = new List<EpsMessage>
                    {
                        new EpsMessage
                        {
                            EpsMessageIdentifier = EpsMessageIdentifier.MsgWait,
                            Language = EpsMessageLanguage.Pt,
                            Label = "string"
                        }
                    },
                    ServicePort = 0,
                    DefaultAuthorizationValue = 100, // Default value
                    DefaultLanguage = EpsMessageLanguage.Pt,
                    TimeWaitCheckCardPresenceInSeconds = 30,
                    TimeWaitMagneticStripeDataInSeconds = 30,
                    TimeWaitGetKeyboardStringDataInSeconds = 30,
                    TimeWaitDisplayCustomerMessageInSeconds = 30
                };

                var jsonSettings = new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
                    Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
                };

                return new JsonResult(schemaObject, jsonSettings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }        [HttpGet("enums")]
        public ActionResult GetEpsConfigurationEnums()
        {
            try
            {
                var enumsInfo = new
                {
                    EpsMessageLanguage = Enum.GetValues<OPTConfigurator.Types.EpsMessageLanguage>()
                        .Select(e => new { Value = (int)e, Name = e.ToString() }).ToList(),
                    EpsMessageIdentifier = Enum.GetValues<OPTConfigurator.Types.EpsMessageIdentifier>()
                        .Select(e => new { Value = (int)e, Name = e.ToString() }).ToList(),
                    EpsReceiptLabelIdentifier = Enum.GetValues<OPTConfigurator.Types.EpsReceiptLabelIdentifier>()
                        .Select(e => new { Value = (int)e, Name = e.ToString() }).ToList(),
                    CardTypes = Enum.GetValues<OPTConfigurator.Types.CardTypes>()
                        .Select(e => new { Value = (int)e, Name = e.ToString() }).ToList()
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
