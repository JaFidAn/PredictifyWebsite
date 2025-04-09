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
            .NotEmpty().WithMessage("Country ID is required")
            .GreaterThan(0).WithMessage("Id must be greater than zero");

        RuleForEach(x => x.TeamSeasonLeagues)
            .SetValidator(new TeamSeasonLeagueCreateValidator());
    }
}
