using Application.DTOs.Matches;
using FluentValidation;

namespace Application.Validators.Matches;

public class CreateMatchValidator : AbstractValidator<CreateMatchDto>
{
    public CreateMatchValidator()
    {
        RuleFor(x => x.Team1Id)
            .NotEmpty().WithMessage("Team1Id is required")
            .GreaterThan(0).WithMessage("Team1Id must be greater than zero");

        RuleFor(x => x.Team2Id)
            .NotEmpty().WithMessage("Team2Id is required")
            .GreaterThan(0).WithMessage("Team2Id must be greater than zero")
            .NotEqual(x => x.Team1Id).WithMessage("A team cannot play against itself");

        RuleFor(x => x.MatchDate)
            .NotEmpty().WithMessage("MatchDate is required");

        RuleFor(x => x.SeasonId)
            .NotEmpty().WithMessage("SeasonId is required")
            .GreaterThan(0).WithMessage("SeasonId must be greater than zero");

        RuleFor(x => x.LeagueId)
            .NotEmpty().WithMessage("LeagueId is required")
            .GreaterThan(0).WithMessage("LeagueId must be greater than zero");
    }
}
