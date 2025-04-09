using Domain.Entities.Common;

namespace Domain.Entities;

public class Competition : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public bool IsInternational { get; set; }

    public ICollection<League> Leagues { get; set; } = new List<League>();
}
