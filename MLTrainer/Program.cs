using MLTrainer.Services;

namespace MLTrainer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🚀 Starting Forecast Model Training...");

        var trainer = new ForecastModelTrainer();

        // Məlumat faylının tam yolunu burada göstər
        var dataPath = Path.Combine("Data", "forecast-training-data.csv");

        trainer.Train(dataPath);

        Console.WriteLine("✅ Training complete.");
    }
}