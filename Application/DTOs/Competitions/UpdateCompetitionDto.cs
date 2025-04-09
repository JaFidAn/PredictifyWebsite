namespace Application.DTOs.Competitions;

public class UpdateCompetitionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsInternational { get; set; }
}
