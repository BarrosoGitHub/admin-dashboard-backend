namespace OPTConfigurator.Helpers;

public static class BoardHelper
{
    public static string GetBoardType()
    {
        try
        {
            var socIdPath = "/sys/devices/soc0/soc_id";
            if (File.Exists(socIdPath))
            {
                var socIdContent = File.ReadAllText(socIdPath);
                return socIdContent.Trim().StartsWith("I.MX8", StringComparison.OrdinalIgnoreCase) ? "Toradex" : "TS7970";
            }
            else
            {
                throw new FileNotFoundException($"soc_id file not found at {socIdPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error determining board type: {ex.Message}");
            return null!;
        }
    }
}