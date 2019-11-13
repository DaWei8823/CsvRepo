using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public class File : IFile
    {
        public IFileReader GetFileReader(string path)
        {
            throw new NotImplementedException();
        }
    }
}
