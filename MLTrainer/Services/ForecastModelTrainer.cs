using Microsoft.ML;
using MLTrainer.Models;

namespace MLTrainer.Services
{
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

            var trainTestSplit = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(ForecastModelInput.OutcomeId),
                    nameof(ForecastModelInput.StreakCount),
                    nameof(ForecastModelInput.MaxStreak),
                    nameof(ForecastModelInput.Ratio))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(ForecastModelInput.IsCorrect),
                    featureColumnName: "Features"));

            Console.WriteLine("📊 Model təlim olunur...");
            var model = pipeline.Fit(trainTestSplit.TrainSet);

            var predictions = model.Transform(trainTestSplit.TestSet);
            var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: nameof(ForecastModelInput.IsCorrect));

            Console.WriteLine($"✅ Model təlimi tamamlandı!");
            Console.WriteLine($"🎯 Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"📈 AUC: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"🎯 F1 Score: {metrics.F1Score:P2}");

            var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Model");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var modelPath = Path.Combine(outputDirectory, "ForecastPredictionModel.zip");
            _mlContext.Model.Save(model, trainTestSplit.TrainSet.Schema, modelPath);

            Console.WriteLine($"💾 Model saxlandı: {modelPath}");
        }

        public void TrainPerOutcome(string dataPath)
        {
            var outcomes = new List<(int OutcomeId, string Code)>
            {
                (1, "WIN"),
                (2, "DRAW"),
                (3, "LOSE"),
                (4, "OVER_3_5"),
                (5, "UNDER_1_5")
            };

            foreach (var (outcomeId, code) in outcomes)
            {
                Console.WriteLine($"🔁 Training model for outcome: {code}");

                IDataView allData = _mlContext.Data.LoadFromTextFile<ForecastModelInput>(
                    path: dataPath,
                    hasHeader: true,
                    separatorChar: ',');

                var filteredData = _mlContext.Data.FilterRowsByColumn(allData, nameof(ForecastModelInput.OutcomeId),
                    lowerBound: outcomeId, upperBound: outcomeId + 0.1);

                var trainTestSplit = _mlContext.Data.TrainTestSplit(filteredData, testFraction: 0.2);

                var pipeline = _mlContext.Transforms.Concatenate("Features",
                        nameof(ForecastModelInput.StreakCount),
                        nameof(ForecastModelInput.MaxStreak),
                        nameof(ForecastModelInput.Ratio))
                    .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: nameof(ForecastModelInput.IsCorrect),
                        featureColumnName: "Features"));

                var model = pipeline.Fit(trainTestSplit.TrainSet);

                var predictions = model.Transform(trainTestSplit.TestSet);
                var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: nameof(ForecastModelInput.IsCorrect));

                Console.WriteLine($"✅ {code} modeli tamamlandı: Accuracy={metrics.Accuracy:P2}, AUC={metrics.AreaUnderRocCurve:P2}");

                var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Model");
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                var modelPath = Path.Combine(outputDirectory, $"ForecastModel_{code}.zip");
                _mlContext.Model.Save(model, trainTestSplit.TrainSet.Schema, modelPath);

                Console.WriteLine($"💾 Model saxlandı: {modelPath}\n");
            }
        }
    }
}
