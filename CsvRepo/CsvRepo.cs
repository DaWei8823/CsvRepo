using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public class Repo : IRepo
    {
        private readonly string _baseDirectory;



        public IEnumerable<TItems> GetAll<TItems>()
        {
            var fileName = GetFileName<TItems>();

            using (var file = File.Exists(fileName) ? new StreamReader(fileName) : GetNullStreamReader())
            {
                string line;
                file.ReadLine(); // advance reader past the header line
                while ((line = file.ReadLine()) != null)
                    yield return Instantiate<TItems>(line);

            }
        }

        public TItem Get<TItem, TKey>(TKey key)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<TItem> Get<TItem>(Func<TItem, bool> pred)
        {
            throw new NotImplementedException();
        }

        public Repo(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public void Add<TItem>(TItem item)
        {
            throw new NotImplementedException();
        }

        public void Delete<TItem>(TItem item)
        {
            throw new NotImplementedException();
        }

        public void Delete<TItem, TKey>(TKey key)
        {
            throw new NotImplementedException();
        }

        public void Update<TItem>(TItem item)
        {
            throw new NotImplementedException();
        }



        private static StreamReader GetNullStreamReader()
            => new StreamReader(new MemoryStream());

        private string GetFileName<TItem>()
            => Path.Combine(_baseDirectory, $"{typeof(TItem).Name}.csv");

        private Func<string, T> GetInstantiator<T>(string header = null)
        {
            var propertyTypes = typeof(T).GetProperties()
                .Select(p => PropertyMapping[p.GetType()]);

            propertyTypes = header != null ?

            var values = Split(lines)
    


    }





        private static readonly IDictionary<Type, Func<string, object>> PropertyMapping = new Dictionary<Type, Func<string, object>>
    {
        { typeof(int), cell => int.TryParse(cell, out int result) ? result : 0 }
    }
    

    private string[] Split(string line)
        => line.Trim('"').Replace("\",\"", "~").Split('~')
    }
}
