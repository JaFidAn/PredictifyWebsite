namespace Application.DTOs.Leagues;

public class CreateLeagueDto
{
    public string Name { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public string? CompetitionId { get; set; }
}
