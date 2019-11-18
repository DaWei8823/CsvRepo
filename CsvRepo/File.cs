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
        private readonly StreamWriter _streamWriter;
        private readonly string _path;


        public File(string path)
        {
            _path = path;
            _streamReader = new StreamReader(path);
            _streamWriter = new StreamWriter(path);
        }

        public string ReadLine()
            => _streamReader.ReadLine();

        public void AppendLine(string line)
            => _streamWriter.WriteLine(line);

        public void DeleteLine(int lineNumber)
        {
            var lines = System.IO.File.ReadAllLines(_path).ToList();
            lines.RemoveAt(lineNumber);
            System.IO.File.WriteAllLines(_path, lines);
        }

        public void Dispose()
            => _streamReader.Dispose();


    }
}
