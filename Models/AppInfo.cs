using System.Reflection;

public class AppInfo
{
    public AppInfo() {
        Id = string.Empty;
        Hash = string.Empty;
        Version = string.Empty;
        Status = string.Empty;
        Message = string.Empty;
        StartUpTime = DateTime.Now;
    }
    public AppInfo(string uuid, string hash)
    {
        Id = uuid;
        Hash = hash;
        Version = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? "NA";
        StartUpTime = DateTime.Now;
        Status = "Starting";
        Message = string.Empty;
    }
    public string Id { get; set; }
    public string Hash { get; set; }
    public string Version { get; set; }
    public string Status { get; set; }
    public DateTime StartUpTime { get; set; }
    public string Message { get; set; }
}