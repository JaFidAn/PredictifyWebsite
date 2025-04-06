using Application.DTOs.Leagues;
using FluentValidation;

namespace Application.Validators.Leagues;

public class UpdateLeagueValidator : AbstractValidator<UpdateLeagueDto>
{
    public UpdateLeagueValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("CountryId is required");

        RuleFor(x => x.CompetitionId)
            .NotEmpty().WithMessage("CompetitionId is required");
    }
}
