namespace Application.DTOs.Teams;

public class UpdateTeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public int LeagueId { get; set; }
    public int SeasonId { get; set; }
}
