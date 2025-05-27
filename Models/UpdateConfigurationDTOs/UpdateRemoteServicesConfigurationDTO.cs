namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateRemoteServicesConfigurationDTO
{
    public string? NotificationServerUri { get; set; }

    public bool? NotificationServerEnabled { get; set; }

    public string? AlarmServerUri { get; set; }

    public bool? AlarmServerEnabled { get; set; }

    public bool? EnableCompression { get; set; }
}
