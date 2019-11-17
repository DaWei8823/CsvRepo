using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo.Tests
{

    public static class Verifications
    {
        public static void VerifyEquals<T>(object[] values, T actual)
        {
            var discrepencies = values.Zip(
                typeof(T).GetProperties(),
                (exp, prop) => new
                {
                    exp,
                    actual = prop.GetValue(actual),
                    propName = prop.Name
                })
                .Where(a => !object.Equals(a.exp, a.actual))
                .Select(a => $"Discrepency in property {a.propName}. Expected: {a.exp}, Actual: {a.actual}");

            if (discrepencies.Any())
                throw new Exception(string.Join("\r\n", discrepencies));
                           
        }
    }

    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public decimal Height { get; set; }
        public DateTime Birthday { get; set; }
    }

    public static class MockCsvs
    {
        public static string[] MockPeopleLines = new string[]
        {
            "\"Name\",\"Age\",\"Height\",\"Birthday\"",
            "\"David\",\"25\",\"60.43\",\"2/14/1994 12:00:00 AM\"",
            "\"Sam\",\"40\",\"65.94\",\"11/29/1990 12:00:00 AM\"",
            "\"Julia\",\"32\",\"64.02\",\"8/5/1983 12:00:00 AM\""
        };

        public static string[] MockCustomerLines = new string[]
        {
            "\"CustomerId\",\"Name\"",
            "\"1\",\"Drake\"",
            "\"2\",\"Andrea\""
        };

        public static string[] MockItemLines = new string[]
        {
            "\"ItemId\",\"Name\"",
            "\"19\",\"Book\"",
            "\"43\",\"Chair\""
        };

        public static string[] MockOrderLines = new string[]
        {
            "\"OrderId\",\"ItemId\",\"CustomerId\"",
            "\"3\",\"43\",\"1\"",
            "\"4\",\"19\",\"2\"",
            "\"5\",\"43\",\"2\""
        };
    }
}
