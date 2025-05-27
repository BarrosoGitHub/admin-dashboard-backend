using Petrotec.Opt.Data.Types.Ped;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdatePinpadConfigurationDTO
{
    public string? HostName { get; set; }
    public int? HostPort { get; set; }
    public bool? ReadIccBin { get; set; }
    public PedModel? PedModel { get; set; }
}