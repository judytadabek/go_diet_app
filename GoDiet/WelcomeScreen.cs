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
        // str to connect to dbs
        public string GetConnectionString()
        {
            string connection = "";
            string part = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename=";
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string connectionPath = getPath + "dbs\\GODIETCUSTINFO.MDF";
            string part2 = ";Integrated Security = True";
            connection = part + connectionPath + part2;
            return connection;

        }

        string userOutputToBox = "";

        void ClosePreviousWindow()
        {
            InitialWindow.ActiveForm.Close();

        }



        public bool CallLogisticRegression()
        {
            bool dietResult = false;

            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string _trainDataPath = getPath + "\\DietData\\DataModel\\trainData\\weight_lr_train1.csv";
            MLContext mlContext = new MLContext(seed: 0);
            var model = BinaryClassML.Train(mlContext, _trainDataPath);
            BinaryClassML.Evaluate(mlContext, model);
            string weight = weightBox.Text;
            string height = GetHeightFromDbs(Username).ToString();
            string bmi = CalculateBMI(weight, height);
            dietResult = BinaryClassML.Predict(mlContext, model, bmi);
            return dietResult;
        }

        public WelcomeScreen() => InitializeComponent();

        string Username = InitialWindow.SetUsername;

        //method to get diet mode:
        public string GetDietModeFromDbs(string Username)
        {
            string mode = "";
            string curDir = Directory.GetCurrentDirectory();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return mode;
        }

        //method to get veggie option from DBS
        public string GetVeggieOptionFromDbs(string Username)
        {
            string veggie = "";
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return veggie;
        }

        //method to get weight from dbs
        public string GetWeightFromDbs(string Username)
        {
            string weight = "";
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return weight;
        }

        // method to get gender from dbs
        public string GetGenderFromDbs(string Username)
        {
            string gender = "";
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return gender;
        }

        public string GetEmailFromDbs(string Username)
        {
            string email = "";
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return email;
        }

        // method to set DietMode in dbs
        private void SetDietModeDbs(string Username, string DietMode)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    dietModeBox.Text = DietMode;
                }

                catch (System.Exception)
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
        }

        // method to change user's name
        private void ChangeUserName(string UserName, string Name)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    nameBox.Text = "";
                }

                catch (System.Exception)
                {
                    MessageBox.Show("Name change sucks.");
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
        }

        private void ChangeUserSurName(string Username, string Surname)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    surnameBox.Text = "";
                }

                catch (System.Exception)
                {
                    MessageBox.Show("Surname change sucks.");
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
        }

        private void ChangeEmail(string Username, string Email)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    emailBx.Text = "";
                }

                catch (System.Exception)
                {
                    MessageBox.Show("Email change sucks.");
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

        }

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            unameBox.Text = Username;
            string height = GetHeightFromDbs(Username).ToString();
            weightBox.Text = GetWeightFromDbs(Username).ToString();
            BMIBox.Text = CalculateBMI(this.weightBox.Text.ToString(), height);
            double kgToLoose = KgToLoose(Username);
            kgBox.Text = WeightResultOutput(Username, kgToLoose);
            intakeBox.Text = CaloriesIntake(Username).ToString();
            dietModeBox.Text = GetDietModeFromDbs(Username);
            SetupCharts4Projection(Username);
        }

        private void RemoveAccountBtn_Click(object sender, EventArgs e)
        {
            int MeasurementNo = 0;
            DialogResult d_res = MessageBox.Show("Are you sure you want to remove your account with all your data?", "Confirm", MessageBoxButtons.YesNo);
            if (d_res == DialogResult.Yes)
            {
                //code for implementing data removal functionality
                using (
                SqlConnection connection = new SqlConnection(GetConnectionString()))
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

        private void BackBtn_Click(object sender, EventArgs e) => Close();
        private void TabPage1_Click(object sender, EventArgs e) { }
        private void TabPage2_Click(object sender, EventArgs e) { }
        private void Label1_Click(object sender, EventArgs e) { }
        private void Label3_Click(object sender, EventArgs e) { }
        private void RadioButton2_CheckedChanged(object sender, EventArgs e) { }
        private void TextBox1_TextChanged(object sender, EventArgs e) { }
        private void Label2_Click_1(object sender, EventArgs e) { }
        private void GenderBox_SelectedIndexChanged(object sender, EventArgs e) { }
        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e) { }
        private void TextBox9_TextChanged(object sender, EventArgs e) { }
        private void TextBox7_TextChanged(object sender, EventArgs e) { }
        private void UnameBox_TextChanged(object sender, EventArgs e) { }
        private void IntakeBox_TextChanged(object sender, EventArgs e) { }
        private void ClearBtn_Click(object sender, EventArgs e)
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
            if (yesRadioBtn.Checked)
            {
                Vegetarian = "Yes";
            }
            if (noRadioBtn.Checked)
            {
                Vegetarian = "No";
            }

            if (nameBox.Text != "")
            {
                ChangeUserName(Username, nameBox.Text);
            }

            if (surnameBox.Text != "")
            {
                ChangeUserSurName(Username, surnameBox.Text);
            }

            if (emailBx.Text != "")
            {
                ChangeEmail(Username, emailBx.Text);
            }

            using (SqlConnection sqlConnect = new SqlConnection(GetConnectionString()))
            {


                // code to update vegetarian option
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
                            CommandText = "SELECT * from [tblMeasures] WHERE UserName=@UserName AND Date=@Date"
                        };
                        cmdUpdateWeight.Parameters.AddWithValue("@Username", Username);
                        cmdUpdateWeight.Parameters.AddWithValue("@Weight", ModifWeightIn);
                        cmdUpdateWeight.Parameters.AddWithValue("@Date", Date);
                        cmdUpdateWeight.Connection = sqlConnect;
                        string dateFromDbs = "";

                        SqlCommand sqlCmdUpdateWeight = new SqlCommand();
                        try
                        {
                            sqlConnect.Open();
                            SqlDataReader dataReader = cmdUpdateWeight.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    dateFromDbs = dataReader.GetDateTime(4).Date.ToString();
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
                                        string height = GetHeightFromDbs(Username).ToString();
                                        BMIBox.Text = CalculateBMI(ModifWeightIn, height);

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
            double kgToLoose = KgToLoose(Username);
            kgBox.Text = WeightResultOutput(Username, kgToLoose);

            nameBox.Text = "";
            surnameBox.Text = "";
            oldPasswBox.Text = "";
            newPasswBox.Text = "";
            modifLastWeightIn.Text = "";

        }

        public int GetMeasurementNoWithTodayDate(string Username)
        {
            int MeasurementNo = 0;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    MessageBox.Show("Oh no, more work to do !");
                    connection.Close();
                }
            }
            return MeasurementNo;
        }
    
        public void Button1_Click(object sender, EventArgs e)
        {
            int MeasurementNo = GetMeasurementNoWithTodayDate(Username);

            var today2 = DateTime.Now.Date;
            var tomorrow2 = today2.AddDays(1);

            var dateForMealFromDbs2 = GetMealDate(MeasurementNo);
            if (tomorrow2 != dateForMealFromDbs2)
            {
                string mode = "";
                string kgToLoose = WeightResultOutput(Username, KgToLoose(Username));
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
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                            intakeBox.Text = CaloriesIntake(Username).ToString();
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

            var dateForMealFromDbs = GetMealDate(MeasurementNo);
            if (tomorrow == dateForMealFromDbs)
            {
                MessageBox.Show("You can't change diet mode today, try tomorrow!");
            }
            else
            {
                var randomRecipes = RandomizeRecipesSelection(CaloriesIntake(Username).ToString(), GetVeggieOptionFromDbs(Username), path, breakJsonName, break2JsonName, lunchJsonName, dinnerJsonName);
                RecordRecipesToDbs(randomRecipes, MeasurementNo);
                int breakID = GetBreakfastID(MeasurementNo);
                int break2ID = GetBreakfast2ID(MeasurementNo);
                int lunchID = GetLunchID(MeasurementNo);
                int dinnerID = GetLunchID(MeasurementNo);
                string break2RTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_breakfast2.rtf";
                string breakRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_breakfast.rtf";
                string lunchRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_lunch.rtf";
                string dinnerRTFName = Username + "_" + tomorrow.ToString("ddMMyyyy") + "_dinner.rtf";
                //breakfast
                SetUpRecipe(path, breakJsonName, breakID, tomorrow, breakRTFName);
                //breakfast2
                SetUpRecipe(path, break2JsonName, break2ID, tomorrow, break2RTFName);
                //lunch
                SetUpRecipe(path, lunchJsonName, lunchID, tomorrow, lunchRTFName);
                //dinner
                SetUpRecipe(path, dinnerJsonName, dinnerID, tomorrow, dinnerRTFName);
                //MessageBox.Show("Your Diet Mode is selected!");
            }
        }

        public DateTime GetMealDate(int MeasurementNo)
        {
            DateTime mealDate = new DateTime().Date;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return mealDate;
        }

        public int GetBreakfastID(int MeasurementNo)
        {
            int breakID = -1;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return breakID;
        }

        public int GetBreakfast2ID(int MeasurementNo)
        {
            int break2ID = -1;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return break2ID;

        }

        public int GetLunchID(int MeasurementNo)
        {
            int lunchID = -1;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return lunchID;
        }

        public int GetDinnerID(int MeasurementNo)
        {
            int dinnerID = -1;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return dinnerID;
        }

        public int GetCaloriesToConsume(int MeasurementNo)
        {
            int calories = -1;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }
            return calories;
        }

        public void Button2_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                                        string bmi = CalculateBMI(Weight.ToString(), Height.ToString());
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

            kgBox.Text = WeightResultOutput(Username, KgToLoose(Username));


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

        private void PredictedProgressWeightLossLbl_Click(object sender, EventArgs e) { }
        private void WeightBox_TextChanged(object sender, EventArgs e) { }
        private void CurrentLossChart_Click(object sender, EventArgs e) { }
        private void Button1_Click_1(object sender, EventArgs e) { }
        private void BMIBox_TextChanged(object sender, EventArgs e)
        {
            string height = GetHeightFromDbs(Username).ToString();
            this.BMIBox.Text = CalculateBMI(this.weightBox.Text.ToString(), height);
            this.WeightResultBox.Text = "Press";
            double.TryParse(this.BMIBox.Text, out double valToCompare);
            if (valToCompare < 24.9)
            {
                dietModeBox.Text = "Not set";
                SetDietModeDbs(Username, dietModeBox.Text);
                intakeBox.Text = "0";
            }
        }

        private void PredictedBtnRefresh_Click(object sender, EventArgs e) { }

        ///* this method is to be used within another method
        public string CalculateBMI(string weight, string height)
        {
            string bmi;
            float floatWeight = float.Parse(weight);
            float floatHeight = float.Parse(height);
            float floatBMI = floatWeight / (floatHeight / 100 * floatHeight / 100);
            bmi = floatBMI.ToString();
            return bmi;
        }

        private void TextBox1_TextChanged_1(object sender, EventArgs e) { }
        private void WeightResultBtn_Click(object sender, EventArgs e)
        {
            bool dietResult = CallLogisticRegression();
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

        private void PredictionWeightLoss_Click(object sender, EventArgs e) { }
        private void SurnameBox_TextChanged(object sender, EventArgs e) { }
        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void CurrentWeightLossLbl_Click(object sender, EventArgs e) { }
        private void RadioButton2_CheckedChanged_1(object sender, EventArgs e) { }
        private void RadioButton3_CheckedChanged(object sender, EventArgs e) { }
        private void Label7_Click(object sender, EventArgs e) { }
        private void OldPasswBox_TextChanged(object sender, EventArgs e) { }
        private void Label1_Click_1(object sender, EventArgs e) { }
        private void TextBox2_TextChanged(object sender, EventArgs e) { }

        //method to calculate the proper weight - 21.7 this is the mean of BMI indicator
        public double GetProperWeightCalculation(string Username)
        {
            double weightDesired = 0;
            double height = GetHeightFromDbs(Username);
            double bmiDesired = 21.7;
            //calculate desired weight
            weightDesired = (bmiDesired * (height / 100) * (height / 100));
            return weightDesired;
        }

        //method to get the height from dbs
        public int GetHeightFromDbs(string Username)
        {
            int heightInt = 0;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                try
                {
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
                        MessageBox.Show("Something went wrong");
                    }
                }
                catch (System.Exception)
                {
                    MessageBox.Show("Connection with database went wrong!");
                }

            }
            return heightInt;
        }

        //method to calculate the kg to loose
        public double KgToLoose(string Username)
        {
            Double.TryParse(weightBox.Text, out double weightCurrent);
            double weightDesired = GetProperWeightCalculation(Username);
            double weightToLoose = weightCurrent - weightDesired;
            return weightToLoose;
        }

        //method to output the result to the user

        public string WeightResultOutput(string Username, double kgToLoose)
        {
            double weightRes = KgToLoose(Username);
            //string userOutputToBox = "";
            if (weightRes <= 0)
            {
                this.userOutputToBox = "0";
            }
            else
            {
                this.userOutputToBox = weightRes.ToString("0.##");
            }
            return this.userOutputToBox;
        }

        private void Label8_Click(object sender, EventArgs e) { }

        private void Label2_Click(object sender, EventArgs e) { }


        //method to calculate number of days to achieve goal
        public string DaysToAchieveGoal(string kgToLoose, double ratio)
        {
            string daysNo;
            double.TryParse(kgToLoose, out double kg2L);
            double days2AchieveGoal = kg2L / ratio + 1;
            int days = (int)Math.Round(days2AchieveGoal);

            return days >= 0 ? (daysNo = days.ToString()) : "N/A";

        }

        //method to calculate weeks (7) or months (30)
        public string TimeToAchieveGoal(string days, int noOfDaysInWeekOrMonths)
        {
            string timeUnit;
            int.TryParse(days, out int daysInt);
            double timeDbl = daysInt / noOfDaysInWeekOrMonths;
            decimal timeDec = (decimal)Math.Round(timeDbl, 1);


            return timeUnit = timeDec.ToString();

        }

        private void RefreshGraphsBtn_Click(object sender, EventArgs e)
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

        private void SetupCharts4Projection(string Username)
        {
            double weightToLoose = KgToLoose(Username);
            string slowDays = DaysToAchieveGoal(weightToLoose.ToString(), 0.1);
            string steadyDays = DaysToAchieveGoal(weightToLoose.ToString(), 0.2);
            string intenseDays = DaysToAchieveGoal(weightToLoose.ToString(), 0.3);
            slowModeBox.Text = slowDays;
            slowWeeksBox.Text = TimeToAchieveGoal(slowDays, 7);
            slowMonthsBox.Text = TimeToAchieveGoal(slowDays, 30);

            steadyModeBox.Text = steadyDays;
            steadyWeeksBox.Text = TimeToAchieveGoal(steadyDays, 7);
            steadyMonthsBox.Text = TimeToAchieveGoal(steadyDays, 30);

            intenseModeBox.Text = intenseDays;
            intenseWeeksBox.Text = TimeToAchieveGoal(intenseDays, 7);
            intenseMonthsBox.Text = TimeToAchieveGoal(intenseDays, 30);
        }

        private void DayWeightBox_TextChanged(object sender, EventArgs e) { }

        private void Chart1_Click(object sender, EventArgs e) { }

        //number of calories to eat daily associated with the diet model chosen
        public int CaloriesIntake(string Username)
        {
            int calories = 0;
            string dietMode = GetDietModeFromDbs(Username);
            string gender = GetGenderFromDbs(Username);

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
                    MessageBox.Show("Something went wrong??");

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
                    MessageBox.Show("Something went wrong??");

                }
            }
            else
            {
                MessageBox.Show("Oppps... Something wrong.");
            }
            return calories;
        }

        //method to read recipes from the json file
        //to see if it works, output it to the console?
        public List<Item> LoadJson(string path, string jsonName)
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

        //method to randomize recipes within a scale
        public JsonTextReader GetJsonContent(string path, string jsonName)
        {
            string fullPath = Path.Combine(path, jsonName);
            JsonTextReader reader = new JsonTextReader(new StringReader(fullPath));
            return reader;
        }


        public List<int> RandomizeRecipesSelection(string caloriesIntake, string veggieOption, string path, string breakJsonName,
            string break2JsonName, string lunchJsonName, string dinnerJsonName)
        {
            var breakfastRecipesJsonContent = LoadJson(path, breakJsonName);
            var break2RecipesJsonContent = LoadJson(path, break2JsonName);
            var lunchRecipesJsonContent = LoadJson(path, lunchJsonName);
            var dinnerRecipesJsonContent = LoadJson(path, dinnerJsonName);

            // Dictionary<int, int> idAndCalories = new Dictionary<int, int>();

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
                        Dictionary<int, int> tempDict = new Dictionary<int, int>();
                        tempDict.Add(item.ID, item.CaloriesNo);
                        breakRecipes.Add(tempDict);
                    }
                }
                foreach (Item item in break2RecipesJsonContent)
                {
                    if (item.VeggieOption == "YES")
                    {
                        Dictionary<int, int> tempDict = new Dictionary<int, int>();
                        tempDict.Add(item.ID, item.CaloriesNo);
                        break2Recipes.Add(tempDict);
                    }
                }
                foreach (Item item in lunchRecipesJsonContent)
                {
                    if (item.VeggieOption == "YES")
                    {
                        Dictionary<int, int> tempDict = new Dictionary<int, int>();
                        tempDict.Add(item.ID, item.CaloriesNo);
                        lunchRecipes.Add(tempDict);
                    }
                }
                foreach (Item item in dinnerRecipesJsonContent)
                {
                    if (item.VeggieOption == "YES")
                    {
                        Dictionary<int, int> tempDict = new Dictionary<int, int>();
                        tempDict.Add(item.ID, item.CaloriesNo);
                        dinnerRecipes.Add(tempDict);
                    }
                }
            }
            else
            {
                foreach (Item item in breakfastRecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>();
                    tempDict.Add(item.ID, item.CaloriesNo);
                    breakRecipes.Add(tempDict);
                }
                foreach (Item item in break2RecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>();
                    tempDict.Add(item.ID, item.CaloriesNo);
                    break2Recipes.Add(tempDict);
                }
                foreach (Item item in lunchRecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>();
                    tempDict.Add(item.ID, item.CaloriesNo);
                    lunchRecipes.Add(tempDict);
                }
                foreach (Item item in dinnerRecipesJsonContent)
                {
                    Dictionary<int, int> tempDict = new Dictionary<int, int>();
                    tempDict.Add(item.ID, item.CaloriesNo);
                    dinnerRecipes.Add(tempDict);
                }
            }

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

        //now, add these recipes to the dbs.
        //after you will add them, you can attempt to outputting them to the rich text box :)
        public void RecordRecipesToDbs(List<int> recipesSetup, int MeasurementNo)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                //connection.Open();
                SqlCommand cmdCheckRowsNo = new SqlCommand
                {
                    CommandText = "SELECT * FROM [tblDailyMealSet]",
                    //Connection = connection
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
                try
                {
                    SqlCommand sqlInsertDailyMealSet = new SqlCommand();
                    sqlInsertDailyMealSet.CommandText = "INSERT INTO [tblDailyMealSet] (MealSetId, MeasurementNo, BreakfastID, Breakfast2ID, LunchID, DinnerID, TotalCaloriesNo, Date4Meal) VALUES (@MealSetId, @MeasurementNo, @BreakfastID, @Breakfast2ID, @LunchID, @DinnerID, @TotalCaloriesNo, @Date4Meal)";
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

                }
                catch (System.Exception)
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }

        }

        public void SetUpRecipe(string path, string jsonName, int ID, DateTime recipeDate, string RTFFileName)
        {
            string name = "";
            string caloriesNo = "";
            string description = "";
            string ingredients = "";
            string grams = "";
            string proteins = "";
            string carbons = "";
            string fats = "";
            var jsonContent = LoadJson(path, jsonName);
            foreach (Item item in jsonContent)
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


        }

        private void LoadRecipesBtn_Click(object sender, EventArgs e)
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

        private Font fontUsed;
        private StreamReader reader;

        private void PrintTextFileHandler(object sender, PrintPageEventArgs ppeArgs)
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

        private void printBtn_Click(object sender, EventArgs e)
        {
            //Font fontUsed;
            //StreamReader reader;
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

        //how to construct for email
        public bool SendMail(string MessageBody)
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
                string email = GetEmailFromDbs(Username);
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

        private void emailBtn_Click(object sender, EventArgs e)
        {
            string MessageBody = "BREAKFAST\n" + breakfastRichTxtBx.Text.ToString() + "\n\nBREAKFAST 2\n" + break2RichTxtBx.Text.ToString() + "\n\nLUNCH\n" + lunchRichTxtBx.Text.ToString() +
                    "\n\nDINNER\n" + dinnerRichTxtBx.Text.ToString();
            bool sentstatus = SendMail(MessageBody);
            if (sentstatus == true)
            {
                MessageBox.Show("Email was sent!");
            }
            else
            {
                MessageBox.Show("Sending e-mail failed. Update Your Email and try again!");
            }
        }

        public List<String> GetDatesOfInputWeight(string Username)
        {
            List<String> dates = new List<String>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return dates;
        }

        public List<int> GetWeightInputs(string Username)
        {
            List<int> weightInputs = new List<int>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

            }
            return weightInputs;

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            //it must be taken from dbs the dates and weights in order to output
            var dates = GetDatesOfInputWeight(Username);
            var weightInputs = GetWeightInputs(Username);
            int track = 0;
            foreach (String date in dates)
            {
                this.progressChart.Series["Weight"].Points.AddXY(date, weightInputs[track]);
                track += 1;
            }
            this.progressChart.ChartAreas["ChartArea1"].AxisX.Title = "Date";
            this.progressChart.ChartAreas["ChartArea1"].AxisY.Title = "Your Weight";
        }

        private void PrimaryInformation_SelectedIndexChanged(object sender, EventArgs e) { }
        private void intenseModeBox_TextChanged(object sender, EventArgs e) { }

        private void intenseModeTab_Click(object sender, EventArgs e) { }
        private void intenseWeeksBox_TextChanged(object sender, EventArgs e) { }
        private void intenseMonthsBox_TextChanged(object sender, EventArgs e) { }

        private void button2_Click_1(object sender, EventArgs e)
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

        private void breakfastRichTxtBx_TextChanged(object sender, EventArgs e) { }
    }

}

