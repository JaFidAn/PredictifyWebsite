using Domain.Entities.Common;

namespace Domain.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
}
