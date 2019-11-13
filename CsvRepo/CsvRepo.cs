using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public class Repo : IRepo
    {
        private readonly string _baseDirectory;
        private readonly IFileProvider _fileProvider;

        public Repo(string baseDirectory, IFileProvider fileProvider)
        {
            _baseDirectory = baseDirectory;
            _fileProvider = fileProvider;
        }

        public Repo(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            _fileProvider = new FileProvider();
        }


        public IEnumerable<TItems> GetAll<TItems>()
        {
            var path = GetFileName<TItems>();
            var instantiate = GetInstantiationFunc<TItems>();

            using (var file = _fileProvider.GetFile(path))
            {
                string line;
                file.ReadLine(); // advance reader past the header line
                while ((line = file.ReadLine()) != null)
                    yield return instantiate(line);

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

        public object GetInternal(TKey key)
        {
            throw new NotImplementedException();
        }



        private string GetFileName<TItem>()
            => Path.Combine(_baseDirectory, $"{typeof(TItem).Name}.csv");

        //ToDo: What if columns are not in order that properties are defined in the class?
        private Func<string, T> GetInstantiationFunc<T>()
        {

            var objType = typeof(T);

            var propertyInstantiators = objType.GetProperties().Select(p => p.GetType())
               .Select(propType => PropertyInstantiatorMapping.ContainsKey(propType) 
                ? PropertyInstantiatorMapping[propType] 
                : GetNavigationPropertyInstantiator(objType, propType));

            return line =>
            {
                var 

                Split(line)
                    .Zip(propertyInstantiators, (cell, instantiator) =>
            }
        }

        private Func<string,object> GetNavigationPropertyInstantiator(Type objectType, Type propertyType)
        {


            var foreignKeyProp = objectType.GetProperties()
                .SingleOrDefault(p => p.Name == GetForeignKeyFieldName(propertyType));

            return cell => foreignKeyProp != null 
                ? foreignKeyProp.GetValue()

            

        }

        private static string GetForeignKeyFieldName(Type type)
            => $"{type.Name}Id";




        private static readonly IDictionary<Type, Func<string, object>> PropertyInstantiatorMapping = new Dictionary<Type, Func<string, object>>
    {
        { typeof(int), cell => int.TryParse(cell, out int result) ? result : 0 }
    }
    

    private string[] Split(string line)
        => line.Trim('"').Replace("\",\"", "~").Split('~')
    }
}
