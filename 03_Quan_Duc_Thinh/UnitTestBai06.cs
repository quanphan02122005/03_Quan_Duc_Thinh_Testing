using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class UnitTestBai06
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        private List<string> ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '\"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
                    {
                        current.Append('\"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(CleanValue(current.ToString()));
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            result.Add(CleanValue(current.ToString()));
            return result;
        }

        private string CleanValue(string val)
        {
            if (val == null) return "";
            string trimmed = val.Trim();
            if (trimmed == "\"\"" || trimmed == "\"")
            {
                return "";
            }
            return trimmed;
        }

        [TestMethod]
        [DeploymentItem(@"Data\DataBai06.csv")]
        public void Test_ThayThe_Auto()
        {
            string filePath = "DataBai06.csv";
            if (!File.Exists(filePath))
            {
                Assert.Fail("Không tìm thấy file " + filePath);
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            MethodLibrary.MethodLibrary lib = new MethodLibrary.MethodLibrary();

            string resultFilePath = "KetQua_Bai06.csv";
            if (File.Exists(resultFilePath))
            {
                File.Delete(resultFilePath);
            }

            List<string> failedCases = new List<string>();

            int startIndex = (lines.Length > 0 && lines[0].Trim().StartsWith("1")) ? 0 : 1;

            for (int i = startIndex; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                List<string> parts = ParseCsvLine(line);
                if (parts.Count < 6) continue;

                string testCaseNo = parts[0].Trim();
                string testType = parts[1].Trim();
                string s1 = parts[2];
                string s2 = parts[3];
                string s3 = parts[4];
                string expected = parts[5];

                string actualResultStr = "";
                bool isPass = false;

                try
                {
                    actualResultStr = lib.ThayThe(s1, s2, s3) ?? "";
                    isPass = string.Equals(expected, actualResultStr, StringComparison.Ordinal);
                }
                catch (Exception ex)
                {
                    actualResultStr = ex.GetType().Name;
                }

                string status = isPass ? "PASS" : "FAIL";

                TestContext.WriteLine($"Test Case: {testCaseNo} | s1: \"{s1}\" | s2: \"{s2}\" | s3: \"{s3}\" | Expected: \"{expected}\" | Actual: \"{actualResultStr}\" | Status: {status}");
                File.AppendAllText(resultFilePath, $"{testCaseNo},{testType},\"{s1}\",\"{s2}\",\"{s3}\",\"{expected}\",\"{actualResultStr}\",{status}{Environment.NewLine}", Encoding.UTF8);

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