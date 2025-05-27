namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdatePrioConfigurationDTO
{
    public string? StationId { get; set; }

    public bool? ActivateRedeMais { get; set; }

    public string? RedeMaisServerUrl { get; set; }

    public bool? ActivateEdc { get; set; }

    public string? EdcServerUrl { get; set; }
}
