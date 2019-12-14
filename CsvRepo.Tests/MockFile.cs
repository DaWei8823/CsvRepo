using System.Collections.Generic;

namespace CsvRepo.Tests
{
    internal class MockFile : IFile
    {
        private IList<string> _lines { get; set; }
        private int readerAt = 0;
                
        public MockFile(IList<string> lines)
        {
            _lines = lines;
        }
        
        public void AppendLine(string line)
            =>  _lines.Add(line);
        
        public void AppendLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
                _lines.Add(line);
        }

        public void DeleteLine(int lineNumber)
            => _lines.RemoveAt(lineNumber);

        public ICollection<string> GetLines()
            => _lines;        
    }
}
