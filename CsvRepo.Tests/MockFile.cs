using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo.Tests
{
    internal class MockFile : IFile
    {
        private IList<string> _lines { get; set; }
        private int currentLineNumber = 0;


        public MockFile(IList<string> lines)
        {
            _lines = lines;
        }

        public void AppendLine(string line)
        {
            _lines = _lines.Concat(new[] { line }).ToArray();
        }

        public void DeleteLine(int lineNumber)
            => _lines.RemoveAt(lineNumber);
        

        public void Dispose()
            => _lines = null;
        

        public string ReadLine()
            => currentLineNumber < _lines.Count()
                    ? _lines[currentLineNumber]
                    : null;
        
    }
}
