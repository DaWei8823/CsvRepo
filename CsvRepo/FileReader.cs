using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public class File : IFile
    {
        private readonly StreamReader _streamReader;

        public File(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public void Dispose()
            => _streamReader.Dispose();

        public string ReadLine()
            => _streamReader.ReadLine();
    }
}
