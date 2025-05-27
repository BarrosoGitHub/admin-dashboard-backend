namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateBongasConfigurationDTO
{
    public string? ServiceEndpoint { get; set; }

    public string? ServiceUsername { get; set; }

    public string? ServicePassword { get; set; }

    public bool? AllowDiscount { get; set; }

    public bool? EnableFleetCard { get; set; }

    public bool? EnableIntegration { get; set; }
}
