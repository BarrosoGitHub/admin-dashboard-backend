using Petrotec.Opt.Data.Types.Printer;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateLocalCreditConfigurationDTO
{
    public bool? Active { get; set; }
    public int? Id { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionEs { get; set; }
    public string? DescriptionPt { get; set; }
    public int? PinTries { get; set; }
    public bool? RequestOdometer { get; set; }
    public bool? ValidateLuhn { get; set; }
    public bool? PrintAmountInReceipt { get; set; }
    public string? ExtraReceiptData { get; set; }
    public string? DescriptionFr { get; set; }
    public PrintReceiptType? PrintReceiptType { get; set; }
}