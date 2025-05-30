namespace OPTConfigurator.Models;

public class UserInterfaceConfigurationDTO
{
    public string? Client { get; set; }
    public string? WebsocketServerUrl { get; set; }
    public bool ShowDebugButton { get; set; }
    public bool LoadLastOPTRequestOnStartup { get; set; }
    public bool ReloadWhenGoingToIdleScreen { get; set; }
    public bool ShowVideo { get; set; }
    public bool AxonMultimediaIframe { get; set; }
    public string? AxonMultimediaUrl { get; set; }
    public List<Dictionary<string, string>>? GradeColors { get; set; }
}