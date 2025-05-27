using OPTConfigurator.Models.UpdateDTOs;

namespace OPTConfigurator.Models;

public class UpdateOptConfigurationDTO
{
    public UpdateOptMainConfigurationDTO? OptMainConfiguration { get; set; }
    public UpdatePinpadConfigurationDTO? PinpadConfiguration { get; set; }
    public UpdateForecourtControllerConfigurationDTO? FdcConfiguration { get; set; }
    public UpdateDisplayConfigurationDTO? DisplayConfiguration { get; set; }
    public UpdatePrinterConfigurationDTO? PrinterConfiguration { get; set; }
    public UpdateEpsClientConfigurationDTO? EpsClientConfiguration { get; set; }
    public UpdateViaVerdeConfigurationDTO? ViaVerdeConfiguration { get; set; }
    public UpdateRemoteServicesConfigurationDTO? RemoteServicesConfiguration { get; set; }
    public UpdateGalpConfigurationDTO? GalpConfiguration { get; set; }
    public UpdateRegionalSettingsConfigurationDTO? RegionalSettings { get; set; }
    public UpdateBnaConfigurationDTO? BnaConfiguration { get; set; }
    public UpdateHoIntegrationConfigurationDTO? HeadOfficeConfiguration { get; set; }
    public UpdateTimingsConfigurationDTO? TimingsConfiguration { get; set; }
    public UpdateLocalCreditConfigurationDTO? LocalCreditConfiguration { get; set; }
    public UpdateBankingCardPaymentConfigurationDTO? BankingCardPaymentConfiguration { get; set; }
    public UpdateDiscountsConfigurationDTO? DiscountsConfiguration { get; set; }
    public UpdatePrioConfigurationDTO? PrioConfiguration { get; set; }
    public UpdateBarcodeReaderConfigurationDTO? BarcodeReaderConfiguration { get; set; }
    public UpdateIngenicoConfigurationDTO? IngenicoConfiguration { get; set; }
    public UpdateIntermarcheConfigurationDTO? IntermarcheConfiguration { get; set; }
    public UpdateBongasConfigurationDTO? BongasConfiguration { get; set; }
}
