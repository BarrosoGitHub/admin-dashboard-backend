namespace OPTConfigurator.Types;

public enum EpsMessageLanguage
{
    Pt,
    En,
    Es
}

public enum EpsMessageIdentifier
{
    MsgInsertPin = 1,
    MsgInsertKm = 2,
    MsgInsertIdCode = 3,
    MsgCommunicationInProgress = 4,
    MsgWait = 5,
    MsgPrinting = 6,
    MsgLoadingReceipt = 7,
    MsgErrorSavingFast = 8,
    MsgTryAgain = 9,
    MsgErrorReadFast = 10,
    MsgPointsAttrib = 11,
    MsgTotalPoints = 12,
    MsgDiscountAmountApproved = 13,
    OkText = 14,
    CancelText = 15,
}

public enum EpsReceiptLabelIdentifier
{
    LblGalpFrota,
    LblGalpDiscount,
    LblTerminal,
    LblSession,
    LblOperation,
    LblCardFrota,
    LblCardDiscount,
    LblCustomer,
    LblDriverAndLicensePlate,
    LblExpireDate,
    LblMileage
}

public enum CardTypes
{
    Icc,
    MagneticStripe,
    Contactless,
    ContactlessEmv,
    Mifare
}