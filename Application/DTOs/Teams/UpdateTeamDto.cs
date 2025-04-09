namespace Application.DTOs.Teams;

public class UpdateTeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public List<TeamSeasonLeagueCreateDto> TeamSeasonLeagues { get; set; } = new();
}
