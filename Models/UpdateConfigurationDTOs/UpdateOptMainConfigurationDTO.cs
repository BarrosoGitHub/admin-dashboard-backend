using Petrotec.Opt.Data.Types.Opt;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateOptMainConfigurationDTO
{
    public string? StationId { get; set; }

    public int? WorkstationId { get; set; }

    public string? ApplicationName { get; set; }

    public string? Version { get; set; }

    public OptType? OptType { get; set; }

    public bool? PreventFallback { get; set; }

    public bool? StopOnPrinterError { get; set; }

    public bool? ActivateInvoices { get; set; }

    public bool? InstanceActive { get; set; }

    public bool? ForeignClientInvoiceSimplified { get; set; }

    public int? GasoleoBGradeId { get; set; }

    public Language? PrimaryLanguage { get; set; }

    public Language? SecondaryLanguage { get; set; }

    public Language? TertiaryLanguage { get; set; }

    public Language? LanguageN4 { get; set; }

    public bool? IdleActive { get; set; }

    public Country? CountryCode { get; set; }

    public Company? Company { get; set; }

    public int? MaxDatabaseSize { get; set; }

    public bool? UnmannedEnvironment { get; set; }
}
