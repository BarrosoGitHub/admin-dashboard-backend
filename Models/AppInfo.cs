using System.Reflection;

public class AppInfo
{
    public AppInfo(string uuid, string hash)
    {
        Id = uuid;
        Hash = hash;
        Version = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? "NA";
        StartUpTime = DateTime.Now;
        // Status = ApplicationStatus.Starting.ToString();
    }

    public string Id { get; set; }
    public string Hash { get; set; }
    public string Version { get; set; }
    public string Status { get; set; }
    public DateTime StartUpTime { get; set; }
    public string Message { get; set; }
} 