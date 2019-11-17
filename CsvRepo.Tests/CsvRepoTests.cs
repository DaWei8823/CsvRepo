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
            SetupMockFile("baseDir/Person.csv", MockCsv.MockPeopleLines);

            //Act
            var ppl = _testRepo.Get<Person>().ToList();

            //Assert
            Assert.IsTrue(ppl[])
        }

        private void SetupMockFile(string file, string[] lines)
        {
            var mockFile = new Mock<IFile>();
            _mockFileProvider.Setup(fp => fp.GetFile(file)).Returns(mockFile.Object);
            var sequentialResult = mockFile.SetupSequence(f => f.ReadLine());

            foreach (var line in lines)
                sequentialResult = sequentialResult.Returns(line);
                
        }
    }
}
