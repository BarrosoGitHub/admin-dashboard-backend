namespace OPTConfigurator.Helpers;

public static class BoardHelper
{
    public static string GetBoardType()
    {
        // Default board type - can be configured via environment variable or config
        string boardType = Environment.GetEnvironmentVariable("BOARD_TYPE") ?? "Generic";
        return boardType;
    }
}
