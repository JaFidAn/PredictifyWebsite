namespace Application.DTOs.Teams;

public class TeamDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public string CountryName { get; set; } = null!;
    public List<TeamSeasonLeagueDto> TeamSeasonLeagues { get; set; } = new();
}
