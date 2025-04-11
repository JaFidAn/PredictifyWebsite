using Application.DTOs.TeamOutcomeStreaks;
using FluentValidation;

namespace Application.Validators.TeamOutcomeStreaks;

public class CreateTeamOutcomeStreakValidator : AbstractValidator<CreateTeamOutcomeStreakDto>
{
    public CreateTeamOutcomeStreakValidator()
    {
        RuleFor(x => x.TeamId)
            .GreaterThan(0).WithMessage("Team ID must be greater than 0");

        RuleFor(x => x.OutcomeId)
            .GreaterThan(0).WithMessage("Outcome ID must be greater than 0");

        RuleFor(x => x.StreakCount)
            .GreaterThanOrEqualTo(0).WithMessage("Streak count cannot be negative");

        RuleFor(x => x.MatchId)
            .GreaterThan(0).WithMessage("Match ID must be greater than 0");

        RuleFor(x => x.MatchDate)
            .NotEmpty().WithMessage("Match date is required");
    }
}
