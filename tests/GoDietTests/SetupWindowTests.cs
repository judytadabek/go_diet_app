using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoDiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace GoDiet.Tests
{
    [TestClass()]
    public class SetupWindowTests
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
            SetupWindow sw = new SetupWindow();
            string conRes = sw.GetConnectionString();
            Assert.AreEqual(connectionExp, conRes);
        }
    }
}