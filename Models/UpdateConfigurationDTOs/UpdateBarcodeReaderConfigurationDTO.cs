using Petrotec.Opt.Data.Types.Bcr;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateBarcodeReaderConfigurationDTO
{
    public bool? Enabled { get; set; }

    public BarcodeReaderModel? Model { get; set; }

    public string? SerialPort { get; set; }
}
