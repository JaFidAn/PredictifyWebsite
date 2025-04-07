using Domain.Entities;

namespace Application.Repositories.TeamRepositories;

public interface ITeamWriteRepository : IWriteRepository<Team>
{
    void RemoveTeamSeasonLeagues(Team team);
}
