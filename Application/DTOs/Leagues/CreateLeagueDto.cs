namespace Application.DTOs.Leagues;

public class CreateLeagueDto
{
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public int? CompetitionId { get; set; }
}
