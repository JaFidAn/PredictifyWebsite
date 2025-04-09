using Domain.Entities.Common;

namespace Domain.Entities;

public class Outcome : BaseEntity<int>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
