namespace Application.DTOs.Competitions;

public class CreateCompetitionDto
{
    public string Name { get; set; } = null!;
    public bool IsInternational { get; set; }
}
