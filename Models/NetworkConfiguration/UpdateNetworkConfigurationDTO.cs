namespace OPTConfigurator.Models;

public class UpdateNetworkConfigurationDTO
{
    public string? IPAddress { get; set; }
    public string? SubnetMask { get; set; }
    public string? DefaultGateway { get; set; }
    public bool IsDhcpEnabled { get; set; }
}
