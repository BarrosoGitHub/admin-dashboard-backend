namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateGalpConfigurationDTO
{
    public bool? ActivateB2B { get; set; }

    public bool? ActivateFast { get; set; }

    public bool? ActivateFastMagnetic { get; set; }

    public bool? ActivateGalpFrota { get; set; }

    public bool? ActivateCContinente { get; set; }

    public bool? AllowCContinenteMobileFallback { get; set; }

    public string? LocalServicesEndpointsList { get; set; }

    public bool? ActivateLidlCard { get; set; }

    public bool? ConvertLidlCardToGfb { get; set; }

    public bool? AllowLidlCardManualInput { get; set; }

    public int? ContinenteRetriesLimit { get; set; }

    public bool? EnableBankingPaymentDisclaimer { get; set; }

    public string? ServicesEndpoint { get; set; }

    public string? ServicesAuthToken { get; set; }

    public string? OperatorExternalId { get; set; }

    public string? TouchPointExternalId { get; set; }

    public string? BusinessUnitExternalId { get; set; }

    public List<string>? KnownFrotaBins { get; set; }

    public bool? EnableViaT { get; set; }
}
