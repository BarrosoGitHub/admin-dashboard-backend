using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace OPTConfigurator.Helpers;

public static class NetworkHelper
{
    public static string GetPrivateIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString() != "127.0.0.1")
            {
                return ip.ToString();
            }
        }
        throw new Exception("No private IPv4 address found.");
    }
    
    public static string GetSubnetMask()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c ip addr show eth0 | grep inet | awk '{ print $4}' | cut -d '/' -f 2",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string subnetMask = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return subnetMask.Trim();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get subnet mask", ex);
        }
    }

    public static string GetDefaultGateway()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c route -n | grep UG[ \t] | awk '{print $NF}'",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string gateway = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return gateway.Trim();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get default gateway", ex);
        }
    }

    public static List<string> GetDNSServers()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/cat",
                    Arguments = "/etc/resolv.conf",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string dnsContent = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var dnsServers = new List<string>();
            foreach (var line in dnsContent.Split('\n'))
            {
                if (line.StartsWith("nameserver"))
                {
                    dnsServers.Add(line.Substring(10).Trim());
                }
            }
            return dnsServers;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to get DNS servers", ex);
        }
    }

    public static bool IsDHCPActive()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c ip addr show eth0 | grep inet | awk '{ print $4}' | cut -d '/' -f 1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string ipAddress = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Simple heuristic: if the IP address looks like a private one, assume DHCP is active.
            // This is a very simplistic check and might not cover all cases.
            var ipPattern = @"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})$";
            var ipMatch = Regex.Match(ipAddress, ipPattern);
            return ipMatch.Success;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to check if DHCP is active", ex);
        }
    }
}
