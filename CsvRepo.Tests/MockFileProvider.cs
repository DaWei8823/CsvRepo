using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo.Tests
{
    internal class MockFileProvider : IFileProvider
    {

        private readonly IDictionary<string, IList<string>> _pathToFileLinesMapping;

        public MockFileProvider(IDictionary<string,IList<string>> pathToFileLinesMapping)
        {
            _pathToFileLinesMapping = pathToFileLinesMapping;
        }

        public IFile Create(string path)
        {
            _pathToFileLinesMapping.Add(path, new List<string>());
            return GetFile(path);
        }

        public bool Exists(string path)
            => _pathToFileLinesMapping.ContainsKey(path);
            
        

        public IFile GetFile(string path)
        {
            if (_pathToFileLinesMapping.ContainsKey(path))
                return new MockFile(_pathToFileLinesMapping[path]);

            throw new InvalidOperationException($"Cannot find file at path {path}");

        }

        
    }
}
