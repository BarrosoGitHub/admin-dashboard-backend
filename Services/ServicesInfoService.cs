using System.Collections.Generic;
using OPTConfigurator.Models;
using OPTConfigurator.Services.Interfaces;

namespace OPTConfigurator.Services
{
    public class ServicesInfoService : IServicesInfoService
    {
        public List<AppInfo> GetAllServicesInfo()
        {
            // Mock modules for demonstration
            return new List<AppInfo>
            {
                new AppInfo("fm-footer-web", "9827b96a")
                {
                    Status = "Healthy",
                    Message = "Module 1 is operational.",
                    StartUpTime = DateTime.Now.AddMinutes(-10),
                    Version = "1.4.5.0"
                },
                new AppInfo("fm-display-web", "9827b96a")
                {
                    Status = "Stopped",
                    Message = "Module 2 stopped for maintenance.",
                    StartUpTime = DateTime.Now.AddHours(-1),
                    Version = "1.7.5.0"
                },
                new AppInfo("emc-calculator", "9827b96a")
                {
                    Status = "Healthy",
                    Message = "Module 3 is starting up.",
                    StartUpTime = DateTime.Now.AddMinutes(-2),
                    Version = "2.4.5.0"
                },
                new AppInfo("emc-display-web", "9827b96a")
                {
                    Status = "Healthy",
                    Message = "Module 1 is operational.",
                    StartUpTime = DateTime.Now.AddMinutes(-10),
                    Version = "0.0.5.0"
                },
                new AppInfo("pfs-electron", "9827b96a")
                {
                    Status = "Stopped",
                    Message = "Module 2 stopped for maintenance.",
                    StartUpTime = DateTime.Now.AddHours(-1),
                    Version = "1.1.5.0"
                }
            };
        }

        public string GetTimeZone()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "files", "services_information.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The services information file was not found.", filePath);
            }

            var jsonContent = File.ReadAllText(filePath);
            var servicesInfo = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

            if (servicesInfo != null && servicesInfo.TryGetValue("TimeZone", out var timeZone))
            {
                return timeZone;
            }
            return null!;
        }

        public string SetTimeZone(string timeZone)
        {
            if (!IsValidTimeZone(timeZone))
            {
                var validTimeZones = GetValidTimeZones();
                throw new ArgumentException($"Invalid timezone '{timeZone}'. Valid timezones are: {string.Join(", ", validTimeZones)}", nameof(timeZone));
            }

            string filePath = Path.Combine(AppContext.BaseDirectory, "files", "services_information.json");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The services information file was not found.", filePath);
            }

            var jsonContent = File.ReadAllText(filePath);
            var servicesInfo = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent) ?? new Dictionary<string, string>();

            servicesInfo["TimeZone"] = timeZone;

            var updatedJsonContent = System.Text.Json.JsonSerializer.Serialize(servicesInfo, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJsonContent);

            return timeZone;
        }

        private bool IsValidTimeZone(string timeZone)
        {
            if (string.IsNullOrWhiteSpace(timeZone))
                return false;

            var validTimeZones = Enum.GetValues<TimeZoneEnum>()
                .Select(tz => tz.ToTimeZoneId())
                .ToList();

            return validTimeZones.Contains(timeZone, StringComparer.OrdinalIgnoreCase);
        }

        public List<string> GetValidTimeZones()
        {
            return Enum.GetValues<TimeZoneEnum>()
                .Select(tz => tz.ToTimeZoneId())
                .ToList();
        }
    }
}
