namespace Application.DTOs.Teams;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = null!;
    public List<TeamSeasonLeagueDto> TeamSeasonLeagues { get; set; } = new();
}
