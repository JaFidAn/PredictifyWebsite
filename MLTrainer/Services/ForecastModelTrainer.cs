using Microsoft.ML;
using Microsoft.ML.Data;
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
        // 1. CSV faylını oxu
        IDataView dataView = _mlContext.Data.LoadFromTextFile<ForecastModelInput>(
            path: dataPath,
            hasHeader: true,
            separatorChar: ',');

        // 2. Data-nı tren və test üçün böl
        var trainTestSplit = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

        // 3. Pipeline: Xüsusiyyətləri birləşdir və təlim alqoritması seç
        var pipeline = _mlContext.Transforms.Concatenate("Features",
                nameof(ForecastModelInput.OutcomeId),
                nameof(ForecastModelInput.StreakCount),
                nameof(ForecastModelInput.MaxStreak),
                nameof(ForecastModelInput.Ratio))
            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: nameof(ForecastModelInput.IsCorrect),
                featureColumnName: "Features"));

        // 4. Modeli öyrət
        Console.WriteLine("📊 Model təlim olunur...");
        var model = pipeline.Fit(trainTestSplit.TrainSet);

        // 5. Modeli test et və nəticələri göstər
        var predictions = model.Transform(trainTestSplit.TestSet);
        var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: nameof(ForecastModelInput.IsCorrect));

        Console.WriteLine($"✅ Model təlimi tamamlandı!");
        Console.WriteLine($"🎯 Accuracy: {metrics.Accuracy:P2}");
        Console.WriteLine($"📈 AUC: {metrics.AreaUnderRocCurve:P2}");
        Console.WriteLine($"🎯 F1 Score: {metrics.F1Score:P2}");

        // 6. Modeli fayla yaz
        var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Model");
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var modelPath = Path.Combine(outputDirectory, "ForecastPredictionModel.zip");

        _mlContext.Model.Save(model, trainTestSplit.TrainSet.Schema, modelPath);

        Console.WriteLine($"💾 Model saxlandı: {modelPath}");
    }
}
