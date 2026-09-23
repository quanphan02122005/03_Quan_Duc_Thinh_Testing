using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class UnitTestBai04
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        [DeploymentItem(@"Data\DataBai04.csv")]
        public void Test_Sum_Auto()
        {
            string filePath = "DataBai04.csv";

            if (!File.Exists(filePath))
            {
                Assert.Fail("Không tìm thấy file " + filePath);
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            MethodLibrary.MethodLibrary lib = new MethodLibrary.MethodLibrary();

            if (File.Exists("KetQua_Bai9.csv"))
            {
                File.Delete("KetQua_Bai9.csv");
            }

            List<string> failedCases = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string cleanLine = line.Replace("\"", "");
                string[] parts = cleanLine.Split(',');
                if (parts.Length < 3) continue;

                string testCaseNo = parts[0].Trim();
                string strS0 = parts[1].Trim();

                string expected = string.Join(",", parts, 2, parts.Length - 2).Trim();

                string actualResultStr = "";

                if (!long.TryParse(strS0, out long s0))
                {
                    actualResultStr = "Lỗi: Giá trị s0 phải là số nguyên";
                }
                else
                {
                    try
                    {
                        long s;
                        long k = lib.Sum(s0, out s);
                        actualResultStr = $"k = {k}, s = {s}";
                    }
                    catch (Exception ex)
                    {
                        actualResultStr = ex.GetType().Name;
                    }
                }

                string normExpected = expected.Normalize(NormalizationForm.FormC).Replace(" ", "").Trim();
                string normActual = (actualResultStr ?? "").Normalize(NormalizationForm.FormC).Replace(" ", "").Trim();

                bool isPass = string.Equals(normExpected, normActual, StringComparison.OrdinalIgnoreCase);
                string status = isPass ? "PASS" : "FAIL";

                TestContext.WriteLine($"Test Case: {testCaseNo} | Expected: \"{expected}\" | Actual: \"{actualResultStr}\" | Status: {status}");
                File.AppendAllText("KetQua_Bai9.csv", $"{testCaseNo},{strS0},\"{expected}\",\"{actualResultStr}\",{status}{Environment.NewLine}", Encoding.UTF8);

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