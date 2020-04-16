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
    public class UnitTests
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
            Verifications.VerifyEquals(new object[] { 1, "David", 25, 60.43m, new DateTime(1994, 02, 14) }, ppl[0]);
            Verifications.VerifyEquals(new object[] { 2, "Sam", 40, 65.94m, new DateTime(1990, 11, 29) }, ppl[1]);
            Verifications.VerifyEquals(new object[] { 3, "Julia", 32, 64.02m, new DateTime(1983, 8, 05) }, ppl[2]);
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
        public void GivenFileExists_WhenAdd_AppendCorrectLineToFile()
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

        [Test]
        public void GivenFileDoesNotExist_WhenAdd_CreateNewFileWithItem()
        {
            //Arrange
            var car = new Car
            {
                Make = "Toyota",
                Model = "Prius",
                Year = 2018
            };

            //Act
            _testRepo.Add(car);
            var lines = GetAllLines("baseDir\\Car.csv");

            //Assert
            Assert.AreEqual("\"Make\",\"Model\",\"Year\"", lines[0]);
            Assert.AreEqual("\"Toyota\",\"Prius\",\"2018\"", lines[1]);
            Assert.AreEqual(2, lines.Count());
        }

        [Test]
        public void GivenFileExists_WhenAddRange_AppendCorrectLinesToFile()
        {
            //Arrange
            SetupMockFile("baseDir\\Customer.csv", MockCsvs.MockCustomerLines);

            var customers = new[]
            {
                new Customer
                {
                    CustomerId = 3,
                    Name = "Gilbert"
                },
                new Customer
                {
                    CustomerId = 4,
                    Name = "Lexi"
                }
            };

            //Act
            _testRepo.AddRange(customers);
            var lines = GetAllLines("baseDir\\Customer.csv");

            //Assert
            Assert.AreEqual(MockCsvs.MockCustomerLines[0], lines[0]);
            Assert.AreEqual(MockCsvs.MockCustomerLines[1], lines[1]);
            Assert.AreEqual(MockCsvs.MockCustomerLines[2], lines[2]);
            Assert.AreEqual("\"3\",\"Gilbert\"", lines[3]);
            Assert.AreEqual("\"4\",\"Lexi\"", lines[4]);
        }

        [Test]
        public void GivenFileDoesNotExist_WhenAddRange_CreateNewFileWithItems()
        {
            //Arrange
            var cars = new[]
            {
                new Car
                {
                    Make = "Toyota",
                    Model = "Prius",
                    Year = 2018
                },
                new Car
                {
                    Make = "Subaru",
                    Model = "Outback",
                    Year = 2010
                }
            };
                
            //Act
            _testRepo.AddRange(cars);
            var lines = GetAllLines("baseDir\\Car.csv");

            //Assert
            Assert.AreEqual("\"Make\",\"Model\",\"Year\"", lines[0]);
            Assert.AreEqual("\"Toyota\",\"Prius\",\"2018\"", lines[1]);
            Assert.AreEqual("\"Subaru\",\"Outback\",\"2010\"", lines[2]);
            Assert.AreEqual(3, lines.Count());
        }

        [Test]
        public void GivenPrimaryKey_WhenDelete_RemoveAppropriateItemFromFile()
        {
            //Arrange
            SetupMockFile("baseDir\\Person.csv", MockCsvs.MockPeopleLines);

            //Act 
            _testRepo.Delete<Person>(1);

            var lines = GetAllLines("baseDir\\Person.csv");

            //Assert
            Assert.AreEqual("\"PersonId\",\"Name\",\"Age\",\"Height\",\"Birthday\"", lines[0]);
            //Assert.AreEqual("\"1\",\"David\",\"25\",\"60.43\",\"2/14/1994 12:00:00 AM\"", lines[1]);
            Assert.AreEqual("\"2\",\"Sam\",\"40\",\"65.94\",\"11/29/1990 12:00:00 AM\"", lines[1]);
            Assert.AreEqual("\"3\",\"Julia\",\"32\",\"64.02\",\"8/5/1983 12:00:00 AM\"", lines[2]);
            Assert.AreEqual(3, lines.Count());
        }

        [Test]
        public void GivenItemWithPrimaryKeyOfExistingItem_WhenUpdate_ChangeFieldValues()
        {
            //Arrange
            SetupMockFile("baseDir\\Person.csv", MockCsvs.MockPeopleLines);

            //Act
            _testRepo.Update(new Person
            {
                PersonId = 3,
                Name = "Juliette",
                Age = 33,
                Birthday = new DateTime(1983, 8, 5),
                Height = 64.02m
            });

            var lines = GetAllLines("baseDir\\Person.csv");

            //Assert
            Assert.AreEqual("\"PersonId\",\"Name\",\"Age\",\"Height\",\"Birthday\"", lines[0]);
            Assert.AreEqual("\"1\",\"David\",\"25\",\"60.43\",\"2/14/1994 12:00:00 AM\"", lines[1]);
            Assert.AreEqual("\"2\",\"Sam\",\"40\",\"65.94\",\"11/29/1990 12:00:00 AM\"", lines[2]);
            Assert.AreEqual("\"3\",\"Juliette\",\"33\",\"64.02\",\"8/5/1983 12:00:00 AM\"", lines[3]);
        }

        private void SetupMockFile(string path, string[] lines)
        {
            var file = _mockFileProvider.Create(path);

            foreach (var line in lines)
                file.AppendLine(line);        
              
        }

        private IList<string> GetAllLines(string filePath)
            => _mockFileProvider.GetFile(filePath)
                .GetLines()
                .ToList();


    }
}