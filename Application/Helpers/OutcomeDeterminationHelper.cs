using Domain.Entities;
using Application.Utulity;

namespace Application.Helpers;

public static class OutcomeDeterminationHelper
{
    public static List<MatchOutcome> DetermineOutcomes(Match match, List<Outcome> outcomes)
    {
        var result = new List<MatchOutcome>();

        foreach (var outcome in outcomes)
        {
            switch (outcome.Code.ToUpper())
            {
                case SD.FT_1:
                    AddOutcomeIf(match, result, outcome, match.Team1Goals > match.Team2Goals, match.Team1Id);
                    break;

                case SD.FT_D:
                    AddOutcomeIf(match, result, outcome, match.Team1Goals == match.Team2Goals, match.Team1Id);
                    AddOutcomeIf(match, result, outcome, match.Team1Goals == match.Team2Goals, match.Team2Id);
                    break;

                case SD.FT_2:
                    AddOutcomeIf(match, result, outcome, match.Team2Goals > match.Team1Goals, match.Team2Id);
                    break;

                case SD.OVER_2_5:
                    AddOutcomeIf(match, result, outcome, (match.Team1Goals + match.Team2Goals) > 2.5, match.Team1Id);
                    AddOutcomeIf(match, result, outcome, (match.Team1Goals + match.Team2Goals) > 2.5, match.Team2Id);
                    break;

                case SD.UNDER_2_5:
                    AddOutcomeIf(match, result, outcome, (match.Team1Goals + match.Team2Goals) < 2.5, match.Team1Id);
                    AddOutcomeIf(match, result, outcome, (match.Team1Goals + match.Team2Goals) < 2.5, match.Team2Id);
                    break;
            }
        }

        return result;
    }

    private static void AddOutcomeIf(Match match, List<MatchOutcome> result, Outcome outcome, bool condition, int teamId)
    {
        if (condition)
        {
            result.Add(new MatchOutcome
            {
                MatchId = match.Id,
                OutcomeId = outcome.Id,
                TeamId = teamId
            });
        }
    }
}
