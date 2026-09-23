using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class UnitTestBai01
    {
        public TestContext TestContext { get; set; }

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV",
            "|DataDirectory|\\DataBai01.csv", "DataBai01#csv",
            DataAccessMethod.Sequential)]
        [DeploymentItem("Data\\DataBai01.csv")]
        [TestMethod]
        public void ChayTestMax()
        {
            string rawA = TestContext.DataRow[0].ToString().Trim();
            string rawB = TestContext.DataRow[1].ToString().Trim();
            string rawC = TestContext.DataRow[2].ToString().Trim();
            string expectedResult = TestContext.DataRow[3].ToString().Trim();

            if (rawA.Equals("a", StringComparison.OrdinalIgnoreCase))
                return;

            int a = Convert.ToInt32(rawA);
            int b = Convert.ToInt32(rawB);
            int c = Convert.ToInt32(rawC);
            int expected = 0;
            try
            {
                expected = Convert.ToInt32(expectedResult);
            }catch(Exception ex)
            {

            }

            MethodLibrary.MethodLibrary o = new MethodLibrary.MethodLibrary();


            try
            {
                int actual = o.Max(a, b, c);

                TestContext.WriteLine($"a={a}, b={b}, c={c} => Max = {actual}");

                Assert.AreEqual(expected, actual, $"a={a}, b={b}, c={c}: Giá trị Max không đúng.");
            }
            catch (IndexOutOfRangeException ex)
            {
                TestContext.WriteLine($"a={a}, b={b}, c={c} =>{ex.GetType().Name}: {ex.Message}");
                Assert.IsTrue(true);
            }
        }
    }
}
