using Application.Core;

namespace Application.Params;

public class SeasonFilterParams : PaginationParams
{
    public string? Name { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
}
