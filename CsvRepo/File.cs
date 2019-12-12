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
        private readonly string _path;


        public File(string path)
        {
            _path = path;
        }

        public string[] GetLines()
            => System.IO.File.ReadAllLines(_path);

        public void AppendLine(string line)
            => System.IO.File.AppendAllLines(_path, new string[] { line });
            
        


        public void DeleteLine(int lineNumber)
        {
            var lines = System.IO.File.ReadAllLines(_path).ToList();
            lines.RemoveAt(lineNumber);
            System.IO.File.WriteAllLines(_path, lines);
        }



    }
}
