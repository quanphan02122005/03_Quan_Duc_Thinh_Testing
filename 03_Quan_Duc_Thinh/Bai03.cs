using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class Bai03
    {
        public TestContext TestContext { get; set; }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\DataBai03.csv", "DataBai03#csv",
            DataAccessMethod.Sequential)]
        [DeploymentItem("Data\\DataBai03.csv")]
        [TestMethod]
        public void ChayTestTienDien()
        {
            string rawChiSoCu = TestContext.DataRow[0].ToString().Trim();
            string rawChiSoMoi = TestContext.DataRow[1].ToString().Trim();
            string expectedResult = TestContext.DataRow[2].ToString().Trim();

            if (rawChiSoCu.Equals("chiSoCu", StringComparison.OrdinalIgnoreCase))
                return;

            int chiSoCu = Convert.ToInt32(rawChiSoCu);
            int chiSoMoi = Convert.ToInt32(rawChiSoMoi);
            double expected  = Convert.ToDouble(expectedResult);

            MethodLibrary.MethodLibrary o = new MethodLibrary.MethodLibrary();


            double actual = o.TinhTienDien(chiSoCu, chiSoMoi);

            TestContext.WriteLine($"ChiSoCu={chiSoCu}, ChiSoMoi={chiSoMoi} => TienDien that  = {actual}, TienDienDuDoan = {expected}");

            Assert.AreEqual(expected, actual, $"ChiSoCu={chiSoCu}, ChiSoMoi={chiSoMoi} => TienDien that  = {actual}, TienDienDuDoan = {expected}");
        }
    }

}
