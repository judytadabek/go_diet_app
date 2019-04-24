using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoDiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using System.IO;

namespace GoDiet.Tests
{
    [TestClass()]
    public class BinaryClassMLTests
    {
        [TestMethod()]
        public void TrainTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "\\GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            MLContext mlContext = new MLContext(seed: 0);
            TextLoader _textLoader = mlContext.Data.CreateTextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                {
                    new TextLoader.Column("Label", DataKind.Bool, 1),
                    new TextLoader.Column("BMI", DataKind.Text, 0)
                }
            });
            
            IDataView dataView = _textLoader.Read(_trainDataPath);
            var pipeline = mlContext.Transforms.Text.FeaturizeText("BMI", "Features")
                .Append(mlContext.BinaryClassification.Trainers.FastTree(numLeaves: 10, numTrees: 10, minDatapointsInLeaves: 5));
            var exp = pipeline.Fit(dataView);

            var res = BinaryClassML.Train(mlContext, _trainDataPath);
            Assert.AreNotSame(exp, res);
        }

        [TestMethod()]
        public void EvaluateTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "\\GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            MLContext mlContext = new MLContext(seed: 0);

            var model = BinaryClassML.Train(mlContext, _trainDataPath);
            BinaryClassML.Evaluate(mlContext, model);

            Assert.AreEqual("Pass", BinaryClassML.check);
        }

        [TestMethod()]
        public void EvaluateTestException()
        {
            try
            {
                string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
                string _trainDataPath = getPath + "\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
                MLContext mlContext = new MLContext(seed: 0);

                var model = BinaryClassML.Train(mlContext, _trainDataPath);
                BinaryClassML.Evaluate(mlContext, model);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void EvaluateFailTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string trainDataPath = getPath + "\\GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            MLContext mlContext = new MLContext(seed: 0);

            var model = BinaryClassML.Train(mlContext, trainDataPath);
            BinaryClassML.Evaluate(mlContext, model);

            Assert.AreNotEqual("", BinaryClassML.check);
        }

        [TestMethod()]
        public void PredictTrueTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "\\GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            MLContext mlContext = new MLContext(seed: 0);
            TextLoader _textLoader = mlContext.Data.CreateTextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                {
                    new TextLoader.Column("Label", DataKind.Bool, 1),
                    new TextLoader.Column("BMI", DataKind.Text, 0)
                }
            });

            IDataView dataView = _textLoader.Read(_trainDataPath);
            var pipeline = mlContext.Transforms.Text.FeaturizeText("BMI", "Features")
                .Append(mlContext.BinaryClassification.Trainers.FastTree(numLeaves: 10, numTrees: 10, minDatapointsInLeaves: 5));
            var exp = pipeline.Fit(dataView);

            var model = BinaryClassML.Train(mlContext, _trainDataPath);
            bool res = BinaryClassML.Predict(mlContext, model, "58");
           
            Assert.AreEqual(true, res);
        }

        [TestMethod()]
        public void PredictFalseTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "\\GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            MLContext mlContext = new MLContext(seed: 0);
            TextLoader _textLoader = mlContext.Data.CreateTextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                {
                    new TextLoader.Column("Label", DataKind.Bool, 1),
                    new TextLoader.Column("BMI", DataKind.Text, 0)
                }
            });

            IDataView dataView = _textLoader.Read(_trainDataPath);
            var pipeline = mlContext.Transforms.Text.FeaturizeText("BMI", "Features")
                .Append(mlContext.BinaryClassification.Trainers.FastTree(numLeaves: 10, numTrees: 10, minDatapointsInLeaves: 5));
            var exp = pipeline.Fit(dataView);

            ITransformer model = BinaryClassML.Train(mlContext, _trainDataPath);
            bool res = BinaryClassML.Predict(mlContext, model, "10");
            //Console.ReadLine();
            
            Assert.AreEqual(false, res);
        }


        [TestMethod()]
        public void SaveModelAsFileExceptionThrownTest()
        {

            try
            {
                string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
                string _trainDataPath = getPath + "\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
                MLContext mlContext = new MLContext(seed: 0);
                ITransformer model = BinaryClassML.Train(mlContext, _trainDataPath);
                BinaryClassML.SaveModelAsFile(mlContext, model);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void SaveModelAsFile()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "\\GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            MLContext mlContext = new MLContext(seed: 0);
            ITransformer model = BinaryClassML.Train(mlContext, _trainDataPath);
            BinaryClassML.SaveModelAsFile(mlContext, model);
            Assert.AreEqual("Saved", BinaryClassML.check);
        }

        [TestMethod()]
        public void RecordDataTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string modelPath = getPath + "\\GoDiet\\DietData\\DataModel\\modelPath";
            BinaryClassML.RecordData(modelPath, "");
            Assert.AreEqual("Worked", BinaryClassML.check);
        }

        [TestMethod()]
        public void RecordDataIsFileTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string modelPath = getPath + "\\GoDiet\\DietData\\DataModel\\modelPath";
            BinaryClassML.RecordData(modelPath, "");
            InitialWindow.SetUsername = "landon";
            string fileName = "binary_cl_" + InitialWindow.SetUsername.ToString() + ".txt";
            string dataModelPath = Path.Combine(modelPath, fileName);
            string filePath = modelPath + "\\" + fileName;
            BinaryClassML.RecordData(modelPath, "");
            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod()]
        public void RecordDataFileContentTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string modelPath = getPath + "\\GoDiet\\DietData\\DataModel\\modelPath";
            BinaryClassML.RecordData(modelPath, "");
            InitialWindow.SetUsername = "landon";
            string fileName = "binary_cl_" + InitialWindow.SetUsername.ToString() + ".txt";
            string dataModelPath = Path.Combine(modelPath, fileName);
            string filePath = modelPath + "\\" + fileName;
            BinaryClassML.RecordData(modelPath, "");
            string content = "Binary Classification Algorithm Data Record";
            bool checkThis = false;
            using (StreamReader sr = new StreamReader(filePath))
            {
                string contents = sr.ReadToEnd();
                if (contents.Contains(content))
                {
                    checkThis = true;
                }
            }
            Assert.AreEqual(true, checkThis);
        }

        [TestMethod()]
        public void RecordDataExceptionThrownTest()
        {
            try
            {
                string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
                string modelPath = getPath + "\\DietData\\DataModel\\modelPath";
                BinaryClassML.RecordData(modelPath, "");
                Assert.Fail();
            }
            catch (Exception) { }
        }
    }
}