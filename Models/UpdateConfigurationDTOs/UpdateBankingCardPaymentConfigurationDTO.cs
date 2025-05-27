using Petrotec.Opt.Data.Types.Printer;

namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateBankingCardPaymentConfigurationDTO
{
    public AuthTicketPrintMode? AuthorizationTicketMode { get; set; }

    public decimal? MinAuthAmount { get; set; }
}
