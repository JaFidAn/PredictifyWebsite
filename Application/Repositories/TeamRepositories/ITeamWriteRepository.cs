using Domain.Entities;

namespace Application.Repositories.TeamRepositories;

public interface ITeamWriteRepository : IWriteRepository<Team, int>
{
    void RemoveTeamSeasonLeagues(Team team);
}
