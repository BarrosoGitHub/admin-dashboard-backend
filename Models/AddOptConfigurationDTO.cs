
using Petrotec.Opt.Data.Models.Configuration.Opt;

namespace OPTConfigurator.Models;

public class AddOptConfigurationDTO
{
    public OptMainConfiguration OptMainConfiguration { get; set; }
    public PinpadConfiguration PinpadConfiguration { get; set; }
    public ForecourtControllerConfiguration FdcConfiguration { get; set; }
    public DisplayConfiguration DisplayConfiguration { get; set; }
    public PrinterConfiguration PrinterConfiguration { get; set; }
    public EpsClientConfiguration EpsClientConfiguration { get; set; }
    public ViaVerdeConfiguration ViaVerdeConfiguration { get; set; }
    public RemoteServicesConfiguration RemoteServicesConfiguration { get; set; }
    public GalpConfiguration? GalpConfiguration { get; set; }
    public RegionalSettingsConfiguration RegionalSettings { get; set; }
    public BnaConfiguration BnaConfiguration { get; set; }
    public HoIntegrationConfiguration HeadOfficeConfiguration { get; set; }
    public TimingsConfiguration TimingsConfiguration { get; set; }
    public LocalCreditConfiguration LocalCreditConfiguration { get; set; }
    public BankingCardPaymentConfiguration BankingCardPaymentConfiguration { get; set; }
    public DiscountsConfiguration DiscountsConfiguration { get; set; }
    public PrioConfiguration? PrioConfiguration { get; set; }
    public BarcodeReaderConfiguration BarcodeReaderConfiguration { get; set; }
    public IngenicoConfiguration IngenicoConfiguration { get; set; }
    public IntermarcheConfiguration? IntermarcheConfiguration { get; set; }
    public BongasConfiguration? BongasConfiguration { get; set; }
}
