namespace OPTConfigurator.Models.UpdateDTOs;

public class UpdateTimingsConfigurationDTO
{
    public int? AskUserIfHeWantsToInsertFiscalData { get; set; }

    public int? AskUserIfHeWantsToInsertLicensePlate { get; set; }

    public int? GenericYesNo { get; set; }

    public int? DisplayPrinterNotAvailableError { get; set; }

    public int? DisplayErrorViaVerdeInitFailure { get; set; }

    public int? DisplayErrorInsufficientFunds { get; set; }

    public int? GenericError { get; set; }

    public int? IdleWaitForCard { get; set; }

    public int? GenericWaitForInput { get; set; }

    public int? AskUserDiscountCard { get; set; }

    public int? AskUserForReceipt { get; set; }

    public int? GenericInformation { get; set; }

    public int? TransactionComplete { get; set; }

    public int? GenericOk { get; set; }

    public int? GenericReadCard { get; set; }

    public int? TrxNotAuthorized { get; set; }

    public int? GetFiscalNumber { get; set; }

    public int? GetLicensePlate { get; set; }

    public int? AccountingPeriodPrintSelection { get; set; }
}
