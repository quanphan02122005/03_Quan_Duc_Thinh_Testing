using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class UntiTestBai08
    {
        public static IEnumerable<object[]> LoadCsvData()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "DataBai08.csv");
            var lines = File.ReadAllLines(filePath, Encoding.UTF8).Skip(1);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',');
                string rawArray = parts[0].Trim();
                string rawLeft = parts[1].Trim();
                string rawRight = parts[2].Trim();
                string expected = parts[3].Trim();

                int[] list;
                if (rawArray.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                {
                    list = null;
                }
                else if (string.IsNullOrEmpty(rawArray))
                {
                    list = new int[0];
                }
                else
                {
                    list = rawArray.Split(';')
                                   .Select(x => Convert.ToInt32(x.Trim()))
                                   .ToArray();
                }

                int left = int.Parse(rawLeft);
                int right = int.Parse(rawRight);

                yield return new object[] { list, left, right, expected };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(LoadCsvData), DynamicDataSourceType.Method)]
        [DeploymentItem("Data\\DataBai08.csv", "Data")]
        public void ChayTestQuickSort(int[] list, int left, int right, string expected)
        {
            MethodLibrary.MethodLibrary o = new MethodLibrary.MethodLibrary();

            if (expected.StartsWith("Lỗi", StringComparison.OrdinalIgnoreCase))
            {
                bool threwException = false;
                try
                {
                    o.QuickSort(list, left, right);
                }
                catch (Exception)
                {
                    threwException = true; 
                }

                Assert.IsTrue(threwException, "Hàm phải quăng ngoại lệ khi dữ liệu đầu vào bị lỗi.");
            }
            else
            {
                o.QuickSort(list, left, right);

                int[] expectedArray = expected.Split(';')
                                              .Select(x => Convert.ToInt32(x.Trim().TrimEnd(']')))
                                              .ToArray();

                CollectionAssert.AreEqual(expectedArray, list);
            }
        }
    }
}
