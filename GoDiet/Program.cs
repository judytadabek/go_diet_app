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
                    //Application.Run(new InitialWindow());
                    if (initialWindow.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new WelcomeScreen());
                    }
                }
                else if (resultSetupWindow == DialogResult.No)
                {
                    Application.Run(initialWindow);
                }
            }
            else
            {
                //do nothing...
            }

            //Application.Run(new InitialWindow());
        }
    }
}

