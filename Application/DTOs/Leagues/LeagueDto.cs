namespace Application.DTOs.Leagues;

public class LeagueDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public string CompetitionId { get; set; } = null!;
}
