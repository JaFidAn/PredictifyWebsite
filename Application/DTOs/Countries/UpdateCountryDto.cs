namespace Application.DTOs.Countries;

public class UpdateCountryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}
