namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdatePrinterConfigurationDTO
{
    public string? PrinterServerAddr { get; set; }
    public int? PrinterServerPort { get; set; }
    public int? Cols { get; set; }
    public bool UseUsb { get; set; }
    public bool LoadLogoOnStartup { get; set; }
    public string? LogoFilePath { get; set; }
    
}
