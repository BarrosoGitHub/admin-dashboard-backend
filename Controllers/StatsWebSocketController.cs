using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace OPTConfiguration.API.Application.Controllers;

[ApiController]
[Route("ws/stats")]
public class StatsWebSocketController : ControllerBase
{
    [HttpGet]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await SendStats(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }

    private async Task SendStats(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var stats = new
            {
                cpuTemperatures = GetCpuTemperatures(),
                ramUsage = GetRamUsage(),
                diskSpace = GetDiskSpace()
            };
            var json = JsonSerializer.Serialize(stats);
            var bytes = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            await Task.Delay(3000);
        }
    }

    private Dictionary<string, double?> GetCpuTemperatures()
    {
        var result = new Dictionary<string, double?>();
        try
        {
            var zones = System.IO.Directory.GetDirectories("/sys/class/thermal", "thermal_zone*");
            foreach (var zoneDir in zones)
            {
                string labelPath = System.IO.Path.Combine(zoneDir, "type");
                string tempPath = System.IO.Path.Combine(zoneDir, "temp");
                string label = System.IO.File.Exists(labelPath) ? System.IO.File.ReadAllText(labelPath).Trim() : System.IO.Path.GetFileName(zoneDir);
                double? value = null;
                if (System.IO.File.Exists(tempPath))
                {
                    string tempStr = System.IO.File.ReadAllText(tempPath).Trim();
                    if (int.TryParse(tempStr, out int temp))
                    {
                        value = temp / 1000.0;
                    }
                }
                // Simplify label: only keep cpu0, cpu1, gpu0, gpu1, drc0, pmic0, etc.
                string simpleLabel = label.Split('-')[0];
                result[simpleLabel] = value;
            }
        }
        catch
        {
            // log or ignore
        }
        return result;
    }

    private object GetRamUsage()
    {
        try
        {
            var memInfo = System.IO.File.ReadAllLines("/proc/meminfo");
            long totalKb = 0, freeKb = 0, availKb = 0, bufferKb = 0, cacheKb = 0;
            foreach (var line in memInfo)
            {
                if (line.StartsWith("MemTotal:")) totalKb = ParseKb(line);
                else if (line.StartsWith("MemFree:")) freeKb = ParseKb(line);
                else if (line.StartsWith("MemAvailable:")) availKb = ParseKb(line);
                else if (line.StartsWith("Buffers:")) bufferKb = ParseKb(line);
                else if (line.StartsWith("Cached:")) cacheKb = ParseKb(line);
            }
            return new
            {
                total = Math.Round(totalKb / 1024.0, 1),
                free = Math.Round(freeKb / 1024.0, 1),
                avail = Math.Round(availKb / 1024.0, 1),
                buffer = Math.Round(bufferKb / 1024.0, 1),
                cache = Math.Round(cacheKb / 1024.0, 1),
                load = Math.Round((totalKb - freeKb - bufferKb - cacheKb) / 1024.0, 1)
            };
        }
        catch
        {
            return new { };
        }
        static long ParseKb(string line)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return long.TryParse(parts[1], out var value) ? value : 0;
        }
    }


    private object GetDiskSpace()
    {
        try
        {
            var drives = new List<object>();
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = "-c df -k --output=source,size,used,avail,pcent,target | tail -n +2",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (var process = System.Diagnostics.Process.Start(psi))
            {
                if (process != null)
                {
                    string? line;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 6 && parts[0].StartsWith("/dev/"))
                        {
                            long size = long.TryParse(parts[1], out var sz) ? sz * 1024 : 0L;
                            long used = long.TryParse(parts[2], out var us) ? us * 1024 : 0L;
                            double percentUsed = size > 0 ? (double)used / size * 100 : 0;
                            drives.Add(new
                            {
                                filesystem = parts[0],
                                total = size,
                                used = used,
                                percentUsed = percentUsed,
                                mount = parts[5]
                            });
                        }
                    }
                }
            }
            return drives;
        }
        catch
        {
            return new object[0];
        }
    }
}
