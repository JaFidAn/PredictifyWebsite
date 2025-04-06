using Domain.Entities.Common;

namespace Domain.Entities;

public class Season : BaseEntity
{
    public string Name { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
