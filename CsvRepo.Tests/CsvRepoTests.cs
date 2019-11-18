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

        private readonly Mock<IFileProvider> _mockFileProvider = new Mock<IFileProvider>();

        private CsvRepo _testRepo;

        [SetUp]
        public void Setup()
        {
            _testRepo = new CsvRepo("baseDir", _mockFileProvider.Object);
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
        public void GivenClassParentNavigationProperties_WhenGetAll_SetParentValuesProperly()
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
            Assert.AreEqual("Chair", orders[1].Item.Name);
        }

        private void SetupMockFile(string file, string[] lines)
        {
            _mockFileProvider
                .Setup(fp => fp.Exists(file))
                .Returns(true);

            _mockFileProvider
                .Setup(fp => fp.GetFile(file))
                .Returns(SetupFileWithSequence(lines));          
              
        }

        private IFile SetupFileWithSequence(string[] lines)
        {
            var mockFile = new Mock<IFile>();
            var sequentialResult = mockFile.SetupSequence(f => f.ReadLine());

            foreach (var line in lines)
                sequentialResult = sequentialResult.Returns(line);

            return mockFile.Object;
        }
    }
}
