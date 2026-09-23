using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _03_Quan_Duc_Thinh
{
    [TestClass]
    public class Bai07
    {
        public static IEnumerable<object[]> LoadCsvData()
        {
            string filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "DataBai07.csv"
            );

            var lines = File.ReadAllLines(filePath, Encoding.UTF8).Skip(1);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                string rawArray = parts[0].Trim();
                string expected = parts[1].Trim();

                int[] arr;

                if (rawArray.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                {
                    arr = null;
                }
                else if (string.IsNullOrEmpty(rawArray))
                {
                    arr = Array.Empty<int>();
                }
                else
                {
                    arr = rawArray
                        .Split(';')
                        .Select(x => Convert.ToInt32(x.Trim()))
                        .ToArray();
                }

                yield return new object[] { arr, expected };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(LoadCsvData), DynamicDataSourceType.Method)]
        [DeploymentItem("Data\\DataBai07.csv", "Data")]
        public void ChayTestLargest(int[] a, string expected)
        {
            MethodLibrary.MethodLibrary o = new MethodLibrary.MethodLibrary();

            if (a == null)
            {
                var ex = Assert.ThrowsException<ArgumentNullException>(
                    () => o.Largest(a)
                );

                StringAssert.Contains(
                    ex.Message,
                    "Mảng truyền vào không được null"
                );
            }
            else
            {
                int actual = o.Largest(a);

                Assert.AreEqual(
                    int.Parse(expected),
                    actual
                );
            }
        }
    }
}
