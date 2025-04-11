using Domain.Entities.Common;

namespace Domain.Entities;

public class Team : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public ICollection<TeamSeasonLeague> TeamSeasonLeagues { get; set; } = new List<TeamSeasonLeague>();
}
