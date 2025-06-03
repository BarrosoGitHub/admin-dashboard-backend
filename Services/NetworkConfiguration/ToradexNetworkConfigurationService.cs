using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using OPTConfigurator.Helpers;
using OPTConfigurator.Services.Interfaces;
using System.Diagnostics;
using OPTConfigurator.Models;

namespace OPTConfigurator.Services;

public class ToradexNetworkConfigurationService : INetworkConfigurationService
{
    public async Task UpdateNetworkConfigurationAsync(UpdateNetworkConfigurationDTO configuration)
    {
        if (configuration == null || string.IsNullOrEmpty(configuration.IPAddress))
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        string nmcliSetCommand = $"/usr/bin/nmcli connection modify \"network0\"";
        if (!configuration.IsDhcpEnabled)
        {
            nmcliSetCommand += $" ipv4.method manual";
            nmcliSetCommand += $" ipv4.addresses \"{Utils.GetIpAddressWithPrefix(configuration.IPAddress, configuration.SubnetMask!)}\"";
            nmcliSetCommand += $" ipv4.gateway \"{configuration.DefaultGateway}\"";
        }
        else
        {
            nmcliSetCommand += $" ipv4.method auto";
            nmcliSetCommand += $" ipv4.addresses \'\'";
            nmcliSetCommand += $" ipv4.gateway \'\'";
        }

        var process1 = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{nmcliSetCommand}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        try
        {
            process1.Start();
            await process1.WaitForExitAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update network configuration: {ex.Message}");
        }

        Utils.ScheduleNetworkRebootAsync();
    }

    public async Task<GetNetworkConfigurationDTO> GetNetworkConfigurationAsync()
    {
        GetNetworkConfigurationDTO result = new GetNetworkConfigurationDTO();

        string nmcliCommand = "connection show network0";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/nmcli",
                Arguments = nmcliCommand,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        string[] lines = output.Split('\n');
        foreach (var line in lines)
        {
            if (line.Contains("IP4.ADDRESS[1]:"))
            {
                string[] ipDetails = line.Split(':');
                if (ipDetails.Length > 1)
                {
                    string ipAddressAndSubnet = ipDetails[1].Trim();
                    result.SubnetMask = Utils.GetSubnetMaskFromIp(ipAddressAndSubnet);

                    string[] ipParts = ipAddressAndSubnet.Split('/');
                    if (ipParts.Length == 2)
                    {
                        result.IPAddress = ipParts[0].Trim();
                    }
                }
            }
            else if (line.Contains("IP4.GATEWAY:"))
            {
                string[] gatewayDetails = line.Split(':');
                if (gatewayDetails.Length > 1)
                {
                    result.DefaultGateway = gatewayDetails[1].Trim();
                }
            }
            else if (line.Contains("ipv4.method"))
            {
                string[] methodDetails = line.Split(':');
                if (methodDetails.Length > 1)
                {
                    string methodValue = methodDetails[1].Trim();
                    result.IsDhcpEnabled = methodValue.ToLower() != "manual";
                }
            }
        }
        return result;
    }
}
