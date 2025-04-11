using Domain.Entities.Common;

namespace Domain.Entities;

public class League : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public int CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;
}
