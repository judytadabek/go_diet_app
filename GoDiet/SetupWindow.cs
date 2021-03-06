﻿// need to improve GUI
// change images
// connectionString is hardcoded! Change it.
// refactor code - SQL especially divide into smaller classes, DRY
// User Agreement Contract to be produced and attached


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
using System.IO;
using System.Diagnostics;

namespace GoDiet

{
    public partial class SetupWindow : Form
    {
        public SetupWindow()
        {
            InitializeComponent();
        }
        public void SetupWindow_Load(object sender, EventArgs e) { }
        public void SignUpButton_Click(object sender, EventArgs e)
        {
            if (Username.Text == "" || Password.Text == "" || passwConf.Text == "" || Height2.Text == "" || Weight.Text == "")
            {
                MessageBox.Show("Please fill mandatory fields");

            }
            else if (Password.Text != passwConf.Text)
            {
                MessageBox.Show("Passwords are not identical");
            }
            else
            {
                using (SqlConnection sqlConnect = new SqlConnection(MyMethodsLib.GetConnectionString()))
                {
                    sqlConnect.Open();
                    SqlCommand sqlCmdAddUserNamePasswGennderIfVeggie = new SqlCommand("UserAdd", sqlConnect);
                    SqlCommand sqlCmdAddMeasures = new SqlCommand("MeasureAdd", sqlConnect);
                    SqlCommand sqlCmdAddOtherInfo = new SqlCommand("OtherInfoAdd", sqlConnect);

                    sqlCmdAddMeasures.CommandType = CommandType.StoredProcedure;
                    sqlCmdAddUserNamePasswGennderIfVeggie.CommandType = CommandType.StoredProcedure;
                    sqlCmdAddOtherInfo.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdAddUser = new SqlCommand
                    {
                        CommandText = "select * from [tblUserNamePassw] where Username=@Username"
                    };
                    cmdAddUser.Parameters.AddWithValue("@Username", Username.Text.Trim());
                    cmdAddUser.Connection = sqlConnect;
                    SqlDataReader dataReader = cmdAddUser.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        MessageBox.Show("Username is already taken, try different one.");

                    }
                    else
                    {
                        int countUsernameLetters = Username.Text.Trim().Count();
                        if (countUsernameLetters > 2)
                        {
                            sqlCmdAddUserNamePasswGennderIfVeggie.Parameters.AddWithValue("@Username", Username.Text.Trim());
                        }
                        else
                        {
                            MessageBox.Show("Username is too short.");
                        }
                    }
                    dataReader.Close();
                    int countPasswChars = Password.Text.Trim().Count();
                    if (countPasswChars > 6)
                    {
                        sqlCmdAddUserNamePasswGennderIfVeggie.Parameters.AddWithValue("@Password", Password.Text.Trim());
                    }
                    else
                    {
                        MessageBox.Show("Password is too short, should contain at least 7 digits/letters.");
                    }

                    bool isChecked = true;
                    if (radioBtnYes.Checked == isChecked || radioBtnNo.Checked == isChecked)
                    {
                        if (radioBtnYes.Checked == isChecked)
                        {
                            sqlCmdAddUserNamePasswGennderIfVeggie.Parameters.AddWithValue("@Vegetarian", "Yes");
                        }
                        else
                        {
                            sqlCmdAddUserNamePasswGennderIfVeggie.Parameters.AddWithValue("@Vegetarian", "No");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please, select if you are vegetarian or not.");
                    }


                    if (GenderBox.SelectedItem.ToString() != "")
                    {
                        sqlCmdAddUserNamePasswGennderIfVeggie.Parameters.AddWithValue("@Gender", GenderBox.SelectedItem.ToString());
                    }
                    else
                    {
                        MessageBox.Show("Please, select your gender.");

                    }
                    SqlCommand cmdAddMeasures = new SqlCommand
                    {
                        CommandText = "select Username from [tblMeasures]"
                    };
                    cmdAddMeasures.Parameters.AddWithValue("@Username", Username.Text.Trim());
                    cmdAddMeasures.Connection = sqlConnect;
                    SqlDataReader dataReader2 = cmdAddMeasures.ExecuteReader();
                    int count = 0;
                    if (dataReader2.HasRows)
                    {
                        while (dataReader2.Read())
                        {
                            count++;
                        }
                    }
                    sqlCmdAddMeasures.Parameters.AddWithValue("@MeasurementNo", count);
                    sqlCmdAddMeasures.Parameters.AddWithValue("@Username", Username.Text.Trim());
                    bool correctHeightValue = int.TryParse(Height2.Text.Trim(), out int heightNumber);

                    if (correctHeightValue)
                    {
                        if (heightNumber < 50 || heightNumber > 230)
                        {
                            MessageBox.Show("Are you sure you entered your height properly?");
                        }
                        else
                        {
                            sqlCmdAddMeasures.Parameters.AddWithValue("@Height", Convert.ToInt32(Height2.Text.Trim()));
                            //setHeight = heightNumber;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please put only digits as height in cm");
                    }

                    bool correctWeightValue = decimal.TryParse(Weight.Text.Trim(), out decimal weightNumber);
                    if (correctWeightValue)
                    {
                        if (weightNumber < 30 || weightNumber > 200)
                        {
                            MessageBox.Show("Are you sure you entered your weight correctly?");
                        }
                        else
                        {
                            sqlCmdAddMeasures.Parameters.AddWithValue("@Weight", decimal.Parse(Weight.Text.Trim()));
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please put only digits as weight in kg");
                    }
                    dataReader2.Close();

                    SqlCommand cmdAddEmail = new SqlCommand
                    {
                        CommandText = "select Username from [tblMeasures]"
                    };
                    cmdAddEmail.Parameters.AddWithValue("@Username", Username.Text.Trim());
                    cmdAddEmail.Connection = sqlConnect;
                    SqlDataReader dataReader3 = cmdAddEmail.ExecuteReader();
                    int countAgain = 0;
                    if (dataReader3.HasRows)
                    {
                        while (dataReader3.Read())
                        {
                            countAgain++;
                        }
                    }

                    dataReader3.Close();
                    sqlCmdAddOtherInfo.Parameters.AddWithValue("@AddInfoID", countAgain);

                    if (Email.Text.Trim().Contains("@") || Email.Text.Trim().Count() == 0)
                    {
                        sqlCmdAddOtherInfo.Parameters.AddWithValue("@UserName", Username.Text.Trim());
                        sqlCmdAddOtherInfo.Parameters.AddWithValue("@Email", Email.Text.Trim());
                    }
                    else
                    {
                        MessageBox.Show("Given email is invalid");
                    }
                    sqlCmdAddUserNamePasswGennderIfVeggie.ExecuteNonQuery();
                    sqlCmdAddMeasures.ExecuteNonQuery();
                    sqlCmdAddOtherInfo.ExecuteNonQuery();
                    MessageBox.Show("Signing Up succeeded!" + "\n" + "Your login is: " + Username.Text.Trim());
                    //EnterDateAndUserWeightCsv(path);
                    this.DialogResult = DialogResult.OK;
                    Close();
                    Clear();
                }

            }
        }
        void Clear()
        {
            Password.Text = passwConf.Text = "";
        }

        public void BackBtn_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.DialogResult = DialogResult.No;
        }
        public void CheckBtn_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(MyMethodsLib.GetConnectionString()))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand
                {
                    CommandText = "select * from [tblUserNamePassw] where Username=@Username"
                };
                cmd.Parameters.AddWithValue("@Username", Username.Text.Trim());
                cmd.Connection = connection;
                SqlDataReader dataReader = cmd.ExecuteReader();
                if (!dataReader.HasRows)
                {
                    checkInfo.Visible = true;
                    checkInfo.Text = "Username available";
                    checkInfo.ForeColor = System.Drawing.Color.Green;
                    dataReader.Close();
                }
                else
                {
                    checkInfo.Visible = true;
                    checkInfo.Text = "Username taken";
                    checkInfo.ForeColor = System.Drawing.Color.Red;
                }
            }

        }

        void ChangeFocus(TextBox name, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                name.Focus();
            }
        }

        public void Username_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeFocus(Password, e);
        }

        public void Password_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeFocus(passwConf, e);
        }

        public void Surname_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeFocus(Height2, e);
        }

        public void Weight_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeFocus(Email, e);
        }

        public void Height2_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeFocus(Weight, e);
        }

        public void Email_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SubmitBtn.PerformClick();
            }
        }

        private void AgreementLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string getPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"));
            string path = getPath + "DietData";
            string pathToFile = path + "\\End_User_Agreement.pdf";
            Process.Start(pathToFile);

        }
    }
}

