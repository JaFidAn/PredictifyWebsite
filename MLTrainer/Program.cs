using MLTrainer.Services;

namespace MLTrainer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🚀 Forecast Model Training başladı...");

        var trainer = new ForecastModelTrainer();

        // Məlumat faylının tam yolu
        var dataPath = Path.Combine("Data", "forecast-training-data.csv");

        // 1️⃣ Ümumi model təlimi
        trainer.Train(dataPath);

        // 2️⃣ Outcome-lara görə ayrıca model təlimi
        trainer.TrainPerOutcome(dataPath);

        Console.WriteLine("✅ Bütün təlimlər tamamlandı.");
    }
}