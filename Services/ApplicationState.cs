namespace OPTConfigurator.Services;


public static class ApplicationState
{
    private static readonly object _lock = new object();
    private static bool _isRebooting = false;

    public static bool IsRebooting
    {
        get
        {
            lock (_lock)
            {
                return _isRebooting;
            }
        }
    }

    public static void SetRebooting()
    {
        lock (_lock)
        {
            _isRebooting = true;
            Console.WriteLine("[ApplicationState] System state changed to: REBOOTING");
        }
    }

    public static void ResetState()
    {
        lock (_lock)
        {
            _isRebooting = false;
            Console.WriteLine("[ApplicationState] System state reset to: NORMAL");
        }
    }
}
