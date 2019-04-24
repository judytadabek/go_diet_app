// change absolute for relative path
// how to close this window after 
//Screen is shown up?

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


namespace GoDiet
{
    public partial class InitialWindow : Form
    {
        public InitialWindow() => InitializeComponent();
        public static string SetUsername = "";
        public static string SetPassword = "";
        public void SignIn_Click(object sender, EventArgs e)
        {
 
                using (
                    SqlConnection connection = new SqlConnection(MyMethodsLib.GetConnectionString()))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select * from [tblUserNamePassw] where Username=@Username and Password=@Password";
                    cmd.Parameters.AddWithValue("@Username", UsernameLogin.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", PasswLogin.Text.Trim());
                    cmd.Connection = connection;
                    SqlDataReader dataReader = cmd.ExecuteReader();
                    if (!dataReader.HasRows)
                    {
                        MessageBox.Show("Username does not exist or password is incorrect.");
                        Clear();
                    }
                    else
                    {
                        SetUsername = UsernameLogin.Text;
                        this.DialogResult = DialogResult.OK;
                    }
                }
            
        }

        void Clear()
        {
            UsernameLogin.Text = PasswLogin.Text = "";
        }
        public void SignUpButton_Click(object sender, EventArgs e) => this.DialogResult = DialogResult.Yes;

        public void UsernameLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PasswLogin.Focus();
            }
        }

        public void PasswLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SignIn.PerformClick();
            }
        }

        public void UsernameLogin_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

