namespace Application.DTOs.Seasons;

public class CreateSeasonDto
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
