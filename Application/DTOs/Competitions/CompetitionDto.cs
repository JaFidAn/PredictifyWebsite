namespace Application.DTOs.Competitions;

public class CompetitionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsInternational { get; set; }
}
