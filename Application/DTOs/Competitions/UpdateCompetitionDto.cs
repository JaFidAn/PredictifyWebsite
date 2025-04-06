namespace Application.DTOs.Competitions;

public class UpdateCompetitionDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsInternational { get; set; }
}
