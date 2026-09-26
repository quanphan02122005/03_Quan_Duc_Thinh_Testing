using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class UnitTestBai02
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem(@"Data\DataBai02.csv")]
        public void ChayTestGiaiPhuongTrinhBac2()
        {
            string filePath = "DataBai02.csv";
            if (!File.Exists(filePath))
            {
                // Thử tìm trong thư mục Data (khi chạy không deploy)
                if (File.Exists(@"Data\DataBai02.csv")) filePath = @"Data\DataBai02.csv";
                else Assert.Fail("Không tìm thấy file " + filePath);
            }

            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
            MethodLibrary.MethodLibrary o = new MethodLibrary.MethodLibrary();

            const string resultFile = "KetQua_Bai02.csv";
            if (File.Exists(resultFile)) File.Delete(resultFile);
            File.AppendAllText(resultFile, "a,b,c,Expected,Actual,Status" + Environment.NewLine, Encoding.UTF8);

            List<string> failedCases = new List<string>();
            int total = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("a,", StringComparison.OrdinalIgnoreCase)) continue;

                string[] head = SplitFirstN(line, ',', 3);
                if (head == null) continue;
                total++;

                string rawA = head[0].Trim(), rawB = head[1].Trim(), rawC = head[2].Trim();
                string expectedFull = head[3].Trim().Trim('"');

                int a, b, c;
                if (!int.TryParse(rawA, out a) || !int.TryParse(rawB, out b) || !int.TryParse(rawC, out c))
                {
                    failedCases.Add($"Dòng {i + 1}: dữ liệu a,b,c không phải số nguyên ({line})");
                    continue;
                }

                string expectedMsg = expectedFull.Split(',')[0].Trim();
                float expX1, expX2;
                bool hasX = TryParseExpectedX(expectedFull, out expX1, out expX2);

                float actX1, actX2;
                string actualMsg = o.SolveQuadratic(a, b, c, out actX1, out actX2);
                string actualFull = BuildActualFull(actualMsg, actX1, actX2, hasX);

                bool msgPass = IsMsgMatch(actualMsg, expectedMsg);
                bool xPass = IsXMatch(expX1, expX2, hasX, actX1, actX2);
                bool pass = msgPass && xPass;
                string status = pass ? "PASS" : "FAIL";

                TestContext.WriteLine(
                    $"a={a}, b={b}, c={c} => Expected: \"{expectedFull}\" | Actual: \"{actualFull}\" | Msg:{(msgPass ? "OK" : "SAI")} X:{(xPass ? "OK" : "SAI")} => {status}");
                File.AppendAllText(resultFile,
                    $"{a},{b},{c},\"{expectedFull}\",\"{actualFull}\",{status}{Environment.NewLine}", Encoding.UTF8);

                if (!pass)
                {
                    string reason = !msgPass
                        ? $"Message sai (Expected chứa \"{expectedMsg}\", Actual \"{actualMsg}\")"
                        : $"x1,x2 sai (Expected x1={Fmt(expX1)} x2={Fmt(expX2)}, Actual x1={Fmt(actX1)} x2={Fmt(actX2)})";
                    failedCases.Add($"a={a},b={b},c={c}: {reason}");
                }
            }

            TestContext.WriteLine($"Tổng: {total} case, PASS: {total - failedCases.Count}, FAIL: {failedCases.Count}");
            if (failedCases.Count > 0)
            {
                Assert.Fail($"Có {failedCases.Count}/{total} test case bị FAIL:{Environment.NewLine}"
                    + string.Join(Environment.NewLine, failedCases));
            }
        }

        private static string[] SplitFirstN(string line, char sep, int n)
        {
            List<int> pos = new List<int>();
            for (int k = 0; k < line.Length && pos.Count < n; k++)
                if (line[k] == sep) pos.Add(k);
            if (pos.Count < n) return null;
            string[] parts = new string[n + 1];
            int prev = 0;
            for (int k = 0; k < n; k++)
            {
                parts[k] = line.Substring(prev, pos[k] - prev);
                prev = pos[k] + 1;
            }
            parts[n] = line.Substring(prev);
            return parts;
        }

        private static string Norm(string s)
        {
            if (s == null) return "";
            string t = s.Normalize(NormalizationForm.FormC).Trim();
            return Regex.Replace(t, @"\s+", " ");
        }

        private static bool IsMsgMatch(string actual, string expected)
        {
            string a = Norm(actual);
            string e = Norm(expected);
            if (string.Equals(a, e, StringComparison.OrdinalIgnoreCase)) return true;
            if (a.EndsWith(e, StringComparison.OrdinalIgnoreCase)) return true;
            if (("Có " + e).Equals(a, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        private static bool TryParseExpectedX(string expectedFull, out float x1, out float x2)
        {
            x1 = float.NaN;
            x2 = float.NaN;
            if (string.IsNullOrWhiteSpace(expectedFull)) return false;

            Match m1 = Regex.Match(expectedFull, @"x1\s*=\s*x2\s*=\s*(\S+)", RegexOptions.IgnoreCase);
            if (m1.Success)
            {
                x1 = ParseFloat(m1.Groups[1].Value);
                x2 = x1;
                return true;
            }
            Match m2 = Regex.Match(expectedFull, @"x1\s*=\s*([-\d\.NaNn]+)\s*,\s*x2\s*=\s*([-\d\.NaNn]+)", RegexOptions.IgnoreCase);
            if (m2.Success)
            {
                x1 = ParseFloat(m2.Groups[1].Value);
                x2 = ParseFloat(m2.Groups[2].Value);
                return true;
            }
            return false; // VD "Vô số nghiệm": không có x1, x2
        }

        private static float ParseFloat(string s)
        {
            s = s.Trim().TrimEnd('.', ';');
            if (s.Equals("NaN", StringComparison.OrdinalIgnoreCase)) return float.NaN;
            float v;
            if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) return v;
            if (float.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out v)) return v;
            return float.NaN;
        }

        private static bool FloatEqual(float expected, float actual)
        {
            if (float.IsNaN(expected) && float.IsNaN(actual)) return true;
            if (float.IsNaN(expected) || float.IsNaN(actual)) return false;
            return Math.Abs(expected - actual) < 1e-3;
        }

        private static bool IsXMatch(float expX1, float expX2, bool hasX, float actX1, float actX2)
        {
            if (!hasX)
            {
                return float.IsNaN(actX1) && float.IsNaN(actX2);
            }
            if (FloatEqual(expX1, actX1) && FloatEqual(expX2, actX2)) return true;
            return FloatEqual(expX1, actX2) && FloatEqual(expX2, actX1);
        }

        private static string BuildActualFull(string msg, float x1, float x2, bool hasX)
        {
            if (!hasX) return string.Format("{0}, x1 = x2 = {1}", msg, Fmt(x1));
            if (FloatEqual(x1, x2)) return string.Format("{0}, x1 = x2 = {1}", msg, Fmt(x1));
            return string.Format("{0}, x1 = {1}, x2 = {2}", msg, Fmt(x1), Fmt(x2));
        }

        private static string Fmt(float v)
        {
            return float.IsNaN(v) ? "NaN" : v.ToString(CultureInfo.InvariantCulture);
        }
    }
}
