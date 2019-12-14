using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public interface IFile 
    {
        ICollection<string> GetLines();
        void AppendLine(string line);
        void AppendLines(IEnumerable<string> lines);
        void DeleteLine(int lineNumber);
    }
}
