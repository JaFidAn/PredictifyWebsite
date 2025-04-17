using Microsoft.ML.Data;

namespace MLTrainer.Models;

public class ForecastModelInput
{
    // ML.NET üçün atributlar, sütunların sırasını və adlarını izah edir

    [LoadColumn(0)]
    public float OutcomeId { get; set; }

    [LoadColumn(1)]
    public float StreakCount { get; set; }

    [LoadColumn(2)]
    public float MaxStreak { get; set; }

    [LoadColumn(3)]
    public float Ratio { get; set; }

    [LoadColumn(4)]
    public bool IsCorrect { get; set; } // Bu bizim təlim modelinin hədəfidir (label)
}