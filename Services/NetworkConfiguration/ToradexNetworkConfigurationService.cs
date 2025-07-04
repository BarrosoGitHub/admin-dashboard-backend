using OPTConfigurator.Helpers;
using OPTConfigurator.Services.Interfaces;
using System.Diagnostics;
using OPTConfigurator.Models;

namespace OPTConfigurator.Services;

public class ToradexNetworkConfigurationService : INetworkConfigurationService
{
    public async Task<bool> UpdateNetworkConfigurationAsync(UpdateNetworkConfigurationDTO configuration)
    {
        if (configuration == null || string.IsNullOrEmpty(configuration.IPAddress))
        {
            return false;
        }

        string nmcliSetCommand = $"/usr/bin/nmcli connection modify \"network0\"";
        if (!configuration.IsDhcpEnabled)
        {
            nmcliSetCommand += $" ipv4.method manual";
            nmcliSetCommand += $" ipv4.addresses \"{Utils.GetIpAddressWithPrefix(configuration.IPAddress, configuration.SubnetMask!)}\"";
            nmcliSetCommand += $" ipv4.gateway \"{configuration.DefaultGateway}\"";
            
            if (!string.IsNullOrEmpty(configuration.PrimaryDns) || !string.IsNullOrEmpty(configuration.SecondaryDns))
            {
                var dnsServers = new List<string>();
                if (!string.IsNullOrEmpty(configuration.PrimaryDns))
                    dnsServers.Add(configuration.PrimaryDns);
                if (!string.IsNullOrEmpty(configuration.SecondaryDns))
                    dnsServers.Add(configuration.SecondaryDns);
                
                nmcliSetCommand += $" ipv4.dns \"{string.Join(",", dnsServers)}\"";
            }
        }
        else
        {
            nmcliSetCommand += $" ipv4.method auto";
            nmcliSetCommand += $" ipv4.addresses \'\'";
            nmcliSetCommand += $" ipv4.gateway \'\'";
            nmcliSetCommand += $" ipv4.dns \'\'";
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
            
            if (process1.ExitCode == 0)
            {
                Console.WriteLine("Network configuration updated successfully.");
                
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Utils.ScheduleNetworkRebootAsync();
                        Console.WriteLine("Network reboot completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Network reboot failed: {ex.Message}");
                    }
                });
                
                return true;
            }
            else
            {
                Console.WriteLine($"Network configuration update failed with exit code: {process1.ExitCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to update network configuration: {ex.Message}");
            return false;
        }
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

        // Get NTP address
        try
        {
            string ntpConfPath = "/etc/ntp.conf";
            if (System.IO.File.Exists(ntpConfPath))
            {
                var ntpLines = System.IO.File.ReadAllLines(ntpConfPath);
                var ntpServerLine = ntpLines.FirstOrDefault(l => l.Trim().StartsWith("server "));
                if (ntpServerLine != null)
                {
                    result.NtpAddress = ntpServerLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);
                }
            }
            else
            {
                string timesyncdPath = "/etc/systemd/timesyncd.conf";
                if (System.IO.File.Exists(timesyncdPath))
                {
                    var timesyncdLines = System.IO.File.ReadAllLines(timesyncdPath);
                    var ntpLine = timesyncdLines.FirstOrDefault(l => l.Trim().StartsWith("NTP="));
                    if (ntpLine != null)
                    {
                        result.NtpAddress = ntpLine.Split('=', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);
                    }
                }
            }
        }
        catch { /* ignore errors */ }

        try
        {
            var ntpActiveProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"timedatectl show -p NTPSynchronized\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            ntpActiveProcess.Start();
            string ntpActiveOutput = await ntpActiveProcess.StandardOutput.ReadToEndAsync();
            await ntpActiveProcess.WaitForExitAsync();
            result.NtpActive = ntpActiveOutput.Trim().EndsWith("yes");
        }
        catch { result.NtpActive = null; }

        // Get active state
        try
        {
            var activeProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/nmcli",
                    Arguments = "-t -f GENERAL.STATE connection show network0",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            activeProcess.Start();
            string activeOutput = await activeProcess.StandardOutput.ReadToEndAsync();
            await activeProcess.WaitForExitAsync();
            // Output format: GENERAL.STATE:activated (or similar)
            result.Active = activeOutput.Contains("activated");
        }
        catch { result.Active = false; }

        try
        {
            var dnsProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/nmcli",
                    Arguments = "-t -f ipv4.dns connection show network0",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            dnsProcess.Start();
            string dnsOutput = await dnsProcess.StandardOutput.ReadToEndAsync();
            await dnsProcess.WaitForExitAsync();
            
            if (!string.IsNullOrEmpty(dnsOutput))
            {
                var dnsLine = dnsOutput.Trim();
                if (dnsLine.Contains(":"))
                {
                    var dnsPart = dnsLine.Split(':')[1].Trim();
                    if (!string.IsNullOrEmpty(dnsPart))
                    {
                        var dnsServers = dnsPart.Split(',');
                        if (dnsServers.Length > 0 && !string.IsNullOrEmpty(dnsServers[0]))
                        {
                            result.PrimaryDns = dnsServers[0].Trim();
                        }
                        if (dnsServers.Length > 1 && !string.IsNullOrEmpty(dnsServers[1]))
                        {
                            result.SecondaryDns = dnsServers[1].Trim();
                        }
                    }
                }
            }
        }
        catch { /* ignore DNS errors */ }

        return result;
    }
}
