using Petrotec.Opt.Data.Types.Bna;
using Petrotec.Opt.Data.Types.Opt;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateBnaConfigurationDTO
{
    public bool? BnaActive { get; set; }

    public BnaModel? Model { get; set; }

    public string? SerialPort { get; set; }

    public int? BaudRate { get; set; }

    public int? DataBits { get; set; }

    public int? Parity { get; set; }

    public int? StopBits { get; set; }

    public int? FlowControl { get; set; }

    public decimal? MaxValue { get; set; }

    public decimal? MinValue { get; set; }

    public string? OperatingPeriod { get; set; }

    public PrepayReceiptMode? PrepayTicketMode { get; set; }

    public CashOperationMode? CashOperationMode { get; set; }
}
