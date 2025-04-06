using Domain.Entities.Common;

namespace Domain.Entities;

public class League : BaseEntity
{
    public string Name { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public Country Country { get; set; } = null!;

    public string CompetitionId { get; set; } = null!;
    public Competition Competition { get; set; } = null!;
}
