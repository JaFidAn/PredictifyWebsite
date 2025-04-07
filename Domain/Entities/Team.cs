using Domain.Entities.Common;

namespace Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; } = null!;
    public string CountryId { get; set; } = null!;
    public Country Country { get; set; } = null!;

    public ICollection<TeamSeasonLeague> TeamSeasonLeagues { get; set; } = new List<TeamSeasonLeague>();
}
