using Application.DTOs.Teams;
using FluentValidation;

namespace Application.Validators.Teams;

public class TeamSeasonLeagueCreateValidator : AbstractValidator<TeamSeasonLeagueCreateDto>
{
    public TeamSeasonLeagueCreateValidator()
    {
        RuleFor(x => x.SeasonId)
            .NotEmpty().WithMessage("Season ID is required")
            .GreaterThan(0).WithMessage("Id must be greater than zero");

        RuleFor(x => x.LeagueId)
            .NotEmpty().WithMessage("League ID is required")
            .GreaterThan(0).WithMessage("Id must be greater than zero");
    }
}
