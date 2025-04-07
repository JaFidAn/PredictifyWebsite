using Application.DTOs.Teams;
using FluentValidation;

namespace Application.Validators.Teams;

public class TeamSeasonLeagueCreateValidator : AbstractValidator<TeamSeasonLeagueCreateDto>
{
    public TeamSeasonLeagueCreateValidator()
    {
        RuleFor(x => x.SeasonId)
            .NotEmpty().WithMessage("Season ID is required");

        RuleFor(x => x.LeagueId)
            .NotEmpty().WithMessage("League ID is required");
    }
}
