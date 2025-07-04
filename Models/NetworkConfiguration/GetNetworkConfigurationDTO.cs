namespace OPTConfigurator.Models;

public class GetNetworkConfigurationDTO
{
    public string? IPAddress { get; set; }
    public string? SubnetMask { get; set; }
    public string? DefaultGateway { get; set; }
    public bool? IsDhcpEnabled { get; set; }
    public string? NtpAddress { get; set; } // Renamed from NtpAdress
    public bool? NtpActive { get; set; } // Indicates if NTP is active
    public bool? Active { get; set; } // Added for network connection active state
    public string? PrimaryDns { get; set; }
    public string? SecondaryDns { get; set; }
}
