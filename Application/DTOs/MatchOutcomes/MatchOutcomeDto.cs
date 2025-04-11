namespace Application.DTOs.MatchOutcomes;

public class MatchOutcomeDto
{
    public int MatchId { get; set; }
    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;
    public int OutcomeId { get; set; }
    public string OutcomeName { get; set; } = null!;
}
