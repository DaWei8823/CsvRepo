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
        private int readerAt = 0;
                
        public MockFile(IList<string> lines)
        {
            _lines = lines;
        }



        public void AppendLine(string line)
            =>  _lines.Add(line);
        

        public void DeleteLine(int lineNumber)
            => _lines.RemoveAt(lineNumber);


        public void Dispose() { }


        public string ReadLine()
        {
            var result = readerAt < _lines.Count()
                      ? _lines[readerAt]
                      : null;

            readerAt++;

            return result;
        }
        
    }
}
