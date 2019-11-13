using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public interface IFile : IDisposable
    {
        IFileReader GetFileReader(string path);
    }
}
