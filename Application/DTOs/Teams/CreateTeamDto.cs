namespace Application.DTOs.Teams;

public class CreateTeamDto
{
    public string Name { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public List<TeamSeasonLeagueCreateDto> TeamSeasonLeagues { get; set; } = new();
}
