using Domain.Entities;
using Application.Utilities;

namespace Application.Helpers;

public static class OutcomeDeterminationHelper
{
    public static List<MatchOutcome> DetermineOutcomes(Match match, List<Outcome> outcomes)
    {
        var result = new List<MatchOutcome>();

        var totalGoals = (match.Team1Goals ?? 0) + (match.Team2Goals ?? 0);

        foreach (var outcome in outcomes)
        {
            switch (outcome.Code.ToUpperInvariant())
            {
                case SD.WIN:
                    if (match.Team1Goals > match.Team2Goals)
                    {
                        AddOutcome(result, match.Id, outcome.Id, match.Team1Id);
                    }
                    if (match.Team2Goals > match.Team1Goals)
                    {
                        AddOutcome(result, match.Id, outcome.Id, match.Team2Id);
                    }
                    break;

                case SD.LOSE:
                    if (match.Team1Goals < match.Team2Goals)
                    {
                        AddOutcome(result, match.Id, outcome.Id, match.Team1Id);
                    }
                    if (match.Team2Goals < match.Team1Goals)
                    {
                        AddOutcome(result, match.Id, outcome.Id, match.Team2Id);
                    }
                    break;

                case SD.DRAW:
                    if (match.Team1Goals == match.Team2Goals)
                    {
                        AddOutcome(result, match.Id, outcome.Id, match.Team1Id);
                        AddOutcome(result, match.Id, outcome.Id, match.Team2Id);
                    }
                    break;

                case SD.OVER_3_5:
                    if (totalGoals > 3)
                    {
                        AddBothTeamsOutcome(result, match.Id, outcome.Id, match.Team1Id, match.Team2Id);
                    }
                    break;

                case SD.UNDER_1_5:
                    if (totalGoals <= 1)
                    {
                        AddBothTeamsOutcome(result, match.Id, outcome.Id, match.Team1Id, match.Team2Id);
                    }
                    break;
            }
        }

        return result;
    }

    public static List<int> DetermineOutcomesForTeam(Match match, List<Outcome> outcomes, int teamId)
    {
        var result = new List<int>();
        var totalGoals = (match.Team1Goals ?? 0) + (match.Team2Goals ?? 0);

        foreach (var outcome in outcomes)
        {
            switch (outcome.Code.ToUpperInvariant())
            {
                case SD.WIN:
                    if ((match.Team1Goals > match.Team2Goals && teamId == match.Team1Id) ||
                        (match.Team2Goals > match.Team1Goals && teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.LOSE:
                    if ((match.Team1Goals < match.Team2Goals && teamId == match.Team1Id) ||
                        (match.Team2Goals < match.Team1Goals && teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.DRAW:
                    if (match.Team1Goals == match.Team2Goals && (teamId == match.Team1Id || teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.OVER_3_5:
                    if (totalGoals > 3 && (teamId == match.Team1Id || teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.UNDER_1_5:
                    if (totalGoals <= 1 && (teamId == match.Team1Id || teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;
            }
        }

        return result;
    }

    private static void AddOutcome(List<MatchOutcome> result, int matchId, int outcomeId, int teamId)
    {
        result.Add(new MatchOutcome
        {
            MatchId = matchId,
            OutcomeId = outcomeId,
            TeamId = teamId
        });
    }

    private static void AddBothTeamsOutcome(List<MatchOutcome> result, int matchId, int outcomeId, int team1Id, int team2Id)
    {
        AddOutcome(result, matchId, outcomeId, team1Id);
        AddOutcome(result, matchId, outcomeId, team2Id);
    }
}
