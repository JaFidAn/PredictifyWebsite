using Application.DTOs.Teams;
using FluentValidation;

namespace Application.Validators.Teams;

public class UpdateTeamValidator : AbstractValidator<UpdateTeamDto>
{
    public UpdateTeamValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Team ID is required")
            .GreaterThan(0).WithMessage("Id must be greater than zero");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required")
            .MaximumLength(150).WithMessage("Team name must not exceed 150 characters");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Country ID is required");

        RuleForEach(x => x.TeamSeasonLeagues)
            .SetValidator(new TeamSeasonLeagueCreateValidator());
    }
}
