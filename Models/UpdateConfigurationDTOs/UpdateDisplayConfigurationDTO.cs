namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateDisplayConfigurationDTO
{
    public int? DisplayWidth { get; set; }

    public int? DisplayHeight { get; set; }

    public string? Video { get; set; }

    public string? GuiServerServicesUrl { get; set; }

    public int? GuiServicesLocalPort { get; set; }
}
