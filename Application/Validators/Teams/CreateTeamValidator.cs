using Application.DTOs.Teams;
using FluentValidation;

namespace Application.Validators.Teams;

public class CreateTeamValidator : AbstractValidator<CreateTeamDto>
{
    public CreateTeamValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MaximumLength(150).WithMessage("Team name must not exceed 150 characters");

        RuleFor(x => x.CountryId)
            .GreaterThan(0).WithMessage("Country ID must be greater than zero");

        RuleFor(x => x.LeagueId)
            .GreaterThan(0).WithMessage("League ID must be greater than zero");

        RuleFor(x => x.SeasonId)
            .GreaterThan(0).WithMessage("Season ID must be greater than zero");
    }
}
