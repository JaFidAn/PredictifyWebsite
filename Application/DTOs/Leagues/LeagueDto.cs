namespace Application.DTOs.Leagues;

public class LeagueDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public int CompetitionId { get; set; }
}
