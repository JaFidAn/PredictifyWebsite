using Application.Repositories.MatchRepositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories.MatchRepositories
{
    public class MatchWriteRepository : WriteRepository<Match, int>, IMatchWriteRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchWriteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void RemoveMatchTeamSeasonLeagues(Match match)
        {
            var matchTeamSeasonLeagues = match.MatchTeamSeasonLeagues.ToList();
            _context.MatchTeamSeasonLeagues.RemoveRange(matchTeamSeasonLeagues);
        }

        public void RemoveMatchOutcomes(Match match)
        {
            var outcomes = match.Outcomes.ToList();
            _context.Set<MatchOutcome>().RemoveRange(outcomes);
        }
    }
}
