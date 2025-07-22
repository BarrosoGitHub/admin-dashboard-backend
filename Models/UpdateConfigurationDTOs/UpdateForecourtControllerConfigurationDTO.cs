using Petrotec.Opt.Data.Types.Fcc;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateForecourtControllerConfigurationDTO
{
    public string? HostName { get; set; }

    public int? EptId { get; set; }

    public int? CountryCode { get; set; }

    public bool? ReportPrinterErrors { get; set; }

    public int? MaxSimultaneousLogins { get; set; }

    public bool? ManageEpt { get; set; }

    public List<int>? AssignedPumps { get; set; }

    public FccInterfaceTypes? ControllerType { get; set; }

    public decimal? MaxAuthorizeVolume { get; set; }

    public decimal? MaxAuthorizeAmount { get; set; }

    public bool? AllowDecimalPreset { get; set; }

    public decimal? MinPresetAmount { get; set; }

    public int? MaxFuellingTime { get; set; }

    public bool? MiIp04SpainActive { get; set; }

    public int? MaxPreAuthorizationWaitTime { get; set; }
}
