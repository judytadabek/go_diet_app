using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using System.IO;
using Newtonsoft.Json;
using SautinSoft.Document;
using System.Windows;
using System.Drawing.Printing;
using Microsoft.Office.Interop.Outlook;

namespace GoDiet
{
    public class MyMethodsLib
    {
        public static string GetConnectionString()
        {
            string connection = "";
            string part = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename=";
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string connectionPath = getPath + "dbs\\GODIETCUSTINFO.MDF";
            string part2 = ";Integrated Security = True";
            connection = part + connectionPath + part2;
            return connection;
        }

        public static bool CallLogisticRegression(string weight, string Username, string _trainDataPath)
        {
            bool dietResult = false;
            MLContext mlContext = new MLContext(seed: 0);
            var model = BinaryClassML.Train(mlContext, _trainDataPath);
            BinaryClassML.Evaluate(mlContext, model);
            // string weight = weightBox.Text;
            string con = MyMethodsLib.GetConnectionString();
            string height = GetHeightFromDbs(Username, con).ToString();
            string bmi = CalculateBMI(weight, height);
            dietResult = BinaryClassML.Predict(mlContext, model, bmi);
            return dietResult;
        }

        //method to get diet mode:
        public static string GetDietModeFromDbs(string Username, string connectionString)
        {
            string mode = "";
            string curDir = Directory.GetCurrentDirectory();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select DietMode from [tblMeasures] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            mode = dataReader.GetString(0);
                        }
                    }
                    dataReader.Close();
                    connection.Close();

                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return mode;
        }

        //method to get veggie option from DBS
        public static string GetVeggieOptionFromDbs(string Username, string connectionStr)
        {
            string veggie = "";
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select Vegetarian from [tblUserNamePassw] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            veggie = dataReader.GetString(0);

                        }
                    }
                    dataReader.Close();
                    connection.Close();

                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return veggie;
        }

        //method to get weight from dbs
        public static string GetWeightFromDbs(string Username, string connectStr)
        {
            string weight = "";
            using (SqlConnection connection = new SqlConnection(connectStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select * from [tblMeasures] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            weight = dataReader.GetInt32(3).ToString();
                        }
                    }
                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                    connection.Close();
                }
            }
            return weight;
        }

        // method to get gender from dbs
        public static string GetGenderFromDbs(string Username, string connStr)
        {
            string gender = "";
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select Gender from [tblUserNamePassw] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            gender = dataReader.GetString(0);
                        }
                    }
                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                     connection.Close();
                }
            }
            return gender;
        }

        public static string GetEmailFromDbs(string Username, string connStr)
        {
            string email = "";
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select Email from [tblOtherInfo] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            email = dataReader.GetString(0);
                        }
                    }
                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                    connection.Close();
                }
            }
            return email;
        }

        public static int GetMeasurementNoWithTodayDate(string Username, string connStr)
        {
            int MeasurementNo = 0;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                var Date = DateTime.Now.Date;
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select MeasurementNo from [tblMeasures] where Username=@Username AND Date=@Date"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Parameters.AddWithValue("@Date", Date);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            MeasurementNo = dataReader.GetInt32(0);
                        }
                        dataReader.Close();
                        connection.Close();
                    }
                }
                catch (System.Exception)
                {
                    //MessageBox.Show("Oh no, more work to do !");
                    connection.Close();
                }
            }
            return MeasurementNo;
        }

        public static DateTime GetMealDate(int MeasurementNo, string connStr)
        {
            DateTime mealDate = new DateTime().Date;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select Date4Meal from [tblDailyMealSet] where MeasurementNo=@MeasurementNo"
                };
                cmd.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            mealDate = dataReader.GetDateTime(0);
                        }
                    }

                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                    connection.Close();
                }
            }
            return mealDate;
        }

        public static int GetBreakfastID(int MeasurementNo, string connStr)
        {
            int breakID = -1;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select BreakfastID from [tblDailyMealSet] where MeasurementNo=@MeasurementNo"
                };
                cmd.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            breakID = dataReader.GetInt32(0);
                        }
                    }

                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                        connection.Close();
                }
            }
            return breakID;
        }

        public static int GetBreakfast2ID(int MeasurementNo, string connStr)
        {
            int break2ID = -1;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select Breakfast2ID from [tblDailyMealSet] where MeasurementNo=@MeasurementNo"
                };
                cmd.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            break2ID = dataReader.GetInt32(0);
                        }
                    }

                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return break2ID;

        }

        public static int GetLunchID(int MeasurementNo, string connStr)
        {
            int lunchID = -1;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select LunchID from [tblDailyMealSet] where MeasurementNo=@MeasurementNo"
                };
                cmd.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            lunchID = dataReader.GetInt32(0);
                        }
                    }

                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return lunchID;
        }

        public static int GetDinnerID(int MeasurementNo, string connStr)
        {
            int dinnerID = -1;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select DinnerID from [tblDailyMealSet] where MeasurementNo=@MeasurementNo"
                };
                cmd.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            dinnerID = dataReader.GetInt32(0);
                        }
                    }

                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return dinnerID;
        }

        public static int GetCaloriesToConsume(int MeasurementNo, string connStr)
        {
            int calories = -1;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select TotalCaloriesNo from [tblDailyMealSet] where MeasurementNo=@MeasurementNo"
                };
                cmd.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                cmd.Connection = connection;
                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            calories = dataReader.GetInt32(0);
                        }
                    }

                    dataReader.Close();
                    connection.Close();
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return calories;
        }

        ///* this method is to be used within another method
        public static string CalculateBMI(string weight, string height)
        {
            string bmi;
            float floatWeight = float.Parse(weight);
            float floatHeight = float.Parse(height);
            float floatBMI = floatWeight / (floatHeight / 100 * floatHeight / 100);
            bmi = floatBMI.ToString();
            return bmi;
        }

        //method to calculate the proper weight - 21.7 this is the mean of BMI indicator
        public static double GetProperWeightCalculation(string Username)
        {
            double weightDesired = 0;
            double height = GetHeightFromDbs(Username, GetConnectionString());
            double bmiDesired = 24;
            //calculate desired weight
            weightDesired = (bmiDesired * (height / 100) * (height / 100));
            return weightDesired;
        }

        //method to get the height from dbs
        public static int GetHeightFromDbs(string Username, string connStr)
        {
            int heightInt = 0;
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                //try
                //{
                    SqlCommand cmd = new SqlCommand
                    {
                        CommandText = "select Height from [tblMeasures] where Username=@Username"
                    };
                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Height", heightInt);
                    cmd.Connection = connection;
                    try
                    {
                        connection.Open();
                        SqlDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                heightInt = dataReader.GetInt32(0);
                            }
                        }
                        dataReader.Close();
                        connection.Close();
                    }
                    catch (System.Exception)
                    {
                        //MessageBox.Show("Something went wrong");
                    }
               // }
                //catch (System.Exception)
                //{
                    //MessageBox.Show("Connection with database went wrong!");
               // }

            }
            return heightInt;
        }

        //method to calculate the kg to loose
        public static double KgToLoose(string Username, string weight)
        {

            Double.TryParse(weight, out double weightCurrent);
            double weightDesired = GetProperWeightCalculation(Username);
            double weightToLoose = weightCurrent - weightDesired;
            return weightToLoose;
        }

        //method to output the result to the user
        public static string WeightResultOutput(string Username, double kgToLoose, string weight)
        {
            //weight must be passed as weightBox.Text
            string userOutputToBox = "";
            double weightRes = KgToLoose(Username, weight);
            //string userOutputToBox = "";
            if (weightRes <= 0)
            {
                userOutputToBox = "0";
            }
            else
            {
                userOutputToBox = weightRes.ToString("0.##");
            }
            return userOutputToBox;
        }

        //method to calculate number of days to achieve goal
        public static string DaysToAchieveGoal(string kgToLoose, double ratio)
        {
            string daysNo;
            double.TryParse(kgToLoose, out double kg2L);
            double days2AchieveGoal = kg2L / ratio + 1;
            int days = (int)Math.Round(days2AchieveGoal);

            return days >= 0 ? (daysNo = days.ToString()) : "N/A";

        }

        //method to calculate weeks (7) or months (30)
        public static string TimeToAchieveGoal(string days, int noOfDaysInWeekOrMonths)
        {
            string timeUnit;
            int.TryParse(days, out int daysInt);
            double timeDbl = daysInt / noOfDaysInWeekOrMonths;
            decimal timeDec = (decimal)Math.Round(timeDbl, 1);


            return timeUnit = timeDec.ToString();

        }
        //number of calories to eat daily associated with the diet model chosen
        public static int CaloriesIntake(string Username)
        {
            int calories = 0;
            string dietMode = GetDietModeFromDbs(Username, GetConnectionString());
            string gender = GetGenderFromDbs(Username, GetConnectionString());

            //MAN
            //fast pace: 1500 - 1600
            //medium pace: 1600 - 1800
            //slow pace: 1800 - 2000
            if (gender == "Male")
            {
                if (dietMode.ToLower() == "not set" || dietMode == "")
                {
                    calories = 0;
                }
                else if (dietMode == "M: Slow")
                {
                    calories = 2000;
                }
                else if (dietMode == "M: Steady")
                {
                    calories = 1800;
                }
                else if (dietMode == "M: Intense")
                {
                    calories = 1600;
                }
                else
                {
                    //MessageBox.Show("Something went wrong??");

                }
            }

            //WOMAN
            ////fast pace: 1200 - 1450 daily
            //medium pace: 1450 - 1750
            //slow pace: 1750 - 1900
            else if (gender == "Female")
            {
                if (dietMode.ToLower() == "not set" || dietMode == "")
                {
                    calories = 0;
                }
                else if (dietMode == "M: Slow")
                {
                    calories = 1900;
                }
                else if (dietMode == "M: Steady")
                {
                    calories = 1750;
                }
                else if (dietMode == "M: Intense")
                {
                    calories = 1450;
                }
                else
                {
                    // MessageBox.Show("Something went wrong??");

                }
            }
            else
            {
                //MessageBox.Show("Oppps... Something wrong.");
            }
            return calories;
        }

        public static List<Item> LoadJson(string path, string jsonName)
        {
            string fullPath = Path.Combine(path, jsonName);
            List<Item> items = new List<Item>();
            using (StreamReader r = new StreamReader(fullPath))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<Item>>(json);
            }
            return items;
        }

        public class Item
        {
            public int ID;
            public string Name;
            public int CaloriesNo;
            public string Description;
            public string VeggieOption;
            public string Ingredients;
            public int Gram;
            public int Proteins;
            public int Carbons;
            public int Fats;
        }

        public static List<int> RandomizeRecipesSelection(string caloriesIntake, string veggieOption, string path, string breakJsonName,
            string break2JsonName, string lunchJsonName, string dinnerJsonName)
        {
            var breakfastRecipesJsonContent = LoadJson(path, breakJsonName);
            var break2RecipesJsonContent = LoadJson(path, break2JsonName);
            var lunchRecipesJsonContent = LoadJson(path, lunchJsonName);
            var dinnerRecipesJsonContent = LoadJson(path, dinnerJsonName);

            //list to store all recipies for breakfast for example
            List<Dictionary<int, int>> breakRecipes = new List<Dictionary<int, int>>();
            List<Dictionary<int, int>> break2Recipes = new List<Dictionary<int, int>>();
            List<Dictionary<int, int>> lunchRecipes = new List<Dictionary<int, int>>();
            List<Dictionary<int, int>> dinnerRecipes = new List<Dictionary<int, int>>();
            //we need to get the right recipes based on veggie option
            if (veggieOption == "Yes")
            {
                //use list with ID no and recorded no of calories for randomizing?
                foreach (Item item in breakfastRecipesJsonContent)
                {
                    if (item.VeggieOption == "YES")
                    {
                        Dictionary<int, int> tempDict = new Dictionary<int, int>
                        {
                            { item.ID, item.CaloriesNo }
                        };
                        breakRecipes.Add(tempDict);
                    }
                }
                foreach (Item item in break2RecipesJsonContent)
                {
                    if (item.VeggieOption == "YES")
                    {
                        Dictionary<int, int> tempDict = new Dictionary<int, int>
                        {
                            { item.ID, item.CaloriesNo }
                        };
                        break2Recipes.Add(tempDict);
                    }
                }
                foreach (Item item in lunchRecipesJsonContent)
                {
                    if (item.VeggieOption == "YES")
                    {
                        Dictionary<int, int> tempDict = new Dictionary<int, int>
                        {
                            { item.ID, item.CaloriesNo }
                        };
                        lunchRecipes.Add(tempDict);
                    }
                }
                foreach (Item item in dinnerRecipesJsonContent)
                {
                    if (item.VeggieOption == "YES")
                    {
                        Dictionary<int, int> tempDict = new Dictionary<int, int>
                        {
                            { item.ID, item.CaloriesNo }
                        };
                        dinnerRecipes.Add(tempDict);
                    }
                }
            }
            else if (veggieOption == "No")
            {
                foreach (Item item in breakfastRecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>
                    {
                        { item.ID, item.CaloriesNo }
                    };
                    breakRecipes.Add(tempDict);
                }
                foreach (Item item in break2RecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>
                    {
                        { item.ID, item.CaloriesNo }
                    };
                    break2Recipes.Add(tempDict);
                }
                foreach (Item item in lunchRecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>
                    {
                        { item.ID, item.CaloriesNo }
                    };
                    lunchRecipes.Add(tempDict);
                }
                foreach (Item item in dinnerRecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>
                    {
                        { item.ID, item.CaloriesNo }
                    };
                    dinnerRecipes.Add(tempDict);
                }

            }
            //else { }

            //ok, we have necessary recipes now, what's next?
            //we should randomize them yay
            //get the no of calories
            //1. pick any from the breakfast list,  get the calories no.
            // 2. pick another from the breakfast 2 list, get the calories no
            // 3. pick from the lunch list
            // 4. pick from the dinner list
            //sum up calories no, repeat until threshold will be met (+- 50 kcal difference only)
            //then return the ID numbers for each of the selected recipies

            List<int> recipesSetup = new List<int>();
            int.TryParse(caloriesIntake, out int kcal);
            int kcalCalculated = 0;
            while ((kcalCalculated <= (kcal - 50)) || (kcalCalculated >= (kcal + 20)))
            {

                recipesSetup.Clear();
                kcalCalculated = 0;
                Random rnd = new Random();

                int choice = rnd.Next(breakRecipes.Count());
                Dictionary<int, int> breakResult = breakRecipes.ElementAt(choice);
                kcalCalculated = kcalCalculated + breakResult.Values.ElementAt(0);

                choice = rnd.Next(break2Recipes.Count());
                Dictionary<int, int> break2Res = break2Recipes.ElementAt(choice);
                kcalCalculated = kcalCalculated + break2Res.Values.ElementAt(0);

                choice = rnd.Next(lunchRecipes.Count());
                Dictionary<int, int> lunchRes = lunchRecipes.ElementAt(choice);
                kcalCalculated = kcalCalculated + lunchRes.Values.ElementAt(0);

                choice = rnd.Next(dinnerRecipes.Count());
                Dictionary<int, int> dinnerRes = dinnerRecipes.ElementAt(choice);
                kcalCalculated += dinnerRes.Values.ElementAt(dinnerRes.Count() - 1);
                recipesSetup.Add(breakResult.Keys.ElementAt(0));
                recipesSetup.Add(break2Res.Keys.ElementAt(0));
                recipesSetup.Add(lunchRes.Keys.ElementAt(0));
                recipesSetup.Add(dinnerRes.Keys.ElementAt(0));
                recipesSetup.Add(kcalCalculated);

            }
            return recipesSetup;
        }

        //how to construct for email
        public static bool SendMail(string MessageBody, string Username)
        {
            try
            {
                Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook.NameSpace NS = app.GetNamespace("MAPI");
                Microsoft.Office.Interop.Outlook.MAPIFolder objFolder = NS.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderOutbox);
                Microsoft.Office.Interop.Outlook.MailItem objMail = (Microsoft.Office.Interop.Outlook.MailItem)objFolder.Items.Add(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                //objMail.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatRichText;
                objMail.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;

                objMail.Body = MessageBody;

                objMail.Subject = "Your Recipe For Day " + DateTime.Now.Date.AddDays(1).ToString("dd/MM/yyyy");
                string email = GetEmailFromDbs(Username, GetConnectionString());
                objMail.To = email;
                objMail.CC = "";
                objMail.Send();

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static List<String> GetDatesOfInputWeight(string Username, string connStr)
        {
            List<String> dates = new List<String>();
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select Date from [tblMeasures] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Connection = connection;

                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            var date = dataReader.GetDateTime(0).ToString("dd/MM/yyyy");
                            dates.Add(date);

                        }
                    }
                    dataReader.Close();
                    connection.Close();

                }
                catch (System.Exception)
                {
                   // if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return dates;
        }

        public static List<int> GetWeightInputs(string Username, string connStr)
        {
            List<int> weightInputs = new List<int>();
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select Weight from [tblMeasures] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username);
                cmd.Connection = connection;

                try
                {
                    connection.Open();
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            //var date = dataReader.GetDateTime(0).ToString("dd/MM/yyyy");
                            weightInputs.Add(dataReader.GetInt32(0));

                        }
                    }
                    dataReader.Close();
                    connection.Close();

                }
                catch (System.Exception)
                {
                   // if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return weightInputs;

        }

        // method to set DietMode in dbs
        public static string SetDietModeDbs(string Username, string DietMode, string connStr)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                var Date = DateTime.Now.Date;
                try
                {
                    SqlCommand cmd = new SqlCommand
                    {
                        CommandText = "UPDATE[tblMeasures] SET DietMode = @DietMode WHERE UserName = @UserName and Date = @Date"
                    };
                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Date", Date);
                    cmd.Parameters.AddWithValue("@DietMode", DietMode);
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    //dietModeBox.Text = DietMode;
                }

                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return DietMode;
        }

        public static bool checkForTesting = true;
        // method to change user's name
        public static void ChangeUserName(string UserName, string Name, string connStr)
        {
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand
                    {
                        CommandText = "UPDATE[tblOtherInfo] SET Name = @Name WHERE UserName = @UserName"
                    };
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Your Name is changed successfully");
                    //nameBox.Text = "";
                    MyMethodsLib.checkForTesting = true;
                }

                catch (System.Exception)
                {
                    //MessageBox.Show("Name change sucks.");
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                    MyMethodsLib.checkForTesting = false;
                }

            }
        }

        public static void ChangeUserSurName(string Username, string Surname, string conn)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand
                    {
                        CommandText = "UPDATE[tblOtherInfo] SET Surname = @Surname WHERE UserName = @UserName"
                    };
                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Surname", Surname);
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Your SurName is changed successfully");
                    checkForTesting = true;
                }

                catch (System.Exception)
                {
                    //MessageBox.Show("Surname change sucks.");
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                    checkForTesting = false;
                }

            }
        }

        public static void ChangeEmail(string Username, string Email, string conn)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {

                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand
                    {
                        CommandText = "UPDATE[tblOtherInfo] SET Email = @Email WHERE UserName = @UserName"
                    };
                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();
                    connection.Close();
                    MessageBox.Show("Your Email is changed successfully");
                    checkForTesting = true;
                }

                catch (System.Exception)
                {
                    connection.Close();
                    checkForTesting = false;
                }
            }


        }

        public static void RecordRecipesToDbs(List<int> recipesSetup, int MeasurementNo, string conn)
        {
            using (SqlConnection connection = new SqlConnection(conn))
            {
                try { 
                SqlCommand cmdCheckRowsNo = new SqlCommand
                {
                    CommandText = "SELECT * FROM [tblDailyMealSet]",
                };
                cmdCheckRowsNo.Connection = connection;
                int counter = 0;
                connection.Open();
                SqlDataReader dataReader = cmdCheckRowsNo.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        counter++;
                    }
                }
                dataReader.Close();

                var today = DateTime.Now.Date;
                DateTime tomorrow = today.AddDays(1);
                int BreakfastID = recipesSetup[0];
                int Breakfast2ID = recipesSetup[1];
                int LunchID = recipesSetup[2];
                int DinnerID = recipesSetup[3];
                int TotalCaloriesNo = recipesSetup[4];

                    SqlCommand sqlInsertDailyMealSet = new SqlCommand
                    {
                        CommandText = "INSERT INTO [tblDailyMealSet] (MealSetId, MeasurementNo, BreakfastID, Breakfast2ID, LunchID, DinnerID, TotalCaloriesNo, Date4Meal) VALUES (@MealSetId, @MeasurementNo, @BreakfastID, @Breakfast2ID, @LunchID, @DinnerID, @TotalCaloriesNo, @Date4Meal)"
                    };
                    sqlInsertDailyMealSet.Parameters.AddWithValue("MealSetId", counter);
                    sqlInsertDailyMealSet.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                    sqlInsertDailyMealSet.Parameters.AddWithValue("@BreakfastID", BreakfastID);
                    sqlInsertDailyMealSet.Parameters.AddWithValue("@Breakfast2ID", Breakfast2ID);
                    sqlInsertDailyMealSet.Parameters.AddWithValue("@LunchID", LunchID);
                    sqlInsertDailyMealSet.Parameters.AddWithValue("@DinnerID", DinnerID);
                    sqlInsertDailyMealSet.Parameters.AddWithValue("@TotalCaloriesNo", TotalCaloriesNo);
                    sqlInsertDailyMealSet.Parameters.AddWithValue("@Date4Meal", tomorrow);
                    sqlInsertDailyMealSet.Connection = connection;
                    sqlInsertDailyMealSet.ExecuteNonQuery();
                    connection.Close();
                    checkForTesting = true;
                }
                catch (System.Exception)
                {
                    //if (connection.State == ConnectionState.Open)
                        connection.Close();
                    checkForTesting = false;
                }
            }
        }

        public static void SetUpRecipe(string path, string jsonName, int ID, DateTime recipeDate, string RTFFileName)
        {
            string name = "";
            string caloriesNo = "";
            string description = "";
            string ingredients = "";
            string grams = "";
            string proteins = "";
            string carbons = "";
            string fats = "";
            var jsonContent = MyMethodsLib.LoadJson(path, jsonName);
            foreach (MyMethodsLib.Item item in jsonContent)
            {
                if (item.ID == ID)
                {
                    name = item.Name;
                    caloriesNo = item.CaloriesNo.ToString();
                    description = item.Description;
                    ingredients = item.Ingredients;
                    grams = item.Gram.ToString();
                    proteins = item.Proteins.ToString();
                    carbons = item.Carbons.ToString();
                    fats = item.Fats.ToString();
                    break;
                }
            }
            FileInfo rtf = new FileInfo(RTFFileName);
            DocumentCore rtfdoc = new DocumentCore();
            rtfdoc.Content.Start.Insert(String.Format("RECIPE FOR DAY: \t \t"),
                new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.DarkMagenta, Bold = true, Size = 16 });
            rtfdoc.Content.End.Insert(String.Format(recipeDate.ToString("dddd, dd MMMM yyyy")), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.DarkMagenta, Italic = true, Bold = true, Size = 14 });
            rtfdoc.Content.End.Insert(String.Format("\n"));
            rtfdoc.Content.End.Insert(String.Format("Calories: \t"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Red, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(caloriesNo), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(" kcal"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format("\tGram: \t"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Red, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(grams), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(" g"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format("\tProtein: \t"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Red, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(proteins), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(" g"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format("\tCarbon: \t"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Red, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(carbons), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(" g"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format("\t Fat: \t"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Red, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(fats), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format(" g"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 9 });
            rtfdoc.Content.End.Insert(String.Format("\n"));
            rtfdoc.Content.End.Insert(String.Format(name), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Italic = true, Size = 14, AllCaps = true });
            rtfdoc.Content.End.Insert(String.Format("\n\nIngredients: \n"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 13 });
            rtfdoc.Content.End.Insert(String.Format(ingredients), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Italic = true, Size = 12 });
            rtfdoc.Content.End.Insert(String.Format("\n\nDescription: \n"), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Bold = true, Size = 13 });
            rtfdoc.Content.End.Insert(String.Format(description), new CharacterFormat() { FontName = "Garamont", FontColor = SautinSoft.Document.Color.Black, Italic = true, Size = 12 });
            rtfdoc.Save(rtf.FullName, SaveOptions.RtfDefault);
            checkForTesting = true;

        }

        public static string getUsername(string userName)
        {
            return userName;
        }
    }
}
