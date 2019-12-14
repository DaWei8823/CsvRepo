using System.Collections.Generic;
using System.Linq;

namespace CsvRepo
{
    public class File : IFile
    {
        private readonly string _path;
        
        public File(string path)
        {
            _path = path;
        }

        public ICollection<string> GetLines()
            => System.IO.File.ReadAllLines(_path);

        public void AppendLine(string line)
            => AppendLines(new[] { line });
            
        public void AppendLines(IEnumerable<string> lines)
            => System.IO.File.AppendAllLines(_path, lines);

        public void DeleteLine(int lineNumber)
        {
            var lines = System.IO.File.ReadAllLines(_path).ToList();
            lines.RemoveAt(lineNumber);
            System.IO.File.WriteAllLines(_path, lines);
        }

    }
}
