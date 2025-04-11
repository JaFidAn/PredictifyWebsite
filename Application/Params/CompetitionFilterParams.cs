using Application.Core;

namespace Application.Params;

public class CompetitionFilterParams : PaginationParams
{
    public string? Name { get; set; }
    public bool? IsInternational { get; set; }
}
