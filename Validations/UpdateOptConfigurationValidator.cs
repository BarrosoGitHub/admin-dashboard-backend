using FluentValidation;
using OPTConfigurator.Models;

namespace OPTConfigurator.Validations
{
    public class UpdateOptConfigurationRequestValidator : AbstractValidator<UpdateOptConfigurationDTO>
    {
        public UpdateOptConfigurationRequestValidator()
        {
            //OptMainConfiguration
            RuleFor(x => x.OptMainConfiguration!.OptType)
                .IsInEnum()
                .When(x => x.OptMainConfiguration != null)
                .WithMessage("OptType must be a valid enum value.");
            RuleFor(x => x.OptMainConfiguration!.PrimaryLanguage)
                .IsInEnum()
                .When(x => x.OptMainConfiguration != null)
                .WithMessage("PrimaryLanguage must be a valid enum value.");
            RuleFor(x => x.OptMainConfiguration!.SecondaryLanguage)
                .IsInEnum()
                .When(x => x.OptMainConfiguration != null)
                .WithMessage("SecondaryLanguage must be a valid enum value.");
            RuleFor(x => x.OptMainConfiguration.TertiaryLanguage)
                .IsInEnum()
                .When(x => x.OptMainConfiguration != null)
                .WithMessage("TertiaryLanguage must be a valid enum value.");
            RuleFor(x => x.OptMainConfiguration.LanguageN4)
                .IsInEnum()
                .When(x => x.OptMainConfiguration != null)
                .WithMessage("LanguageN4 must be a valid enum value.");
            RuleFor(x => x.OptMainConfiguration.CountryCode)
                .IsInEnum()
                .When(x => x.OptMainConfiguration != null)
                .WithMessage("CountryCode must be a valid enum value.");
            RuleFor(x => x.OptMainConfiguration.Company)
                .IsInEnum()
                .When(x => x.OptMainConfiguration != null)
                .WithMessage("Company must be a valid enum value.");

            // PinpadConfiguration
            RuleFor(x => x.PinpadConfiguration.PedModel)
                .IsInEnum()
                .When(x => x.PinpadConfiguration != null)
                .WithMessage("PinpadType must be a valid enum value.");

            // FdcConfiguration
            RuleFor(x => x.FdcConfiguration.ControllerType)
                .IsInEnum()
                .When(x => x.FdcConfiguration != null)
                .WithMessage("ControllerType must be a valid enum value.");

            // DisplayConfiguration

            // PrinterConfiguration

            // EpsClientConfiguration

            // ViaVerdeConfiguration

            // RemoteServicesConfiguration

            // GalpConfiguration

            // RegionalSettingsConfiguration

            // BnaConfiguration
            RuleFor(x => x.BnaConfiguration.PrepayTicketMode)
                .IsInEnum()
                .When(x => x.BnaConfiguration != null)
                .WithMessage("PrepayTicketMode must be a valid enum value.");
            RuleFor(x => x.BnaConfiguration.CashOperationMode)
                .IsInEnum()
                .When(x => x.BnaConfiguration != null)
                .WithMessage("CashOperationMode must be a valid enum value.");

            // HeadOfficeConfiguration

            // TimingsConfiguration

            // LocalCreditConfiguration
            RuleFor(x => x.LocalCreditConfiguration.PrintReceiptType)
                .IsInEnum()
                .When(x => x.LocalCreditConfiguration != null)
                .WithMessage("PrintReceiptType must be a valid enum value.");

            // BankingCardPaymentConfiguration
            RuleFor(x => x.BankingCardPaymentConfiguration.AuthorizationTicketMode)
                .IsInEnum()
                .When(x => x.BankingCardPaymentConfiguration != null)
                .WithMessage("AuthorizationTicketMode must be a valid enum value.");

            // DiscountsConfiguration
            RuleFor(x => x.DiscountsConfiguration.TypeOfDiscountTaxFormula)
                .IsInEnum()
                .When(x => x.DiscountsConfiguration != null)
                .WithMessage("TypeOfDiscountTaxFormula must be a valid enum value.");

            // PrioConfiguration

            // BarcodeReaderConfiguration
            RuleFor(x => x.BarcodeReaderConfiguration.Model)
                .IsInEnum()
                .When(x => x.BarcodeReaderConfiguration != null)
                .WithMessage("BarcodeReaderModel must be a valid enum value.");

            // IngenicoConfiguration


            // IntermarcheConfiguration


            // BongasConfiguration
           
        }
    }
}