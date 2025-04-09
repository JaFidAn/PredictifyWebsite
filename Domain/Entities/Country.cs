using Domain.Entities.Common;

namespace Domain.Entities;

public class Country : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;

    public ICollection<Team> Teams { get; set; } = new List<Team>();
}
