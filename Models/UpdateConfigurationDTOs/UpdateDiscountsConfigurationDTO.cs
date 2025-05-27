using Petrotec.Opt.Data.Types.Opt;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateDiscountsConfigurationDTO
{
    public bool? Active { get; set; }

    public string? DescriptionEn { get; set; }

    public string? DescriptionEs { get; set; }

    public string? DescriptionPt { get; set; }

    public string? ExtraReceiptData { get; set; }

    public string? DescriptionFr { get; set; }

    public DiscountApplicationMode? TypeOfDiscountTaxFormula { get; set; }
}
