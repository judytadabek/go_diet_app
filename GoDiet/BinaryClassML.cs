using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;



namespace GoDiet
{

    public class BinaryClassML
    {
        public static string check = "";
        static string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
        static readonly string _testDataPath = getPath + "\\DietData\\DataModel\\testData\\weight_lr_test1.csv";
        static readonly string modelPathZip = getPath + "\\DietData\\DataModel\\modelPath\\Model.zip";
        static readonly string modelPath = getPath + "\\DietData\\DataModel\\modelPath";
        //BinaryClassData - input dataset class
        // BinaryClassification - float labelling 0 for negative, 1 for positive.
        // 0 - no need to go for diet, 1 - qualified for a diet
        //BinaryData - float data containing BMI result; it might be required to change to the string?

        public class BinaryClassData
        {
            [Column(ordinal: "0")]
            public string BMI;
            [Column(ordinal: "1", name: "Label")] //order of each field in the data file
            public bool BinaryClass;

        }

        //class used for prediction after the model has been trained
        public class BinaryClassPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }

            [ColumnName("Probability")]
            public float Probability { get; set; }

            [ColumnName("Score")]
            public float Score { get; set; }
        }



        //The environment provides a context for your ML job that can be used for exception tracking and logging.
        static MLContext mlContext = new MLContext(seed: 0);

        static TextLoader _textLoader = mlContext.Data.CreateTextReader(new TextLoader.Arguments()
        {
            Separator = ",",
            HasHeader = true,
            Column = new[]
                {
                    new TextLoader.Column("Label", DataKind.Bool, 1),
                    new TextLoader.Column("BMI", DataKind.Text, 0)
                }
        }
        );

        public static ITransformer Train(MLContext mlContext, string dataPath)
        {
            IDataView dataView = _textLoader.Read(dataPath);
            var pipeline = mlContext.Transforms.Text.FeaturizeText("BMI", "Features")
                .Append(mlContext.BinaryClassification.Trainers.FastTree(numLeaves: 10, numTrees: 10, minDatapointsInLeaves: 5));

            string timeDate = DateTime.Now.ToString();
            string createTrain = " =============== Create and Train the Model =============== ";
            Console.WriteLine(createTrain);
            RecordData(modelPath, timeDate);
            RecordData(modelPath, createTrain);

            var model = pipeline.Fit(dataView);

            string trainingEnd = "=============== End of training ===============";
            Console.WriteLine(trainingEnd);
            Console.WriteLine();
            RecordData(modelPath, trainingEnd);

            return model;
        }

        public static void Evaluate(MLContext mlContext, ITransformer model)
        {
            IDataView dataView = _textLoader.Read(_testDataPath);

            string evaluating = "=============== Evaluating Model accuracy with Test data===============";
            Console.WriteLine(evaluating);
            RecordData(modelPath, evaluating);

            var predictions = model.Transform(dataView);
            var metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

            string metricsEval = "Model quality metrics evaluation";
            string thinLine = "--------------------------------";
            string acc = $"Accuracy: {metrics.Accuracy:P2}";
            string auc = $"Auc: {metrics.Auc:P2}";
            string f1 = $"F1Score: {metrics.F1Score:P2}";
            string endModelEval = "=============== End of model evaluation ===============";
            Console.WriteLine();
            Console.WriteLine(metricsEval);
            Console.WriteLine(thinLine);
            Console.WriteLine(acc);
            Console.WriteLine(auc);
            Console.WriteLine(f1);
            Console.WriteLine(endModelEval);
            RecordData(modelPath, metricsEval);
            RecordData(modelPath, thinLine);
            RecordData(modelPath, acc);
            RecordData(modelPath, auc);
            RecordData(modelPath, f1);
            RecordData(modelPath, endModelEval);
            SaveModelAsFile(mlContext, model);
            check = "Pass";
        }


        public static bool Predict(MLContext mlContext, ITransformer model, string dataToBePredicted)
        {
            bool qualified4diet = false;
            var predictionFunction = model.CreatePredictionEngine<BinaryClassData, BinaryClassPrediction>(mlContext);

            BinaryClassData sampleStatement = new BinaryClassData
            {
                BMI = dataToBePredicted
            };
            var resultprediction = predictionFunction.Predict(sampleStatement);

            string predictionTest = "=============== Prediction Test of model with a single sample and test dataset ===============";
            string data = $"Sentiment: {sampleStatement.BMI} | Prediction: {(Convert.ToBoolean(resultprediction.Prediction) ? "1" : "0")} | Probability: {resultprediction.Probability} ";
            string endPred = "=============== End of Predictions ===============";
            Console.WriteLine();
            Console.WriteLine(predictionTest);

            Console.WriteLine();
            Console.WriteLine(data);
            Console.WriteLine(endPred);
            Console.WriteLine();

            RecordData(modelPath, predictionTest);
            RecordData(modelPath, data);
            RecordData(modelPath, endPred);
            RecordData(modelPath, "**********END OF PREDICTION *******************");
            RecordData(modelPath, Environment.NewLine);
            qualified4diet = Convert.ToBoolean(resultprediction.Prediction);
            return qualified4diet;
        }

        //public static void PredictWithModelLoadedFromFile(MLContext mlContext)
        //{
        //    IEnumerable<BinaryClassData> sentiments = new[]
        //    {
        //        new BinaryClassData
        //        {
        //            BMI = "50"
        //        },
        //        new BinaryClassData
        //        {
        //            BMI = "12"
        //        }
        //    };
        //    ITransformer loadedModel;
        //    using (var stream = new FileStream(modelPathZip, FileMode.Open, FileAccess.Read, FileShare.Read))
        //    {
        //        loadedModel = mlContext.Model.Load(stream);
        //    }

        //    // Create prediction engine
        //    var sentimentStreamingDataView = mlContext.CreateStreamingDataView(sentiments);
        //    var predictions = loadedModel.Transform(sentimentStreamingDataView);

        //    // Use the model to predict whether comment data is toxic (1) or nice (0).
        //    var predictedResults = predictions.AsEnumerable<BinaryClassPrediction>(mlContext, reuseRowObject: false);

        //    Console.WriteLine();

        //    Console.WriteLine("=============== Prediction Test of loaded model with a multiple samples ===============");
        //    var sentimentsAndPredictions = sentiments.Zip(predictedResults, (sentiment, prediction) => (sentiment, prediction));
        //    foreach (var (sentiment, prediction) in sentimentsAndPredictions)
        //    {
        //        Console.WriteLine($"Sentiment: {sentiment.BMI} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "1" : "0")} | Probability: {prediction.Probability} ");
        //    }
        //    Console.WriteLine("=============== End of prediction ===============");
        //}


        //saves the model as zip
        public static void SaveModelAsFile(MLContext mlContext, ITransformer model)
        {
            using (var fs = new FileStream(modelPathZip, FileMode.Create, FileAccess.Write, FileShare.Write))
                mlContext.Model.Save(model, fs);
            string savedModel = "The model is saved to " + modelPathZip;
            Console.WriteLine(savedModel);

            RecordData(modelPath, "------------------------------------------->");
            RecordData(modelPath, savedModel);
            RecordData(modelPath, "==============================END OF PROCESS================================\n \n \n");
            check = "Saved";
        }

        public static void RecordData(string modelPath, string content)
        {

            string fileName = "binary_cl_" + InitialWindow.SetUsername.ToString() + ".txt";
            string dataModelPath = Path.Combine(modelPath, fileName);

            if (!File.Exists(dataModelPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(dataModelPath))
                {
                    sw.WriteLine("===============================================");
                    sw.WriteLine("Binary Classification Algorithm Data Record");
                    sw.WriteLine("===============================================");
                    sw.WriteLine($"First Data Record on {DateTime.Now} ................");
                    sw.WriteLine("\n");
                    sw.Close();
                    sw.Dispose();
                }
                
            }
            else
            {
                File.AppendAllText(dataModelPath, content + Environment.NewLine);
            }

            check = "Worked";
        }

    }
}

