namespace Application.DTOs.Seasons;

public class SeasonDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
