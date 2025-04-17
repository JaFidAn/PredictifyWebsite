using Domain.Entities;
using Application.Utilities;

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
                case SD.WIN:
                    AddOutcomeIf(match, result, outcome, match.Team1Goals > match.Team2Goals, match.Team1Id);
                    AddOutcomeIf(match, result, outcome, match.Team2Goals > match.Team1Goals, match.Team2Id);
                    break;

                case SD.DRAW:
                    if (match.Team1Goals == match.Team2Goals)
                    {
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team1Id });
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team2Id });
                    }
                    break;

                case SD.LOSE:
                    AddOutcomeIf(match, result, outcome, match.Team1Goals < match.Team2Goals, match.Team1Id);
                    AddOutcomeIf(match, result, outcome, match.Team2Goals < match.Team1Goals, match.Team2Id);
                    break;

                case SD.OVER_2_5:
                    if ((match.Team1Goals + match.Team2Goals) > 2)
                    {
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team1Id });
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team2Id });
                    }
                    break;

                case SD.UNDER_2_5:
                    if ((match.Team1Goals + match.Team2Goals) <= 2)
                    {
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team1Id });
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team2Id });
                    }
                    break;

                case SD.BTTS:
                    if (match.Team1Goals > 0 && match.Team2Goals > 0)
                    {
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team1Id });
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team2Id });
                    }
                    break;

                case SD.BTNS:
                    if (match.Team1Goals == 0 || match.Team2Goals == 0)
                    {
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team1Id });
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team2Id });
                    }
                    break;

                case SD.T1_OVER_1_5:
                    if (match.Team1Goals > 1)
                    {
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team1Id });
                    }
                    break;

                case SD.T2_OVER_1_5:
                    if (match.Team2Goals > 1)
                    {
                        result.Add(new MatchOutcome { MatchId = match.Id, OutcomeId = outcome.Id, TeamId = match.Team2Id });
                    }
                    break;
            }
        }

        return result;
    }

    public static List<int> DetermineOutcomesForTeam(Match match, List<Outcome> outcomes, int teamId)
    {
        var result = new List<int>();

        foreach (var outcome in outcomes)
        {
            switch (outcome.Code.ToUpper())
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

                case SD.OVER_2_5:
                    if ((match.Team1Goals + match.Team2Goals) > 2 && (teamId == match.Team1Id || teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.UNDER_2_5:
                    if ((match.Team1Goals + match.Team2Goals) <= 2 && (teamId == match.Team1Id || teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.BTTS:
                    if (match.Team1Goals > 0 && match.Team2Goals > 0 && (teamId == match.Team1Id || teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.BTNS:
                    if ((match.Team1Goals == 0 || match.Team2Goals == 0) && (teamId == match.Team1Id || teamId == match.Team2Id))
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.T1_OVER_1_5:
                    if (teamId == match.Team1Id && match.Team1Goals > 1)
                    {
                        result.Add(outcome.Id);
                    }
                    break;

                case SD.T2_OVER_1_5:
                    if (teamId == match.Team2Id && match.Team2Goals > 1)
                    {
                        result.Add(outcome.Id);
                    }
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
