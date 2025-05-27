namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateHoIntegrationConfigurationDTO
{
    public bool? Active { get; set; }

    public string? ConnectionString { get; set; }

    public bool? ManageCashOperation { get; set; }

    public string? ConnectionStringLegal { get; set; }
}
