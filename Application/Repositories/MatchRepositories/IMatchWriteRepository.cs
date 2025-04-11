using Domain.Entities;

namespace Application.Repositories.MatchRepositories
{
    public interface IMatchWriteRepository : IWriteRepository<Match, int>
    {
        void RemoveMatchTeamSeasonLeagues(Match match);
        void RemoveMatchOutcomes(Match match);
    }
}
