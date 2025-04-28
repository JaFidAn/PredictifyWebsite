using Microsoft.ML.Data;

namespace MLTrainer.Models;

public class ForecastModelInput
{
    // Outcome ID (təlim modeli üçün süzüləcək, amma saxlanılmalıdır)
    [LoadColumn(0)]
    public float OutcomeId { get; set; }

    // Cari streak sayı
    [LoadColumn(1)]
    public float StreakCount { get; set; }

    // Maksimum streak sayı
    [LoadColumn(2)]
    public float MaxStreak { get; set; }

    // Cari streak / Maksimum streak nisbəti
    [LoadColumn(3)]
    public float Ratio { get; set; }

    // Modelin təxmin etməyə çalışacağı nəticə (True/False)
    [LoadColumn(4)]
    public bool IsCorrect { get; set; }
}