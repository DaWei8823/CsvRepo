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
            => new File(path);

        private static StreamReader GetNullStreamReader()
            => new StreamReader(new MemoryStream());

        public bool Exists(string path)
            => IO.File.Exists(path);

        public IFile Create(string path)
        {
            IO.File.Create(path);
            return new File(path);
        }
        
    }
}
