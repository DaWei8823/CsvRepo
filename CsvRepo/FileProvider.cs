using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CsvRepo
{
    public class FileProvider : IFileProvider
    {
        public IFile GetFile(string path)
            => new File(IO.File.Exists(path) ? new StreamReader(path) : GetNullStreamReader());


        private static StreamReader GetNullStreamReader()
            => new StreamReader(new MemoryStream());
    }
}
