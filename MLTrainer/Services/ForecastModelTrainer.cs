using Microsoft.ML;
using MLTrainer.Models;

namespace MLTrainer.Services;

public class ForecastModelTrainer
{
    private readonly MLContext _mlContext;

    public ForecastModelTrainer()
    {
        _mlContext = new MLContext();
    }

    public void Train(string dataPath)
    {
        IDataView dataView = _mlContext.Data.LoadFromTextFile<ForecastModelInput>(
            path: dataPath,
            hasHeader: true,
            separatorChar: ',');

        var split = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

        var pipeline = _mlContext.Transforms.Concatenate("Features",
                nameof(ForecastModelInput.OutcomeId),
                nameof(ForecastModelInput.StreakCount),
                nameof(ForecastModelInput.MaxStreak),
                nameof(ForecastModelInput.Ratio))
            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(ForecastModelInput.IsCorrect),
                featureColumnName: "Features"));

        Console.WriteLine("📊 Ümumi model təlimi başlanır...");
        var model = pipeline.Fit(split.TrainSet);

        var predictions = model.Transform(split.TestSet);
        var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: nameof(ForecastModelInput.IsCorrect));

        Console.WriteLine($"✅ Ümumi model təlimi tamamlandı.");
        PrintMetrics(metrics);

        var modelPath = Path.Combine("Model", "ForecastPredictionModel.zip");
        SaveModel(model, split.TrainSet.Schema, modelPath);
    }

    public void TrainPerOutcome(string dataPath)
    {
        var outcomes = LoadOutcomeListFromCsv("Data/outcomes.csv");

        foreach (var (outcomeId, code) in outcomes)
        {
            Console.WriteLine($"🔁 Təlim: {code}");

            var allData = _mlContext.Data.LoadFromTextFile<ForecastModelInput>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',');

            var filtered = _mlContext.Data.FilterRowsByColumn(allData, nameof(ForecastModelInput.OutcomeId),
                lowerBound: outcomeId, upperBound: outcomeId + 0.1);

            var split = _mlContext.Data.TrainTestSplit(filtered, testFraction: 0.2);

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(ForecastModelInput.StreakCount),
                    nameof(ForecastModelInput.MaxStreak),
                    nameof(ForecastModelInput.Ratio))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(ForecastModelInput.IsCorrect),
                    featureColumnName: "Features"));

            var model = pipeline.Fit(split.TrainSet);

            var predictions = model.Transform(split.TestSet);
            var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: nameof(ForecastModelInput.IsCorrect));

            Console.WriteLine($"✅ {code} modeli təlim edildi.");
            PrintMetrics(metrics);

            var modelPath = Path.Combine("Model", $"ForecastModel_{code}.zip");
            SaveModel(model, split.TrainSet.Schema, modelPath);
        }
    }

    private static List<(int OutcomeId, string Code)> LoadOutcomeListFromCsv(string csvPath)
    {
        var result = new List<(int, string)>();

        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"❌ Fayl tapılmadı: {csvPath}");
            return result;
        }

        foreach (var line in File.ReadLines(csvPath).Skip(1)) // skip header
        {
            var parts = line.Split(',');
            if (parts.Length == 2 && int.TryParse(parts[0], out int id))
            {
                result.Add((id, parts[1].Trim()));
            }
        }

        return result;
    }

    private void SaveModel(ITransformer model, DataViewSchema schema, string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        _mlContext.Model.Save(model, schema, path);
        Console.WriteLine($"💾 Model saxlandı: {path}");
    }

    private void PrintMetrics(Microsoft.ML.Data.CalibratedBinaryClassificationMetrics metrics)
    {
        Console.WriteLine($"🎯 Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"📈 AUC: {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"🎯 F1 Score: {metrics.F1Score:P2}");
        Console.WriteLine(new string('-', 50));
    }
}
