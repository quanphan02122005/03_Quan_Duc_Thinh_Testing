using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class UnitTestBai05
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        [DeploymentItem(@"Data\DataBai05.csv")]
        public void Test_HuyChuoi_Auto()
        {
            string filePath = "DataBai05.csv";
            if (!File.Exists(filePath))
            {
                Assert.Fail("Không tìm thấy file " + filePath);
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            MethodLibrary.MethodLibrary lib = new MethodLibrary.MethodLibrary();

            if (File.Exists("KetQua_Bai10.csv"))
            {
                File.Delete("KetQua_Bai10.csv");
            }

            List<string> failedCases = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                if (parts.Length < 5) continue;

                string testCaseNo = parts[0].Trim();
                string s = parts[1].Trim();
                string strN = parts[2].Trim();
                string strP = parts[3].Trim();
                string expected = parts[4].Trim();

                string actualResultStr = "";
                bool isPass = false;

                if (!int.TryParse(strN, out int n) || !int.TryParse(strP, out int p))
                {
                    actualResultStr = "Lỗi dữ liệu đầu vào";
                }
                else
                {
                    try
                    {
                        actualResultStr = lib.HuyChuoi(s, n, p) ?? "";
                        isPass = string.Equals(expected, actualResultStr, StringComparison.Ordinal);
                    }
                    catch (Exception ex)
                    {
                        actualResultStr = ex.GetType().Name;
                    }
                }

                string status = isPass ? "PASS" : "FAIL";

                TestContext.WriteLine($"Test Case: {testCaseNo} | Expected: \"{expected}\" | Actual: \"{actualResultStr}\" | Status: {status}");
                File.AppendAllText("KetQua_Bai10.csv", $"{testCaseNo},{s},{strN},{strP},\"{expected}\",\"{actualResultStr}\",{status}{Environment.NewLine}", Encoding.UTF8);

                if (!isPass)
                {
                    failedCases.Add($"Test case {testCaseNo} (Expected: \"{expected}\", Actual: \"{actualResultStr}\")");
                }
            }

            if (failedCases.Count > 0)
            {
                Assert.Fail($"Có {failedCases.Count} test case bị FAIL:{Environment.NewLine}" + string.Join(Environment.NewLine, failedCases));
            }
        }
    }
}