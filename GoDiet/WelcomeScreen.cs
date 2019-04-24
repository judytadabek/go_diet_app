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
    public partial class WelcomeScreen : Form
    {
        
        void ClosePreviousWindow() => ActiveForm.Close();
        public WelcomeScreen() => InitializeComponent();
        string Username = InitialWindow.SetUsername;

        public void WelcomeScreen_Load(object sender, EventArgs e)
        {
            string con = MyMethodsLib.GetConnectionString();
            unameBox.Text = Username;
            string height = MyMethodsLib.GetHeightFromDbs(Username, con).ToString();
            weightBox.Text = MyMethodsLib.GetWeightFromDbs(Username, con).ToString();
            BMIBox.Text = MyMethodsLib.CalculateBMI(this.weightBox.Text.ToString(), height);
            double kgToLoose = MyMethodsLib.KgToLoose(Username, weightBox.Text);
            kgBox.Text = MyMethodsLib.WeightResultOutput(Username, kgToLoose, weightBox.Text);
            intakeBox.Text = MyMethodsLib.CaloriesIntake(Username).ToString();
            dietModeBox.Text = MyMethodsLib.GetDietModeFromDbs(Username, MyMethodsLib.GetConnectionString());
            SetupCharts4Projection(Username);
        }

        public void RemoveAccountBtn_Click(object sender, EventArgs e)
        {
            int MeasurementNo = 0;
            DialogResult d_res = MessageBox.Show("Are you sure you want to remove your account with all your data?", "Confirm", MessageBoxButtons.YesNo);
            if (d_res == DialogResult.Yes)
            {
                //code for implementing data removal functionality
                using (
                SqlConnection connection = new SqlConnection(MyMethodsLib.GetConnectionString()))
                {
                    try
                    {
                        // string deleteFromtblMeasures = "DELETE FROM [tblMeasures] WHERE Username=@Username";
                        string deleteFromtblOtherInfo = "DELETE FROM [tblOtherInfo] WHERE Username=@Username";
                        string deleteFromtblUserNamePassw = "DELETE FROM [tblUserNamePassw] WHERE Username=@Username";

                        SqlCommand sqlCmdSelectMeasurementNo = new SqlCommand();
                        SqlCommand sqlCmdDeleteFromtblDailyMealSet = new SqlCommand();
                        //SqlCommand sqlCmdDeleteFromtblMeasures = new SqlCommand();
                        SqlCommand sqlCmdDeleteFromtblOtherInfo = new SqlCommand(deleteFromtblOtherInfo, connection);
                        SqlCommand sqlCmdDeleteFromtblUserNamePassw = new SqlCommand(deleteFromtblUserNamePassw, connection);

                        try
                        {
                            connection.Open();
                            sqlCmdSelectMeasurementNo.CommandType = CommandType.Text;
                            sqlCmdSelectMeasurementNo.CommandText = "select MeasurementNo from [tblMeasures] where Username=@Username";
                            sqlCmdSelectMeasurementNo.Parameters.AddWithValue("@Username", Username);
                            sqlCmdSelectMeasurementNo.Connection = connection;

                            SqlDataReader dataReader = sqlCmdSelectMeasurementNo.ExecuteReader();
                            //connection.Close();
                            if (dataReader.HasRows)
                            {

                                while (dataReader.Read())
                                {
                                    MeasurementNo = dataReader.GetInt32(0);
                                    try
                                    {
                                        sqlCmdDeleteFromtblDailyMealSet.CommandType = CommandType.Text;
                                        sqlCmdDeleteFromtblDailyMealSet.CommandText = "delete from [tblDailyMealSet] WHERE MeasurementNo=@MeasurementNo";
                                        sqlCmdDeleteFromtblDailyMealSet.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                                        sqlCmdDeleteFromtblDailyMealSet.Connection = connection;
                                        sqlCmdDeleteFromtblDailyMealSet.ExecuteNonQuery();
                                        connection.Close();
                                    }
                                    catch (System.Exception)
                                    {
                                        MessageBox.Show("Potentially no records in this table");

                                    }
                                }
                            }
                            dataReader.Close();
                            connection.Close();
                        }
                        catch (System.Exception)
                        {

                        }

                        try
                        {
                            connection.Open();
                            SqlCommand sqlCmdDeleteFromtblMeasures = new SqlCommand("tblMeasuresDelete", connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            //sqlCmdDeleteFromtblMeasures.CommandText = "DELETE FROM [tblMeasures] WHERE Username = @Username";

                            SqlCommand cmdDeleteFromMeasures = new SqlCommand
                            {
                                CommandText = "DELETE FROM [tblMeasures] WHERE Username = @Username"
                            };
                            cmdDeleteFromMeasures.Parameters.AddWithValue("@Username", Username);
                            cmdDeleteFromMeasures.Connection = connection;
                            sqlCmdDeleteFromtblMeasures.Parameters.AddWithValue("@Username", Username);
                            sqlCmdDeleteFromtblMeasures.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (System.Exception)
                        {

                        }

                        try
                        {
                            connection.Open();
                            SqlCommand sqlCmdDeleteFromtblMeasures = new SqlCommand("tblOtherInfoDelete", connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            //sqlCmdDeleteFromtblMeasures.CommandText = "DELETE FROM [tblMeasures] WHERE Username = @Username";

                            SqlCommand cmdDeleteFromMeasures = new SqlCommand
                            {
                                CommandText = "DELETE FROM [tblOtherInfo] WHERE Username = @Username"
                            };
                            cmdDeleteFromMeasures.Parameters.AddWithValue("@Username", Username);
                            cmdDeleteFromMeasures.Connection = connection;
                            sqlCmdDeleteFromtblMeasures.Parameters.AddWithValue("@Username", Username);
                            sqlCmdDeleteFromtblMeasures.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (System.Exception)
                        {

                        }

                        try
                        {
                            connection.Open();
                            SqlCommand sqlCmd = new SqlCommand("tblUserDelete", connection)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            SqlCommand cmd = new SqlCommand
                            {
                                CommandText = "DELETE FROM [tblUserNamePassw] WHERE Username = @Username"
                            };
                            cmd.Parameters.AddWithValue("@Username", Username);
                            cmd.Connection = connection;
                            sqlCmd.Parameters.AddWithValue("@Username", Username);
                            sqlCmd.ExecuteNonQuery();
                            connection.Close();
                        }
                        catch (System.Exception)
                        {

                        }
                    }
                    catch (System.Exception)
                    {

                    }
                    MessageBox.Show("Your Account has been successfully deleted!");
                    Close();
                }
            }
            else if (d_res == DialogResult.No)
            {
                // do nothing :)
            }
        }

        public void ClearBtn_Click(object sender, EventArgs e)
        {
            nameBox.Text = "";
            surnameBox.Text = "";
            yesRadioBtn.Checked = false;
            noRadioBtn.Checked = false;
            GenderBox.SelectedItem = "";
            oldPasswBox.Text = "";
            newPasswBox.Text = "";
            modifLastWeightIn.Text = "";
            emailBx.Text = "";
        }

        public void UpdateBtn_Click(object sender, EventArgs e)
        {
            //declare variables needed for operations
            string Vegetarian = "";
            string Gender = GenderBox.Text;
            string Password = oldPasswBox.Text;
            string ModifWeightIn = modifLastWeightIn.Text;
            if (yesRadioBtn.Checked){ Vegetarian = "Yes";}
            if (noRadioBtn.Checked) { Vegetarian = "No"; }
            string con = MyMethodsLib.GetConnectionString();
            if (nameBox.Text != "") { MyMethodsLib.ChangeUserName(Username, nameBox.Text, con); nameBox.Text = ""; }
            if (surnameBox.Text != "") { MyMethodsLib.ChangeUserSurName(Username, surnameBox.Text, con); surnameBox.Text = ""; }

            if (emailBx.Text != "") { MyMethodsLib.ChangeEmail(Username, emailBx.Text, con); emailBx.Text = ""; }
            using (SqlConnection sqlConnect = new SqlConnection(MyMethodsLib.GetConnectionString()))
            {
                if (Vegetarian != "")
                {
                    try
                    {
                        sqlConnect.Open();
                        SqlCommand sqlCmdUpdateVegetarian = new SqlCommand("VegetarianUpdate", sqlConnect)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlCommand cmdUpdateVegetarian = new SqlCommand
                        {
                            CommandText = "SELECT * from [tblUserNamePassw] WHERE UserName=@UserName"
                        };
                        cmdUpdateVegetarian.Parameters.AddWithValue("@Username", Username);
                        cmdUpdateVegetarian.Parameters.AddWithValue("@Vegetarian", Vegetarian);
                        cmdUpdateVegetarian.Connection = sqlConnect;
                        sqlCmdUpdateVegetarian.Parameters.AddWithValue("@Username", Username);
                        sqlCmdUpdateVegetarian.Parameters.AddWithValue("@Vegetarian", Vegetarian);
                        sqlCmdUpdateVegetarian.ExecuteNonQuery();
                        MessageBox.Show("Vegetarian option was changed!");
                        sqlConnect.Close();
                        yesRadioBtn.Checked = false;
                        noRadioBtn.Checked = false;
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("SQL Query unsuccessful");
                    }
                }

                //code to update Gender
                if (Gender != "")
                {
                    try
                    {
                        sqlConnect.Open();
                        SqlCommand sqlCmdUpdateGender = new SqlCommand("GenderUpdate", sqlConnect)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlCommand sqlCommand = new SqlCommand
                        {
                            CommandText = "SELECT * from [tblUserNamePassw] WHERE UserName=@UserName"
                        };
                        SqlCommand cmdUpdateGender = sqlCommand;
                        cmdUpdateGender.Parameters.AddWithValue("@Username", Username);
                        cmdUpdateGender.Parameters.AddWithValue("@Gender", Gender);
                        cmdUpdateGender.Connection = sqlConnect;
                        sqlCmdUpdateGender.Parameters.AddWithValue("@Username", Username);
                        sqlCmdUpdateGender.Parameters.AddWithValue("@Gender", Gender);
                        sqlCmdUpdateGender.ExecuteNonQuery();
                        MessageBox.Show("Gender changed successfully!");
                        GenderBox.SelectedItem = "";
                        sqlConnect.Close();
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("SQL Query unsuccessful");
                    }
                }

                // fun now... .code to update Password
                if (Password != "")
                {
                    try
                    {
                        sqlConnect.Open();
                        SqlCommand sqlCmdUpdatePassword = new SqlCommand("PasswordUpdate", sqlConnect)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        SqlCommand cmdUpdatePassword = new SqlCommand
                        {
                            CommandText = "SELECT Password from [tblUserNamePassw] WHERE UserName=@UserName"
                        };
                        cmdUpdatePassword.Parameters.AddWithValue("@Username", Username);
                        cmdUpdatePassword.Connection = sqlConnect;
                        try
                        {
                            string oldPassword = "";
                            SqlDataReader dataReader = cmdUpdatePassword.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    oldPassword = dataReader.GetSqlString(0).ToString();
                                }
                            }
                            dataReader.Close();

                            if (Password == oldPassword)
                            {
                                if (newPasswBox.Text != "" && newPasswBox.Text.Count() > 6)
                                {
                                    Password = newPasswBox.Text;
                                }
                                else
                                {
                                    MessageBox.Show("Please make sure your new password contains at least 7 letters!");
                                }
                            }

                        }
                        catch (System.Exception)
                        {
                            MessageBox.Show("Unsuccessful operation!");
                        }

                        sqlCmdUpdatePassword.Parameters.AddWithValue("@Username", Username);
                        sqlCmdUpdatePassword.Parameters.AddWithValue("@Password", Password);
                        sqlCmdUpdatePassword.ExecuteNonQuery();
                        sqlConnect.Close();
                        MessageBox.Show("Password successfullly changed!");
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("SQL Query unsuccessful");
                    }
                }

                //code to update last weight
                if (ModifWeightIn != "")
                {
                    try
                    {
                        var Date = DateTime.Now.Date;
                        SqlCommand cmdUpdateWeight = new SqlCommand
                        {
                            CommandText = "SELECT Date from [tblMeasures] WHERE UserName=@UserName AND Date=@Date"
                        };
                        cmdUpdateWeight.Parameters.AddWithValue("@Username", Username);
                        cmdUpdateWeight.Parameters.AddWithValue("@Weight", ModifWeightIn);
                        cmdUpdateWeight.Parameters.AddWithValue("@Date", Date);
                        cmdUpdateWeight.Connection = sqlConnect;
                        string dateFromDbs = "";
                        
                        SqlCommand sqlCmdUpdateWeight = new SqlCommand();
                        sqlConnect.Open();
                        try
                        {
                            
                            SqlDataReader dataReader = cmdUpdateWeight.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    dateFromDbs = dataReader.GetDateTime(0).Date.ToString();
                                }
                                dataReader.Close();
                                if (dateFromDbs == Date.ToString())
                                {
                                    try
                                    {
                                        sqlCmdUpdateWeight.CommandText = "UPDATE [tblMeasures] SET Weight=@Weight WHERE UserName= @UserName and Date=@Date";
                                        sqlCmdUpdateWeight.Parameters.AddWithValue("@Username", Username);
                                        sqlCmdUpdateWeight.Parameters.AddWithValue("@Weight", ModifWeightIn);
                                        sqlCmdUpdateWeight.Parameters.AddWithValue("@Date", Date);
                                        sqlCmdUpdateWeight.Connection = sqlConnect;
                                        sqlCmdUpdateWeight.ExecuteNonQuery();
                                        sqlConnect.Close();
                                        weightBox.Text = ModifWeightIn;
                                        //string con = MyMethodsLib.GetConnectionString();
                                        string height = MyMethodsLib.GetHeightFromDbs(Username, con).ToString();
                                        BMIBox.Text = MyMethodsLib.CalculateBMI(ModifWeightIn, height);

                                        MessageBox.Show("Today's Weight Input modified successfully!");
                                    }
                                    catch (System.Exception)
                                    {
                                        MessageBox.Show("Ooops");
                                    }
                                }
                                sqlConnect.Close();
                            }
                            else
                            {
                                MessageBox.Show("ATTEMPT FAILED. You can only modify today's input.");
                                sqlConnect.Close();
                            }
                        }
                        catch (System.Exception)
                        {
                            MessageBox.Show("Shit happened");
                            if (sqlConnect.State == ConnectionState.Open)
                                sqlConnect.Close();
                        }
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("SQL Query unsuccessful");
                    }
                }
            }
            double kgToLoose = MyMethodsLib.KgToLoose(Username, weightBox.Text);
            kgBox.Text = MyMethodsLib.WeightResultOutput(Username, kgToLoose, weightBox.Text);

            nameBox.Text = "";
            surnameBox.Text = "";
            oldPasswBox.Text = "";
            newPasswBox.Text = "";
            modifLastWeightIn.Text = "";

        }

        public void Button1_Click(object sender, EventArgs e)
        {
            string con = MyMethodsLib.GetConnectionString();
            int MeasurementNo = MyMethodsLib.GetMeasurementNoWithTodayDate(Username, con);

            var today2 = DateTime.Now.Date;
            var tomorrow2 = today2.AddDays(1);

            var dateForMealFromDbs2 = MyMethodsLib.GetMealDate(MeasurementNo, con);
            if (tomorrow2 != dateForMealFromDbs2)
            {
                string mode = "";
                string kgToLoose = MyMethodsLib.WeightResultOutput(Username, MyMethodsLib.KgToLoose(Username, weightBox.Text), weightBox.Text);
                if (kgToLoose != "0")
                {
                    if (slowModeRadioBtn.Checked)
                    {
                        mode = "M: Slow";
                    }
                    else if (steadyModeRadioBtn.Checked)
                    {
                        mode = "M: Steady";
                    }
                    else if (intenseModeRadioBtn.Checked)
                    {
                        mode = "M: Intense";
                    }
                    else
                    {
                        mode = "";
                        MessageBox.Show("Please, choose Your diet mode!");
                    }
                }
                else
                {
                    mode = "Not set";
                    MessageBox.Show("We cannot help you as Your Weight is perfectly fine!");
                }
                using (SqlConnection connection = new SqlConnection(MyMethodsLib.GetConnectionString()))
                {
                    try
                    {
                        string DietMode = mode;
                        SqlCommand cmd2 = new SqlCommand
                        {
                            CommandText = "UPDATE [tblMeasures] SET DietMode=@DietMode WHERE MeasurementNo=@MeasurementNo"
                        };
                        connection.Open();
                        cmd2.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                        cmd2.Parameters.AddWithValue("@DietMode", DietMode);
                        cmd2.Connection = connection;
                        cmd2.ExecuteNonQuery();
                        connection.Close();
                        dietModeBox.Text = DietMode;
                        MessageBox.Show("Your Diet Mode is set!");
                        if (DietMode != "Not set")
                        {
                            intakeBox.Text = MyMethodsLib.CaloriesIntake(Username).ToString();
                        }
                        else
                        {
                            intakeBox.Text = "N\\A";
                            MessageBox.Show("Sorry, not this time!");
                        }
                    }
                    catch (System.Exception)
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                        MessageBox.Show("Something went wrong with SQL query Line 905");
                    }
                }
            }

            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "DietData";
            string breakJsonName = "b.json";
            string break2JsonName = "b2.json";
            string lunchJsonName = "l.json";
            string dinnerJsonName = "d.json";
            //IF THERE IS ALREADY DATE FOR TOMORROW, DO NOT ALLOW TO CHANGE DIET MODE,
            //OUTPUT THE MESSAGE BOX WITH RELEVANT INFO
            //START DOING METHODS FOR OUTPUTTING TEXT TO RICH BOXES
            //REMEMBER, THERE MIGHT BE NO ENTRY IN THIS DBS
            //SO REMEMBER TO CHECK IT
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var dateForMealFromDbs = MyMethodsLib.GetMealDate(MeasurementNo, con);
            if (tomorrow == dateForMealFromDbs)
            {
                MessageBox.Show("You can't change diet mode today, try tomorrow!");
            }
            else
            {

                var randomRecipes = MyMethodsLib.RandomizeRecipesSelection(MyMethodsLib.CaloriesIntake(Username).ToString(), MyMethodsLib.
                    GetVeggieOptionFromDbs(Username, con), path, breakJsonName, break2JsonName, lunchJsonName, dinnerJsonName);
                MyMethodsLib.RecordRecipesToDbs(randomRecipes, MeasurementNo, con);
                int breakID = MyMethodsLib.GetBreakfastID(MeasurementNo, con);
                int break2ID = MyMethodsLib.GetBreakfast2ID(MeasurementNo, con);
                int lunchID = MyMethodsLib.GetLunchID(MeasurementNo, con);
                int dinnerID = MyMethodsLib.GetDinnerID(MeasurementNo, con);
                string break2RTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_breakfast2.rtf";
                string breakRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_breakfast.rtf";
                string lunchRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_lunch.rtf";
                string dinnerRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_dinner.rtf";
                //breakfast
                MyMethodsLib.SetUpRecipe(path, breakJsonName, breakID, tomorrow, breakRTFName);
                //breakfast2
                MyMethodsLib.SetUpRecipe(path, break2JsonName, break2ID, tomorrow, break2RTFName);
                //lunch
                MyMethodsLib.SetUpRecipe(path, lunchJsonName, lunchID, tomorrow, lunchRTFName);
                //dinner
                MyMethodsLib.SetUpRecipe(path, dinnerJsonName, dinnerID, tomorrow, dinnerRTFName);
                //MessageBox.Show("Your Diet Mode is selected!");
            }
        }

        public void Button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(MyMethodsLib.GetConnectionString()))
            {
                string dailyWeight = dayWeightBox.Text.ToString();

                if (dailyWeight != "")
                {
                    int.TryParse(dailyWeight, out int Weight);
                    try
                    {
                        var Date = DateTime.Now.Date;
                        SqlCommand cmdCheckLastInputDate = new SqlCommand
                        {
                            CommandText = "SELECT Date FROM [tblMeasures] WHERE Username=@Username"
                        };
                        cmdCheckLastInputDate.Parameters.AddWithValue("@Username", Username);
                        cmdCheckLastInputDate.Parameters.AddWithValue("@Date", Date);
                        List<DateTime> dates = new List<DateTime>();
                        cmdCheckLastInputDate.Connection = connection;

                        try
                        {
                            connection.Open();
                            SqlDataReader dataReader = cmdCheckLastInputDate.ExecuteReader();
                            if (dataReader.HasRows)  //it has rows, proved
                            {
                                while (dataReader.Read()) //this while does not go anywhere, why?
                                {
                                    var Date2 = dataReader.GetDateTime(0).Date;
                                    dates.Add(Date2);
                                }
                                dataReader.Close();

                                SqlCommand cmdCheckRowsNo = new SqlCommand
                                {
                                    CommandText = "SELECT * FROM [tblMeasures]",
                                    Connection = connection
                                };

                                int counter = 0;

                                SqlDataReader dataReaderForAllTable = cmdCheckRowsNo.ExecuteReader();

                                while (dataReaderForAllTable.Read())
                                {
                                    counter++;
                                }
                                dataReaderForAllTable.Close();
                                Int32 Height = 0;
                                SqlCommand cmdUserHeight = new SqlCommand
                                {
                                    CommandText = "SELECT Height FROM [tblMeasures] WHERE Username=@Username"
                                };
                                cmdUserHeight.Parameters.AddWithValue("@Username", Username);
                                cmdUserHeight.Parameters.AddWithValue("@Height", Height);
                                cmdUserHeight.Connection = connection;
                                SqlDataReader dataReader4Height = cmdUserHeight.ExecuteReader();

                                if (dataReader4Height.HasRows)
                                {
                                    while (dataReader4Height.Read())
                                    {
                                        Height = dataReader4Height.GetInt32(0);
                                    }
                                    dataReader4Height.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Something went wrong: Line 595; DataReader doesn't have rows!");
                                }
                                string comparedDate = "";
                                foreach (DateTime dd in dates)
                                {
                                    if (dd == Date)
                                    {
                                        comparedDate = dd.ToString();
                                    }
                                }
                                if (comparedDate == "")
                                {
                                    try
                                    {
                                        SqlCommand sqlInsertNewWeightRecord = new SqlCommand();
                                        int MeasurementNo = counter;
                                        sqlInsertNewWeightRecord.CommandText = "INSERT INTO [tblMeasures] (MeasurementNo, Username, Height, Weight, Date) VALUES (@MeasurementNo, @Username, @Height, @Weight, @Date)";
                                        sqlInsertNewWeightRecord.Parameters.AddWithValue("@MeasurementNo", MeasurementNo);
                                        sqlInsertNewWeightRecord.Parameters.AddWithValue("@Username", Username);
                                        sqlInsertNewWeightRecord.Parameters.AddWithValue("@Height", Height);
                                        sqlInsertNewWeightRecord.Parameters.AddWithValue("@Weight", Weight);
                                        sqlInsertNewWeightRecord.Parameters.AddWithValue("@Date", Date);
                                        sqlInsertNewWeightRecord.Connection = connection;
                                        sqlInsertNewWeightRecord.ExecuteNonQuery();
                                        weightBox.Text = Weight.ToString();
                                        dayWeightBox.Clear();
                                        connection.Close();
                                        string bmi = MyMethodsLib.CalculateBMI(Weight.ToString(), Height.ToString());
                                        BMIBox.Text = bmi;

                                        MessageBox.Show("Your Today's Weight has been recorded in database!");

                                    }
                                    catch (System.Exception)
                                    {
                                        MessageBox.Show("Something went wrong: Line 1254");
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("You can't add more data today, you can modify today input only.");
                                    dayWeightBox.Clear();
                                }
                            }
                        }
                        catch (System.Exception)
                        {
                            MessageBox.Show("Something went wrong: Line 630");
                        }
                    }
                    catch (System.Exception)
                    {
                        MessageBox.Show("Something went wrong: Line 628");
                    }
                }
            }

            kgBox.Text = MyMethodsLib.WeightResultOutput(Username, MyMethodsLib.KgToLoose(Username, weightBox.Text), weightBox.Text);


            SetupCharts4Projection(Username);
            string currentWeight = weightBox.Text;
            double.TryParse(currentWeight, out double currentWeightDouble);
            string daysToAchieveGoalsSlowMode = slowModeBox.Text;
            int.TryParse(daysToAchieveGoalsSlowMode, out int daysAllProcessSlowMode);

            int gapDivisor = daysAllProcessSlowMode / 6;
            int counter2 = gapDivisor;
            while (counter2 < daysAllProcessSlowMode - 1)
            {
                this.slowModeChart.Series["Weight"].Points.AddXY(counter2, currentWeightDouble - 0.1 * counter2);
                counter2 += gapDivisor;
            }
            this.slowModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.slowModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";

            //for steadyModeChart
            string daysToAchieveGoalSteadyMode = steadyModeBox.Text;
            int.TryParse(daysToAchieveGoalSteadyMode, out int daysAllProcessSteadyMode);

            int gapDivisorSteady = daysAllProcessSteadyMode / 6;
            int counterSteady = gapDivisorSteady;

            while (counterSteady < daysAllProcessSteadyMode - 1)
            {
                this.steadyModeChart.Series["Weight"].Points.AddXY(counterSteady, currentWeightDouble - 0.2 * counterSteady);
                counterSteady += gapDivisorSteady;
            }
            this.steadyModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.steadyModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";

            //for intenseModeChart
            string daysToAchieveGoalIntenseMode = intenseModeBox.Text;
            int.TryParse(daysToAchieveGoalIntenseMode, out int daysAllProcessIntenseMode);

            int gapDivisorIntense = daysAllProcessIntenseMode / 6;
            int counterIntense = gapDivisorIntense;

            while (counterIntense < daysAllProcessIntenseMode - 1)
            {
                this.intenseModeChart.Series["Weight"].Points.AddXY(counterIntense, currentWeightDouble - 0.3 * counterIntense);
                counterIntense += gapDivisorIntense;
            }
            this.intenseModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.intenseModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";


        }

        public void BMIBox_TextChanged(object sender, EventArgs e)
        {
            string con = MyMethodsLib.GetConnectionString();
            string height = MyMethodsLib.GetHeightFromDbs(Username, con).ToString();
            this.BMIBox.Text = MyMethodsLib.CalculateBMI(this.weightBox.Text.ToString(), height);
            this.WeightResultBox.Text = "Press";
            double.TryParse(this.BMIBox.Text, out double valToCompare);
            if (valToCompare < 24.9)
            {
                dietModeBox.Text = "Not set";
                string dietModeBoxToBeSet = MyMethodsLib.SetDietModeDbs(Username, dietModeBox.Text, con);
                dietModeBox.Text = dietModeBoxToBeSet;
                intakeBox.Text = "0";
            }
        }

        public void WeightResultBtn_Click(object sender, EventArgs e)
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            bool dietResult = MyMethodsLib.CallLogisticRegression(weightBox.Text, Username,_trainDataPath);
            string inputStrWeightResult = "";
            if (dietResult == true)
            {
                inputStrWeightResult = "Above Norm";
            }
            else
            {
                inputStrWeightResult = "Normal";
            }

            WeightResultBox.Text = inputStrWeightResult;
        }

        public void RefreshGraphsBtn_Click(object sender, EventArgs e)
        {
            //methods to refresh charts and data included in it
            SetupCharts4Projection(Username);
            string currentWeight = weightBox.Text;
            double.TryParse(currentWeight, out double currentWeightDouble);
            string daysToAchieveGoalsSlowMode = slowModeBox.Text;
            int.TryParse(daysToAchieveGoalsSlowMode, out int daysAllProcessSlowMode);

            int gapDivisor = daysAllProcessSlowMode / 6;
            int counter = gapDivisor;
            while (counter < daysAllProcessSlowMode - 1)
            {
                this.slowModeChart.Series["Weight"].Points.AddXY(counter, currentWeightDouble - 0.1 * counter);
                counter += gapDivisor;
            }
            this.slowModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.slowModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";

            //for steadyModeChart
            string daysToAchieveGoalSteadyMode = steadyModeBox.Text;
            int.TryParse(daysToAchieveGoalSteadyMode, out int daysAllProcessSteadyMode);

            int gapDivisorSteady = daysAllProcessSteadyMode / 6;
            int counterSteady = gapDivisorSteady;

            while (counterSteady < daysAllProcessSteadyMode - 1)
            {
                this.steadyModeChart.Series["Weight"].Points.AddXY(counterSteady, currentWeightDouble - 0.2 * counterSteady);
                counterSteady += gapDivisorSteady;
            }
            this.steadyModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.steadyModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";

            //for intenseModeChart
            string daysToAchieveGoalIntenseMode = intenseModeBox.Text;
            int.TryParse(daysToAchieveGoalIntenseMode, out int daysAllProcessIntenseMode);

            int gapDivisorIntense = daysAllProcessIntenseMode / 6;
            int counterIntense = gapDivisorIntense;

            while (counterIntense < daysAllProcessIntenseMode - 1)
            {
                this.intenseModeChart.Series["Weight"].Points.AddXY(counterIntense, currentWeightDouble - 0.3 * counterIntense);
                counterIntense += gapDivisorIntense;
            }
            this.intenseModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.intenseModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";


        }

        public void SetupCharts4Projection(string Username)
        {
            double weightToLoose = MyMethodsLib.KgToLoose(Username, weightBox.Text);
            string slowDays = MyMethodsLib.DaysToAchieveGoal(weightToLoose.ToString(), 0.1);
            string steadyDays = MyMethodsLib.DaysToAchieveGoal(weightToLoose.ToString(), 0.2);
            string intenseDays = MyMethodsLib.DaysToAchieveGoal(weightToLoose.ToString(), 0.3);
            slowModeBox.Text = slowDays;
            slowWeeksBox.Text = MyMethodsLib.TimeToAchieveGoal(slowDays, 7);
            slowMonthsBox.Text = MyMethodsLib.TimeToAchieveGoal(slowDays, 30);

            steadyModeBox.Text = steadyDays;
            steadyWeeksBox.Text = MyMethodsLib.TimeToAchieveGoal(steadyDays, 7);
            steadyMonthsBox.Text = MyMethodsLib.TimeToAchieveGoal(steadyDays, 30);

            intenseModeBox.Text = intenseDays;
            intenseWeeksBox.Text = MyMethodsLib.TimeToAchieveGoal(intenseDays, 7);
            intenseMonthsBox.Text = MyMethodsLib.TimeToAchieveGoal(intenseDays, 30);
        }

        public void LoadRecipesBtn_Click(object sender, EventArgs e)
        {
            //method to load rtf files into rich text boxes
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            string break2RTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_breakfast2.rtf";
            string breakRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_breakfast.rtf";
            string lunchRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_lunch.rtf";
            string dinnerRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_dinner.rtf";
            string path = Environment.CurrentDirectory;
            try
            {
                breakfastRichTxtBx.LoadFile(Path.Combine(path, breakRTFName));
                break2RichTxtBx.LoadFile(Path.Combine(path, break2RTFName));
                lunchRichTxtBx.LoadFile(Path.Combine(path, lunchRTFName));
                dinnerRichTxtBx.LoadFile(Path.Combine(path, dinnerRTFName));
            }
            catch (System.Exception)
            {
                MessageBox.Show("Please, update Your weight, select diet mode and then try again.");
            }
        }

        public Font fontUsed;
        public StreamReader reader;

        public void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
        {
            //Get the Graphics object  
            Graphics g = ppeArgs.Graphics;
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            //Read margins from PrintPageEventArgs  
            float leftMargin = ppeArgs.MarginBounds.Left;
            float topMargin = ppeArgs.MarginBounds.Top;
            string line = null;
            //Calculate the lines per page on the basis of the height of the page and the height of the font  
            linesPerPage = ppeArgs.MarginBounds.Height / fontUsed.GetHeight(g);
            //Now read lines one by one, using StreamReader  
            while (count < linesPerPage && ((line = reader.ReadLine()) != null))
            {
                //Calculate the starting position  
                yPos = topMargin + (count * fontUsed.GetHeight(g));
                //Draw text  
                g.DrawString(line, fontUsed, Brushes.Black, leftMargin, yPos, new StringFormat());
                //Move to next line  
                count++;
            }
            //If PrintPageEventArgs has more pages to print  
            if (line != null)
            {
                ppeArgs.HasMorePages = true;
            }
            else
            {
                ppeArgs.HasMorePages = false;
            }
        }

        public void printBtn_Click(object sender, EventArgs e)
        {

        }

        public void EmailBtn_Click(object sender, EventArgs e)
        {
            string MessageBody = "BREAKFAST\n" + breakfastRichTxtBx.Text.ToString() + "\n\nBREAKFAST 2\n" + break2RichTxtBx.Text.ToString() + "\n\nLUNCH\n" + lunchRichTxtBx.Text.ToString() +
                    "\n\nDINNER\n" + dinnerRichTxtBx.Text.ToString();
            bool sentstatus = MyMethodsLib.SendMail(MessageBody, Username);
            if (sentstatus == true)
            {
                MessageBox.Show("Email was sent!");
            }
            else
            {
                MessageBox.Show("Sending e-mail failed. Update Your Email and try again!");
            }
        }

        public void Button1_Click_2(object sender, EventArgs e)
        {
            //it must be taken from dbs the dates and weights in order to output
            this.progressChart.Series["Weight"].Points.Clear();
            string con = MyMethodsLib.GetConnectionString();
            var dates =  MyMethodsLib.GetDatesOfInputWeight(Username, con);
            var weightInputs = MyMethodsLib.GetWeightInputs(Username, con);
            int track = 0;
            foreach (String date in dates)
            {
                this.progressChart.Series["Weight"].Points.AddXY(date, weightInputs[track]);
                track += 1;
            }
            this.progressChart.ChartAreas["ChartArea1"].AxisX.Title = "Date";
            this.progressChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight";
        }

        public void Button2_Click_1(object sender, EventArgs e)
        {
            //methods to refresh charts and data included in it
            this.slowModeChart.Series["Weight"].Points.Clear();
            this.steadyModeChart.Series["Weight"].Points.Clear();
            this.intenseModeChart.Series["Weight"].Points.Clear();
            SetupCharts4Projection(Username);
            string currentWeight = weightBox.Text;
            double.TryParse(currentWeight, out double currentWeightDouble);
            string daysToAchieveGoalsSlowMode = slowModeBox.Text;
            int.TryParse(daysToAchieveGoalsSlowMode, out int daysAllProcessSlowMode);

            int gapDivisor = daysAllProcessSlowMode / 6;
            int counter = gapDivisor;
            while (counter < daysAllProcessSlowMode - 1)
            {
                this.slowModeChart.Series["Weight"].Points.AddXY(counter, currentWeightDouble - 0.1 * counter);
                counter += gapDivisor;
            }
            this.slowModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.slowModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";

            //for steadyModeChart
            string daysToAchieveGoalSteadyMode = steadyModeBox.Text;
            int.TryParse(daysToAchieveGoalSteadyMode, out int daysAllProcessSteadyMode);

            int gapDivisorSteady = daysAllProcessSteadyMode / 6;
            int counterSteady = gapDivisorSteady;

            while (counterSteady < daysAllProcessSteadyMode - 1)
            {
                this.steadyModeChart.Series["Weight"].Points.AddXY(counterSteady, currentWeightDouble - 0.2 * counterSteady);
                counterSteady += gapDivisorSteady;
            }
            this.steadyModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.steadyModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";

            //for intenseModeChart
            string daysToAchieveGoalIntenseMode = intenseModeBox.Text;
            int.TryParse(daysToAchieveGoalIntenseMode, out int daysAllProcessIntenseMode);

            int gapDivisorIntense = daysAllProcessIntenseMode / 6;
            int counterIntense = gapDivisorIntense;

            while (counterIntense < daysAllProcessIntenseMode - 1)
            {
                this.intenseModeChart.Series["Weight"].Points.AddXY(counterIntense, currentWeightDouble - 0.3 * counterIntense);
                counterIntense += gapDivisorIntense;
            }
            this.intenseModeChart.ChartAreas["ChartArea1"].AxisX.Title = "Days";
            this.intenseModeChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight Drop";

        }

        public void printBtn_Click_1(object sender, EventArgs e)
        {
            string filecontent = "BREAKFAST\n" + breakfastRichTxtBx.Text + "\n==================================================================\n"
                + "\n\nBREAKFAST2\n" + break2RichTxtBx.Text.ToString() + "\n==================================================================\n" +
                "\n\nLUNCH\n" + lunchRichTxtBx.Text.ToString() + "\n==================================================================\n"
                + "\n\nDINNER \n" + dinnerRichTxtBx.Text.ToString();
            string tempPath = Environment.CurrentDirectory + "\\temp.txt";
            System.IO.File.WriteAllText(tempPath, filecontent);
            reader = new StreamReader(tempPath);
            fontUsed = new Font("Garamont", 12);
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(this.PrintTextFileHandler);
            pd.Print();
            if (reader != null)
            {
                reader.Close();
            }
            System.IO.File.Delete(tempPath);
        }

        private void slowModeBox_TextChanged(object sender, EventArgs e)
        {

        }
    }

}

