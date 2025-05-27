namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateIngenicoConfigurationDTO
{
    public string? ProductConversions { get; set; }
    public int? PollingAckTimeout { get; set; }
    public bool? ActivateTeleloading { get; set; }
    public string? ProtocolVersion { get; set; }
    public Dictionary<string, string>? GradesMapping;
}
