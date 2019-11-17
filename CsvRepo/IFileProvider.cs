using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public interface IFileProvider 
    {
        IFile GetFile(string path);
        bool Exists(string path);
        IFile Create(string path);
    }
}
