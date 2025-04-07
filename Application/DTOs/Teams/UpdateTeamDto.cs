namespace Application.DTOs.Teams;

public class UpdateTeamDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public List<TeamSeasonLeagueCreateDto> TeamSeasonLeagues { get; set; } = new();
}
