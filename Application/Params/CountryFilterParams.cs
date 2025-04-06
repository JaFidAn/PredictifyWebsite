using Application.Core;

namespace Application.Params;

public class CountryFilterParams : PaginationParams
{
    public string? Name { get; set; }
    public string? Code { get; set; }
}
