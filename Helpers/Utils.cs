using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace OPTConfigurator.Helpers;
public class Utils
{
    public static bool StringToBool(string value)
    {
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ? true : false;
    }

    public static string GetSubnetMaskFromIp(string ipWithPrefix)
    {
        string[] parts = ipWithPrefix.Split('/');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid input format. Expected format: IP/PrefixLength");
        }
        int prefixLength;

        if (!int.TryParse(parts[1], out prefixLength) || prefixLength < 0 || prefixLength > 32)
        {
            throw new ArgumentException("Invalid prefix length. Must be between 0 and 32.");
        }

        uint mask = 0xFFFFFFFF << (32 - prefixLength);
        byte[] maskBytes = BitConverter.GetBytes(mask);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(maskBytes);
        }

        return new IPAddress(maskBytes).ToString();
    }

    public static string GetIpAddressWithPrefix(string ipAddress, string subnetMask)
    {
        if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(subnetMask))
        {
            return $"Invalid IP address or subnet mask.";
        }

        try
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            IPAddress netMask = IPAddress.Parse(subnetMask);

            int prefixLength = ip.AddressFamily == AddressFamily.InterNetworkV6 ? 128 : 32;

            ulong maskLong = BitConverter.ToUInt64(BitConverter.GetBytes(netMask.Address), 0);

            int numberOfLeadingZeros = (int)(Math.Log(maskLong, 2) + 1);

            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                numberOfLeadingZeros -= 96;
            }

            string ipWithPrefix = $"{ip}/{numberOfLeadingZeros}";

            return ipWithPrefix;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing IP address or subnet mask: {ex.Message}");
            return $"Failed to parse IP address or subnet mask due to error: {ex.Message}";
        }
    }

    public static async Task ScheduleNetworkRebootAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        await ReloadNetworkInterfaceConfigAsync("network0");
    }

    private static async Task ReloadNetworkInterfaceConfigAsync(string interfaceName)
    {
        string[] commands = new string[]
        {
            $"nmcli connection down {interfaceName}",
            $"nmcli connection up {interfaceName}"
        };

        foreach (var command in commands)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to reload network interface {interfaceName}: {ex.Message}");
            }
        }
    }
}
