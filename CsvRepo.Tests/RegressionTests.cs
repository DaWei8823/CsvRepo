using CsvRepo.Sample;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo.Tests
{
    [TestFixture]
    public class RegressionTests
    {

        private ICsvRepo _repo;

        [SetUp]
        public void Setup()
        {
            var baseDirectory = Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName,"TestCsvs");

            var bd = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CsvRepoTests");
            Directory.CreateDirectory(bd);
            _repo = new CsvRepo(bd);
        }

        [Test]
        public void Foo()
        {
            var customers = new Customer[]
            {
                new Customer { CustomerId = 1, Name = "David" },
                new Customer { CustomerId = 2, Name = "Danielle" },
                new Customer { CustomerId = 3, Name = "Julio" }
            };

            var items = new Item[]
            {
                new Item { ItemId = 32, Name = "Book" },
                new Item { ItemId = 45, Name = "Chair" },
                new Item { ItemId = 67, Name = "Chair" }
            };

            var orders = new Order[]
            {
                new Order { OrderId = 1, ItemId = 67, CustomerId = 3 },
                new Order { OrderId = 2, ItemId = 45, CustomerId = 2 },
                new Order { OrderId = 3, ItemId = 32, CustomerId = 1 },
            };

            _repo.AddRange(customers);
            _repo.AddRange(items);
            _repo.AddRange(orders);

            
        }
    }
}
