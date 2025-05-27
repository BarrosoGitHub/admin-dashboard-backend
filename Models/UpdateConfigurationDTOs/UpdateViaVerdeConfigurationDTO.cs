namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateViaVerdeConfigurationDTO
{
    public bool? Active { get; set; }

    public short? PosCode { get; set; }

    public string? Side { get; set; }

    public short? CommunicationType { get; set; }

    public string? DrcIpAddress { get; set; }

    public int? DrcPort { get; set; }

    public string? ProxyIpAddress { get; set; }

    public int? ProxyPort { get; set; }
}
