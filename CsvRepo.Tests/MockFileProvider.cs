using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo.Tests
{
    internal class MockFileProvider : IFileProvider
    {

        private readonly IDictionary<string, IFile> _pathToFileMapping;

        public MockFileProvider(IDictionary<string,IFile> pathToFileMapping)
        {
            _pathToFileMapping = pathToFileMapping;
        }

        public IFile Create(string path)
        {
            var file = new MockFile(new List<string>());
            _pathToFileMapping.Add(path, file);
            return file;
        }

        public bool Exists(string path)
            => _pathToFileMapping.ContainsKey(path);
            
        

        public IFile GetFile(string path)
        {
            if (_pathToFileMapping.ContainsKey(path))
                return _pathToFileMapping[path];

            throw new InvalidOperationException($"Cannot find file at path {path}");

        }

        
    }
}
