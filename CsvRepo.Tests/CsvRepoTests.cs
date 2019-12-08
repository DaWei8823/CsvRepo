using CsvRepo.Sample;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvRepo = CsvRepo.CsvRepo;

namespace CsvRepo.Tests
{
    [TestFixture]
    public class CsvRepoTests
    {

        private IFileProvider _mockFileProvider;

        private CsvRepo _testRepo;

        [SetUp]
        public void Setup()
        {

            _mockFileProvider = new MockFileProvider(new Dictionary<string, IList<string>>());
            _testRepo = new CsvRepo("baseDir", _mockFileProvider);
        }


        [Test]
        public void GivenClassWithoutNavigationProperties_WhenGetAll_ReadsExceptedResults()
        {
            //Arrange
            SetupMockFile("baseDir\\Person.csv", MockCsvs.MockPeopleLines);

            //Act
            var ppl = _testRepo.Get<Person>().ToList();

            //Assert
            Verifications.VerifyEquals(new object[] { "David", 25, 60.43m, new DateTime(1994, 02, 14) }, ppl[0]);
            Verifications.VerifyEquals(new object[] { "Sam", 40, 65.94m, new DateTime(1990, 11, 29) }, ppl[1]);
            Verifications.VerifyEquals(new object[] { "Julia", 32, 64.02m, new DateTime(1983, 8, 05) }, ppl[2]);
        }

        [Test]
        public void GivenClassWithParentNavigationProperties_WhenGetAll_SetParentValuesProperly()
        {
            //Arrange
            SetupMockFile("baseDir\\Order.csv", MockCsvs.MockOrderLines);
            SetupMockFile("baseDir\\Customer.csv", MockCsvs.MockCustomerLines);
            SetupMockFile("baseDir\\Item.csv", MockCsvs.MockItemLines);


            //Act
            var orders = _testRepo.Get<Order>().ToList();

            //Assert
            Assert.AreEqual("Drake", orders[0].Customer.Name);
            Assert.AreEqual("Chair", orders[0].Item.Name);
            Assert.AreEqual("Andrea", orders[1].Customer.Name);
            Assert.AreEqual("Book", orders[1].Item.Name);
            Assert.AreEqual("Andrea", orders[1].Customer.Name);
            Assert.AreEqual("Book", orders[1].Item.Name);
            //TODO: Test recursive reference with navigation properties
        }


        [Test]
        public void GivenKey_WhenGet_ReturnCorrectObject()
        {
            //Arrange
            SetupMockFile("baseDir\\Item.csv", MockCsvs.MockItemLines);

            //Act
            var item = _testRepo.Get<Item>(43);

            //Assert
            Assert.AreEqual(43, item.ItemId);
            Assert.AreEqual("Chair", item.Name);
        }
        

        [Test]
        public void GivenFileExists_WhenAdd_AppendCorrectLinesToFile()
        {
            //Arrange
            SetupMockFile("baseDir\\Customer.csv", MockCsvs.MockCustomerLines);

            var customer = new Customer
            {
                CustomerId = 3,
                Name = "Gilbert"
            };

            //Act
            _testRepo.Add(customer);
            var lines = GetAllLines("baseDir\\Customer.csv");

            //Assert
            Assert.AreEqual(MockCsvs.MockCustomerLines[0], lines[0]);
            Assert.AreEqual(MockCsvs.MockCustomerLines[1], lines[1]);
            Assert.AreEqual(MockCsvs.MockCustomerLines[2], lines[2]);
            Assert.AreEqual("\"3\",\"Gilbert\"", lines[3]);
        }

        private void SetupMockFile(string path, string[] lines)
        {
            var file = _mockFileProvider.Create(path);

            foreach (var line in lines)
                file.AppendLine(line);        
              
        }

        private IList<string> GetAllLines(string filePath)
        {
            var list = new List<string>();

            using (var file = _mockFileProvider.GetFile(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                    list.Add(line);
            }

            return list;
        }

    }
}