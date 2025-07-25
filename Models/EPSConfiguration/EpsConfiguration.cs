using OPTConfigurator.Types;

namespace OPTConfigurator.Models
{
    public class EpsPosClient
    {
        public string? PointOfInteractionAddress { get; set; }
        public string? Address { get; set; }
        public string? ApplicationSender { get; set; }
        public int DeviceProxyPort { get; set; }
        public string? SerialNumber { get; set; }
        public string? WorkstationId { get; set; }
        public string? TerminalId { get; set; }
        public List<int>? AllowedAcquirerIds { get; set; }

        public string? Type { get; set; }
    }

    public class EpsAcquirer
    {
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int Id { get; set; }
        public string? ApplicationId { get; set; }
        public List<IssuerRange>? IssuerIdentifierRangeList { get; set; }
        public string? MerchantId { get; set; }
        public string? ServiceAddress { get; set; }
        public int ServicePort { get; set; }
        public bool ForceRequestPin { get; set; }
        public bool ForceRequestOdometer { get; set; }
        public bool ForceRequestRegistration { get; set; }
        public bool ForceRequestDriverId { get; set; }
        public bool CanPerformDiscountOperation { get; set; }
        public bool CanPerformLoyaltyOperation { get; set; }
        public int ConnectTimeoutSeconds { get; set; }
        public int ReadTimeoutSeconds { get; set; }
        public int WriteTimeoutSeconds { get; set; }
        public decimal? MaxTransactionValue { get; set; }
        public decimal? DefaultTransactionValue { get; set; }
        public List<CardTypes>? AllowedCardTypes { get; set; }
        public List<EpsMessagesConfig>? MessagesList { get; set; }
        public List<EpsReceiptLabelsConfig>? ReceiptLabels { get; set; }
    }

    public class EpsLanguage
    {
        public string? Description { get; set; }
        public int Id { get; set; }
    }

    public class EpsMessage
    {
        public EpsMessageIdentifier EpsMessageIdentifier { get; set; }
        public EpsMessageLanguage Language { get; set; }
        public string? Label { get; set; }
    }

    public class EpsConfiguration
    {
        public List<EpsPosClient>? RegisteredTerminals { get; set; }
        public string? InstanceId { get; set; }
        public string? InstanceName { get; set; }
        public List<EpsAcquirer>? Acquirers { get; set; }
        public List<EpsLanguage>? Languages { get; set; }
        public List<EpsMessage>? Messages { get; set; }
        public int ServicePort { get; set; }
        public string? CountryId { get; set; }
        public string CurrencyCode { get; set; } = "978";
        public decimal DefaultAuthorizationValue { get; set; } = 100;
        public EpsMessageLanguage DefaultLanguage { get; set; }

        /// <summary>
        /// Timing definitions
        /// </summary>
        public int? TimeWaitCheckCardPresenceInSeconds { get; set; }
        public int? TimeWaitMagneticStripeDataInSeconds { get; set; }
        public int? TimeWaitGetKeyboardStringDataInSeconds { get; set; }
        public int? TimeWaitDisplayCustomerMessageInSeconds { get; set; }
    }

    public class IssuerRange
    {
        public int First { get; set; }
        public int Last { get; set; }
        public bool AllowDiscount { get; set; }

        public Dictionary<EpsMessageLanguage, string> RebateLabel { get; set; } = new();
    }

    public class EpsMessagesConfig
    {
        public EpsMessageIdentifier EpsMessageIdentifier { get; set; }
        public EpsMessageLanguage Language { get; set; }
        public string? Label { get; set; }
    }

    public class EpsReceiptLabelsConfig
    {
        public EpsReceiptLabelIdentifier EpsReceiptLabelIdentifier { get; set; }
        public EpsMessageLanguage Language { get; set; }
        public string? Label { get; set; }
    }
}
