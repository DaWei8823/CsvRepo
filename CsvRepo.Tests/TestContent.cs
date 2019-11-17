using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo.Tests
{
    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public decimal Height { get; set; }
        public DateTime Birthday { get; set; }
    }

    public static class MockCsv
    {
        public static string[] MockPeopleLines = new string[]
        {
            "\"Name\",\"Age\",\"Height\",\"Birthday\"",
            "\"David\",\"25\",\"60.43\",\"2/14/1994 12:00:00 AM\"",
            "\"Sam\",\"40\",\"65.94\",\"11/29/1990 12:00:00 AM\"",
            "\"Julia\",\"32\",\"64.02\",\"8/5/1983 12:00:00 AM\""
        };
    }
}
