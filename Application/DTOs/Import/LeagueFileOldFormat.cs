namespace Application.DTOs.Import;

public class LeagueFileOldFormat
{
    public string Name { get; set; } = default!;
    public List<RoundDto> Rounds { get; set; } = new();
}

public class RoundDto
{
    public string Name { get; set; } = default!;
    public List<MatchJsonOld> Matches { get; set; } = new();
}

public class MatchJsonOld
{
    public string Date { get; set; } = default!;
    public string Team1 { get; set; } = default!;
    public string Team2 { get; set; } = default!;
    public ScoreDto Score { get; set; } = new();
}