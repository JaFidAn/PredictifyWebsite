using Domain.Entities;

namespace Application.Helpers;

public static class ForecastHelper
{
    public static bool CheckIsCorrect(Match match, string predictedOutcome)
    {
        if (match == null || string.IsNullOrWhiteSpace(predictedOutcome))
        {
            return false;
        }

        var totalGoals = (match.Team1Goals ?? 0) + (match.Team2Goals ?? 0);

        switch (predictedOutcome.Trim().ToUpperInvariant())
        {
            case "WIN":
                return match.Team1Goals > match.Team2Goals;

            case "LOSE":
                return match.Team2Goals > match.Team1Goals;

            case "DRAW":
                return match.Team1Goals == match.Team2Goals;

            case "OVER 3.5 GOALS":
                return totalGoals > 3;

            case "OVER 4.5 GOALS":
                return totalGoals > 4;

            case "OVER 5.5 GOALS":
                return totalGoals > 5;

            case "OVER 6.5 GOALS":
                return totalGoals > 6;

            case "UNDER 0.5 GOALS":
                return totalGoals == 0;

            case "UNDER 1.5 GOALS":
                return totalGoals <= 1;

            default:
                return false;
        }
    }
}