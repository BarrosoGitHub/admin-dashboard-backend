using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OPTConfigurator.Models;

public class GetNetworkConfigurationDTO
{
    public string? IPAddress { get; set; }
    public string? SubnetMask { get; set; }
    public string? DefaultGateway { get; set; }
    public bool? IsDhcpEnabled { get; set; }
    public string? NtpAddress { get; set; } // Renamed from NtpAdress
    public bool? NtpActive { get; set; } // Indicates if NTP is active
}
