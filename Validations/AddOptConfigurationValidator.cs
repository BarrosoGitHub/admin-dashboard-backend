using OPTConfigurator.Models;
using FluentValidation;
using Petrotec.Opt.Data.Types.Opt;

namespace OPTConfigurator.Validations;

public class AddOptConfigurationValidator : AbstractValidator<AddOptConfigurationDTO>
{
    public AddOptConfigurationValidator()
    {
        RuleFor(x => x.StationId)
            .NotNull()
            .WithMessage("StationId is required.");

        RuleFor(x => x.WorkstationId)
            .NotNull()
            .WithMessage("WorkstationId is required.");

        RuleFor(x => x.NetworkSegment)
            .NotNull()
            .WithMessage("NetworkSegment is required.");

        RuleFor(x => x.Company)
            .NotNull()
            .WithMessage("Company is required.")
            .Must(value => Enum.IsDefined(typeof(Company), value))
            .WithMessage(x => $"Company must be a valid value. Valid values: {string.Join(", ", Enum.GetNames(typeof(Company)))}");

        RuleFor(x => x.Country)
            .NotNull()
            .WithMessage("Country is required.")
            .Must(value => Enum.IsDefined(typeof(Country), value))
            .WithMessage(x => $"Country must be a valid value. Valid values: {string.Join(", ", Enum.GetNames(typeof(Country)))}");
    }
}
