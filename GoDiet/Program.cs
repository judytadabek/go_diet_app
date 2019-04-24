using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GoDiet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //maybe while loop?

            var initialWindow = new InitialWindow();
            var resultInitialWindowShowDialog = initialWindow.ShowDialog();
            if (resultInitialWindowShowDialog == DialogResult.OK)
            {
                var welcome = new WelcomeScreen();
                Application.Run(welcome);
            }
            else if (resultInitialWindowShowDialog == DialogResult.Yes)
            {
                var setup = new SetupWindow();
                var resultSetupWindow = setup.ShowDialog();
                if (resultSetupWindow == DialogResult.OK)
                {
                    if (initialWindow.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new WelcomeScreen());
                    }
                }
                else if (resultSetupWindow == DialogResult.No)
                {
                    var initWind2 = new InitialWindow();
                    var res2 = initWind2.ShowDialog();
                    if (res2 == DialogResult.OK)
                    {
                        var welcome2 = new WelcomeScreen();
                        Application.Run(welcome2);
                    }
                    else
                    {
                        MessageBox.Show("Please, try again.");
                    }
                }
            }
            else
            {
                //do nothing...
            }

        }
    }
}

