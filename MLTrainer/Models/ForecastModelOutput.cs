using Microsoft.ML.Data;

namespace MLTrainer.Models;

public class ForecastModelOutput
{
    // Modelin proqnozlaşdırdığı nəticə
    [ColumnName("PredictedLabel")]
    public bool Prediction { get; set; }

    // Nəticənin ehtimalı (Confidence)
    public float Probability { get; set; }

    // Raw skor (bəzi hallarda ehtiyac ola bilər)
    public float Score { get; set; }
}