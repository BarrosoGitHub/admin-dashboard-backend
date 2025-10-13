using System.ComponentModel;

namespace OPTConfigurator.Models
{
    public enum TimeZoneEnum
    {
        [Description("Europe/Lisbon")]
        EuropeLisbon,

        [Description("Europe/Madrid")]
        EuropeMadrid,

        [Description("Atlantic/Azores")]
        AtlanticAzores,

        [Description("Atlantic/Madeira")]
        AtlanticMadeira,

        [Description("Atlantic/Canary")]
        AtlanticCanary
    }

    public static class TimeZoneEnumExtensions
    {
        public static string ToTimeZoneId(this TimeZoneEnum timeZone)
        {
            return timeZone switch
            {
                TimeZoneEnum.EuropeLisbon => "Europe/Lisbon",
                TimeZoneEnum.EuropeMadrid => "Europe/Madrid",
                TimeZoneEnum.AtlanticAzores => "Atlantic/Azores",
                TimeZoneEnum.AtlanticMadeira => "Atlantic/Madeira",
                TimeZoneEnum.AtlanticCanary => "Atlantic/Canary",
                _ => "Europe/Lisbon"
            };
        }

        public static TimeZoneInfo GetTimeZoneInfo(this TimeZoneEnum timeZone)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(timeZone.ToTimeZoneId());
            }
            catch
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("Europe/Lisbon");
                }
                catch
                {
                    // Final fallback to UTC
                    return TimeZoneInfo.Utc;
                }
            }
        }
    }
}