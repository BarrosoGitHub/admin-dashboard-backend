using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using OPTConfigurator.Helpers;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfigurator.Services;

public class TSNetworkConfigurationService : INetworkConfigurationService
{
    public async Task UpdateNetworkConfigurationAsync(UpdateNetworkConfigurationDTO configuration)
    {
        string configFile = "/etc/systemd/network/80-wired.network";

        List<string> lines = new List<string>();
        if (File.Exists(configFile))
        {
            lines = File.ReadAllLines(configFile).ToList();
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith("[Match]") || lines[i].StartsWith("[Network]"))
            {
                continue;
            }
            else if (!string.IsNullOrEmpty(lines[i]) && lines[i].Contains("="))
            {
                var parts = lines[i].Split('=');
                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();

                if (key == "address")
                {
                    key = "Address";
                    lines[i] = $"{key}={Utils.GetIpAddressWithPrefix(configuration.IPAddress!, configuration.SubnetMask!) ?? value}";
                }
                else if (key == "gateway")
                {
                    key = "Gateway";
                    lines[i] = $"{key}={configuration.DefaultGateway ?? value}";
                }
                else if (key == "subnetmask")
                {
                    key = "SubnetMask";
                    lines[i] = $"{key}={configuration.SubnetMask ?? value}";
                }
                else if (key == "dhcp")
                {
                    key = "DHCP";
                    lines[i] = $"{key}={Utils.StringToBool(configuration.IsDhcpEnabled.ToString())}";
                }
            }
        }

        await File.WriteAllLinesAsync(configFile, lines);
    }

    public async Task<GetNetworkConfigurationDTO> GetNetworkConfigurationAsync()
    {
        GetNetworkConfigurationDTO result = new GetNetworkConfigurationDTO();

        string configFile = "/etc/systemd/network/80-wired.network";
        if (File.Exists(configFile))
        {
            string[] lines = File.ReadAllLines(configFile);
            foreach (var line in lines)
            {
                if (line.StartsWith("[Match]") || line.StartsWith("[Network]"))
                {
                    continue;
                }
                else if (!string.IsNullOrEmpty(line))
                {
                    if (line.Contains("="))
                    {
                        var parts = line.Split('=');
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        switch (key.ToLower())
                        {
                            case "address":
                                result.SubnetMask = Utils.GetSubnetMaskFromIp(value);
                                result.IPAddress = Regex.Replace(value, "/.*$", "");
                                break;
                            case "gateway":
                                result.DefaultGateway = value;
                                break;
                            case "dhcp":
                                result.IsDhcpEnabled = Utils.StringToBool(value);
                                break;
                        }
                    }
                }
            }
        }
        return await Task.FromResult(result);
    }
}
