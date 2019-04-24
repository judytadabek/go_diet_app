using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoDiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace GoDiet.Tests
{
    [TestClass()]
    public class MyMethodsLibTests
    {
        [TestMethod()]
        public void GetConnectionStringTest()
        {
            string connectionExp = "";
            string part = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename=";
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string connectionPath = getPath + "dbs\\GODIETCUSTINFO.MDF";
            string part2 = ";Integrated Security = True";
            connectionExp = part + connectionPath + part2;

            string connRes = MyMethodsLib.GetConnectionString();
            Assert.AreEqual(connectionExp, connRes);
        }

        [TestMethod()]
        public void GetConnectionStringFailTest()
        {
            string connectionExp = "";
            string part = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename=";
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\"));
            string connectionPath = getPath + "dbs\\GODIETCUSTINFO.MDF";
            string part2 = ";Integrated Security = True";
            connectionExp = part + connectionPath + part2;

            string connRes = MyMethodsLib.GetConnectionString();
            Assert.AreNotEqual(connectionExp, connRes);
        }

        [TestMethod()]
        public void GetConnectionStringExceptionThrownTest()
        {
            try
            {

                string connRes = MyMethodsLib.GetConnectionString();
                Assert.Fail();
            }
            catch (Exception) { }
        }


        [TestMethod()]
        public void CallLogisticRegressionTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            bool exp = true;
            bool res = MyMethodsLib.CallLogisticRegression("20", "ladygoodwill", _trainDataPath);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void CallLogisticRegressionFailTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "GoDiet\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            bool exp = false;
            bool res = MyMethodsLib.CallLogisticRegression("20", "ladygoodwill", _trainDataPath);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetDietModeFromDbsTest()
        {
            string exp = "";
            string res = MyMethodsLib.GetDietModeFromDbs("ladygoodwill", MyMethodsLib.GetConnectionString());
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetDietModeFromDbsFailTest()
        {
            string exp = "This is not going to happen";
            string res = MyMethodsLib.GetDietModeFromDbs("ladygoodwill", MyMethodsLib.GetConnectionString());
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetDietModeFromDbsExceptionThrownTest()
        {
            try
            {


                string res = MyMethodsLib.GetDietModeFromDbs("ladygoodwill", MyMethodsLib.GetConnectionString());
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetVeggieOptionFromDbsTest()
        {
            string exp = "";
            string con = MyMethodsLib.GetConnectionString();
            string res = MyMethodsLib.GetVeggieOptionFromDbs("ladygoodwill", con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetVeggieOptionFromDbsFailTest()
        {
            string exp = "Not going to happen";
            string con = MyMethodsLib.GetConnectionString();
            string res = MyMethodsLib.GetVeggieOptionFromDbs("ladygoodwill", con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetVeggieOptionFromDbsExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                string res = MyMethodsLib.GetVeggieOptionFromDbs("ladygoodwill", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetWeightFromDbsTest()
        {
            string exp = "";
            string con = MyMethodsLib.GetConnectionString();
            var res = MyMethodsLib.GetWeightFromDbs("ladygoodwill", con);
            Assert.AreEqual(exp, res);
        }


        [TestMethod()]
        public void GetWeightFromDbsFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            string exp = "Not going to happen";
            var res = MyMethodsLib.GetWeightFromDbs("ladygoodwill", con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetWeightFromDbsExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                var res = MyMethodsLib.GetWeightFromDbs("ladygoodwill", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetGenderFromDbsTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            string exp = "";
            string res = MyMethodsLib.GetEmailFromDbs("ladygoodwill", con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetGenderFromDbsFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            string exp = "Not going to happen";
            string res = MyMethodsLib.GetEmailFromDbs("ladygoodwill", con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetGenderFromDbsExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                string res = MyMethodsLib.GetEmailFromDbs("ladygoodwill", con);
                Assert.Fail(); }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetEmailFromDbsTest()
        {
            string exp = "";
            string con = MyMethodsLib.GetConnectionString();
            string res = MyMethodsLib.GetEmailFromDbs("ladygoodwill", con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetEmailFromDbsFailTest()
        {
            string exp = "Not going to happen";
            string con = MyMethodsLib.GetConnectionString();
            string res = MyMethodsLib.GetEmailFromDbs("ladygoodwill", con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetEmailFromDbsExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                string res = MyMethodsLib.GetEmailFromDbs("ladygoodwill", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetMeasurementNoWithTodayDateTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = 0;
            int res = MyMethodsLib.GetMeasurementNoWithTodayDate("ladygoodwill", con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetMeasurementNoWithTodayDateFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = 10;
            int res = MyMethodsLib.GetMeasurementNoWithTodayDate("ladygoodwill", con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetMeasurementNoWithTodayDateExceptionThrownTest()
        {
            try
            {

                string con = MyMethodsLib.GetConnectionString();
                int res = MyMethodsLib.GetMeasurementNoWithTodayDate("ladygoodwill", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetMealDateTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            DateTime exp = default(DateTime);
            var res = MyMethodsLib.GetMealDate(0, con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetMealDateExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                var res = MyMethodsLib.GetMealDate(0, con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetMealDateFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            var exp = "";
            var res = MyMethodsLib.GetMealDate(0, con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetBreakfastIDTest()
        {
            int exp = -1;
            string con = MyMethodsLib.GetConnectionString();
            int res = MyMethodsLib.GetBreakfastID(0, con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetBreakfastIDExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                int res = MyMethodsLib.GetBreakfastID(0, con);
                Assert.Fail();
            }
            catch (Exception)
            {

            }
        }

        [TestMethod()]
        public void GetBreakfastIDFailTest()
        {
            int exp = 10;
            string con = MyMethodsLib.GetConnectionString();
            int res = MyMethodsLib.GetBreakfastID(0, con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetBreakfast2IDTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = -1;
            int res = MyMethodsLib.GetBreakfast2ID(1, con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetBreakfast2IDExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                int res = MyMethodsLib.GetBreakfast2ID(1, con);
                Assert.Fail();
            }
            catch (Exception)
            {

            }

        }

        [TestMethod()]
        public void GetBreakfast2IDFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = 10;
            int res = MyMethodsLib.GetBreakfast2ID(1, con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetLunchIDTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = -1;
            int res = MyMethodsLib.GetLunchID(1, con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetLunchIDFailTest()
        {
            int exp = 1;
            string con = MyMethodsLib.GetConnectionString();
            int res = MyMethodsLib.GetLunchID(1, con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetLunchIDExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                int res = MyMethodsLib.GetLunchID(1, con);
                Assert.Fail();
            }
            catch (Exception)
            {

            }
        }

        [TestMethod()]
        public void GetDinnerIDTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = -1;
            int res = MyMethodsLib.GetDinnerID(1, con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetDinnerIDFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = 1;
            int res = MyMethodsLib.GetDinnerID(1, con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetDinnerIDExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                int res = MyMethodsLib.GetDinnerID(1, con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetCaloriesToConsumeTest()
        {
            int exp = -1;
            string con = MyMethodsLib.GetConnectionString();
            int res = MyMethodsLib.GetCaloriesToConsume(0, con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetCaloriesToConsumeExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                int res = MyMethodsLib.GetCaloriesToConsume(0, con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetCaloriesToConsumeFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            int exp = 0;
            int res = MyMethodsLib.GetCaloriesToConsume(0, con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void CalculateBMITest()
        {
            string bmiExp = "28.40818";
            string bmiRes = MyMethodsLib.CalculateBMI("89", "177");
            Assert.AreEqual(bmiExp, bmiRes);
        }

        [TestMethod()]
        public void CalculateBMIFailTest()
        {
            string bmiExp = "19";
            string bmiRes = MyMethodsLib.CalculateBMI("89", "177");
            Assert.AreNotEqual(bmiExp, bmiRes);
        }

        [TestMethod()]
        public void GetProperWeightCalculationTest()
        {
            double exp = 0.0;
            var res = MyMethodsLib.GetProperWeightCalculation("ladygoodwill");
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetProperWeightCalculationFailTest()
        {
            double exp = 10.0;
            var res = MyMethodsLib.GetProperWeightCalculation("ladygoodwill");
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetHeightFromDbsTest()
        {
            int exp = 0;
            string con = MyMethodsLib.GetConnectionString();
            int res = MyMethodsLib.GetHeightFromDbs("ladygoodwill", con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetHeightFromDbsFailTest()
        {
            int exp = 100;
            string con = MyMethodsLib.GetConnectionString();
            int res = MyMethodsLib.GetHeightFromDbs("ladygoodwill", con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetHeightFromDbsExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                int res = MyMethodsLib.GetHeightFromDbs("ladygoodwill", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void KgToLooseTest()
        {
            double exp = 89;
            var res = MyMethodsLib.KgToLoose("ladygoodwill", "89");
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void KgToLooseFailTest()
        {
            double exp = 100;
            var res = MyMethodsLib.KgToLoose("ladygoodwill", "89");
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void WeightResultOutputTest()
        {
            string exp = "100";
            string res = MyMethodsLib.WeightResultOutput("ladygoodwill", 34.0, "100");
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void WeightResultOutput2Test()
        {
            string exp = "0";
            string res = MyMethodsLib.WeightResultOutput("ladygoodwill", 0, "0");
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void WeightResultOutputFailTest()
        {
            string exp = "101";
            string res = MyMethodsLib.WeightResultOutput("ladygoodwill", 34.0, "100");
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void DaysToAchieveGoalTest()
        {
            string exp = "68";
            string res = MyMethodsLib.DaysToAchieveGoal("100", 1.5);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void DaysToAchieveGoal2Test()
        {
            string exp = "N/A";
            string res = MyMethodsLib.DaysToAchieveGoal("", 0);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void DaysToAchieveGoalFailTest()
        {
            string exp = "90";
            string res = MyMethodsLib.DaysToAchieveGoal("100", 1.5);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void DaysToAchieveGoalFail2Test()
        {
            try
            {
                string res = MyMethodsLib.DaysToAchieveGoal("jutro", 1.5);
                Assert.Fail();
            }
            catch (Exception) { }

        }

        [TestMethod()]
        public void TimeToAchieveGoalTest()
        {
            string exp = "10";
            string res = MyMethodsLib.TimeToAchieveGoal("70", 7);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void TimeToAchieveGoalFailTest()
        {
            string exp = "100";
            string res = MyMethodsLib.TimeToAchieveGoal("70", 7);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void CaloriesIntakeTest()
        {
            int exp = 0;
            int res = MyMethodsLib.CaloriesIntake("ladygoodwill");
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void CaloriesIntakeFailTest()
        {
            int exp = 90;
            int res = MyMethodsLib.CaloriesIntake("ladygoodwill");
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void LoadJsonTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "GoDiet\\DietData";
            var res = MyMethodsLib.LoadJson(path, "b.json");
            int exp = 20;
            Assert.AreEqual(res.Count, exp);
        }

        [TestMethod()]
        public void LoadJsonFailTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "GoDiet\\DietData";
            var res = MyMethodsLib.LoadJson(path, "b.json");
            int exp = 22;
            Assert.AreNotEqual(res.Count, exp);

        }

        //[TestMethod()]
        //public void GetJsonContentTest()
        //{
        //    string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
        //    string path = getPath + "GoDiet\\DietData";
        //    var res = MyMethodsLib.GetJsonContent(path, "b.json");

        //    JsonTextReader exp = null;
        //    Assert.AreEqual(res, exp);
        //}

        // provided values will always differ!
        [TestMethod()]
        public void RandomizeRecipesSelectionVeggieTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "GoDiet\\DietData";
            var res = MyMethodsLib.RandomizeRecipesSelection("1200", "Yes", path,
                "b.json", "b2.json", "l.json", "d.json");

            List<int> expList = new List<int>() { 1, 1, 1, 1, 1 };
            Assert.AreEqual(res.Count, expList.Count);
        }

        [TestMethod()]
        public void RandomizeRecipesSelectionNonVeggieTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "GoDiet\\DietData";
            var res = MyMethodsLib.RandomizeRecipesSelection("1200", "No", path,
                "b.json", "b2.json", "l.json", "d.json");

            List<int> expList = new List<int>() { 1, 1, 1, 1, 1 };
            Assert.AreEqual(res.Count, expList.Count);
        }

        // provided values will always differ!
        [TestMethod()]
        public void RandomizeRecipesSelectionNotSameValuesTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "GoDiet\\DietData";
            var res = MyMethodsLib.RandomizeRecipesSelection("1200", "Yes", path,
                "b.json", "b2.json", "l.json", "d.json");

            List<int> expList = new List<int>() { 1, 1, 1, 1, 1 };
            Assert.AreNotSame(res, expList);
        }



        [TestMethod()]
        public void RandomizeRecipesSelectionNotSameValuesCountFailTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "GoDiet\\DietData";
            var res = MyMethodsLib.RandomizeRecipesSelection("1200", "Yes", path,
                "b.json", "b2.json", "l.json", "d.json");

            List<int> expList = new List<int>() { 1, 1, 1, 1, 1, 1 };
            Assert.AreNotSame(res.Count, expList.Count);
        }

        [TestMethod()]
        public void SendMailTest()
        {
            bool exp = false;
            bool res = MyMethodsLib.SendMail("This is email", "ladygoodwill");
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void SendMailFailTest()
        {
            bool exp = true;
            bool res = MyMethodsLib.SendMail("This is email", "ladygoodwill");
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void SendMailExceptionThrownTest()
        {
            try
            {
                bool res = MyMethodsLib.SendMail("This is email", "ladygoodwill");
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetDatesOfInputWeightTest()
        {
            List<string> emptyList = new List<string>();
            int exp = emptyList.Count;
            string con = MyMethodsLib.GetConnectionString();
            List<string> listRes = MyMethodsLib.GetDatesOfInputWeight("ladygoodwill", con);
            int res = listRes.Count;
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void GetDatesOfInputWeightFailTest()
        {
            List<string> emptyList = new List<string>();
            int exp = emptyList.Count + 1;
            string con = MyMethodsLib.GetConnectionString();
            List<string> listRes = MyMethodsLib.GetDatesOfInputWeight("ladygoodwill", con);
            int res = listRes.Count;
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void GetDatesOfInputWeightExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                List<string> listRes = MyMethodsLib.GetDatesOfInputWeight("ladygoodwill", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void GetWeightInputsTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            List<string> exp = new List<string>();
            var res = MyMethodsLib.GetWeightInputs("ladygoodwill", con);
            Assert.AreEqual(exp.Count, res.Count);
        }

        [TestMethod()]
        public void GetWeightInputsFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            List<string> exp = new List<string>() { "1" };
            var res = MyMethodsLib.GetWeightInputs("ladygoodwill", con);
            Assert.AreNotEqual(exp.Count, res.Count);
        }

        [TestMethod()]
        public void GetWeightInputsExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                var res = MyMethodsLib.GetWeightInputs("ladygoodwill", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void SetDietModeDbsTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            string exp = "M: Intense";
            string res = MyMethodsLib.SetDietModeDbs("ladygoodwill", "M: Intense", con);
            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void SetDietModeDbsFailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            string exp = "M:";
            string res = MyMethodsLib.SetDietModeDbs("ladygoodwill", "M: Intense", con);
            Assert.AreNotEqual(exp, res);
        }

        [TestMethod()]
        public void SetDietModeDbsExceptionThrownTest()
        {
            string exp = "";
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                exp = MyMethodsLib.SetDietModeDbs("ladygoodwill", "M: Intense", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void ChangeUserNameTest()
        {
            bool check = false;
            string con = MyMethodsLib.GetConnectionString();
            MyMethodsLib.ChangeUserName("ladygoodwill", "Annannna", con);
            Assert.AreEqual(check, MyMethodsLib.checkForTesting);
        }

        public void ChangeUserNameExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                MyMethodsLib.ChangeUserName("ladygoodwill", "annnanna", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void ChangeUserSurNameTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            bool check = false;
            MyMethodsLib.ChangeUserSurName("ladygoodwill", "Annannna", con);
            Assert.AreEqual(check, MyMethodsLib.checkForTesting);
        }

        [TestMethod()]
        public void ChangeUserSurNameExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                MyMethodsLib.ChangeUserSurName("ladygoodwill", "Abrakadabra", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void ChangeEmailTest()
        {
            string con = MyMethodsLib.GetConnectionString();
            bool exp = false;
            MyMethodsLib.ChangeEmail("ladygoodwill", "anyemail@com", con);
            Assert.AreEqual(exp, MyMethodsLib.checkForTesting);
        }

        [TestMethod()]
        public void ChangeEmailExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                MyMethodsLib.ChangeEmail("ladygoodwill", "anyemail@com", con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void RecordRecipesToDbsTest()
        {
            bool check = false;
            List<int> example = new List<int>() { 1, 1, 1, 1, 1 };
            string con = MyMethodsLib.GetConnectionString();
            MyMethodsLib.RecordRecipesToDbs(example, 999, con);


            Assert.AreEqual(check, MyMethodsLib.checkForTesting);
        }

        [TestMethod()]
        public void RecordRecipesToDbsExceptionThrownTest()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                List<int> example = new List<int>() {1, 1, 1, 1, 1, 1, 1 };
                MyMethodsLib.RecordRecipesToDbs(example, 1, con);
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void RecordRecipesToDbsExceptionThrown2Test()
        {
            try
            {
                string con = MyMethodsLib.GetConnectionString();
                List<int> example = new List<int>() { 1, 1, 1, 1, 1};
                MyMethodsLib.RecordRecipesToDbs(example, 1, con);
                Assert.Fail();
            }
            catch (Exception) { }
        }


        [TestMethod()]
        public void SetUpRecipeTest()
        {
            bool check = true;
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "GoDiet\\DietData";
            DateTime example = new DateTime(2019, 3, 28);
            MyMethodsLib.SetUpRecipe(path, "b.json", 1, example, "anyname");
            Assert.AreEqual(check, MyMethodsLib.checkForTesting);
        }

        [TestMethod()]
        public void SetUpRecipeWrongPathTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "\\some\\GoDiet\\DietData";
            DateTime example = new DateTime(2019, 3, 28);
            try
            {
                MyMethodsLib.SetUpRecipe(path, "b.json", 1, example, "anyname");
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void SetUpRecipeWrongDateFormatTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "\\GoDiet\\DietData";
            
            try
            {
                DateTime example = new DateTime(2019, 19, 28);
                MyMethodsLib.SetUpRecipe(path, "b.json", 1, example, "anyname");
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void SetUpRecipeWrongJsonNameTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "\\GoDiet\\DietData";
            DateTime example = new DateTime(2019, 3, 28);
            try
            {
                MyMethodsLib.SetUpRecipe(path, "baba.json", 1, example, "anyname");
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void SetUpRecipeWrongIDTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "\\GoDiet\\DietData";
            DateTime example = new DateTime(2019, 3, 28);
            try
            {
                int ID = -100;
                MyMethodsLib.SetUpRecipe(path, "b.json", ID , example, "anyname");
                Assert.Fail();
            }
            catch (Exception) { }
        }

        [TestMethod()]
        public void SetUpRecipeWrongRTFNameTest()
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "\\GoDiet\\DietData";
            DateTime example = new DateTime(2019, 3, 28);
            try
            {
                string name = "'-";
                MyMethodsLib.SetUpRecipe(path, "b.json", 1, example, name);
                Assert.Fail();
            }
            catch (Exception) { }
        }


        [TestMethod()]
        public void getUsernameTest()
        {
            string exp = "ladygoodwill";
            string res = MyMethodsLib.getUsername("ladygoodwill");

            Assert.AreEqual(exp, res);
        }

        [TestMethod()]
        public void getUsernameFailTest()
        {
            string exp = "lala";
            string res = MyMethodsLib.getUsername("ladygoodwill");
            Assert.AreNotEqual(res, exp);
        }
    }

    // TODO
    // Coverage testing
    // checking if methods work for setting values or getting them from dbs ///uhhh
    // Code blocks and integration testing
}