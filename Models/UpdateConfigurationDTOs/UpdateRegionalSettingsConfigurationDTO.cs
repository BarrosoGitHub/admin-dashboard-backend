namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateRegionalSettingsConfigurationDTO
{
    public string? FuelUnitOfMeasure { get; set; }

    public string? Currency { get; set; }

    public string? CurrencySymbol { get; set; }

    public int? FuelVolumeDecimalPlaces { get; set; }

    public int? FuelAmountDecimalPlaces { get; set; }

    public int? FuelUnitPricesDecimalPlaces { get; set; }

    public int? SaleTotalAmountDecimalPlaces { get; set; }
}
