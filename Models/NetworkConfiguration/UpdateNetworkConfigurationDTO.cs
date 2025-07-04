namespace OPTConfigurator.Models;

public class UpdateNetworkConfigurationDTO
{
    public string? IPAddress { get; set; }
    public string? SubnetMask { get; set; }
    public string? DefaultGateway { get; set; }
    public string? NtpAddress { get; set; }
    public bool? NtpActive { get; set; }
    public bool IsDhcpEnabled { get; set; }
    public string? PrimaryDns { get; set; }
    public string? SecondaryDns { get; set; }
}
