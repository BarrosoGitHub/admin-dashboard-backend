namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateIntermarcheConfigurationDTO
{
    public bool? GalittEnabled { get; set; }

    public int? MaxAmount { get; set; }

    public string? ServiceAddress { get; set; }

    public string? PositionId { get; set; }

    public string? TerminalId { get; set; }
}
