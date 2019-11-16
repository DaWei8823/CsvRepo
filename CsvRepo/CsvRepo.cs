using System;
using System.Collections;
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
                    yield return instantiate(Split(line));
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

        private object GetInternal(Type type, string key)
        {
            throw new NotImplementedException();
        }

        private object GetInternal(Type type, Func<object,bool> pred)
        {
            throw new NotImplementedException();
        }

        private string GetFileName<TItem>()
            => GetFileName(typeof(TItem));

        private string GetFileName(Type t)
            => Path.Combine(_baseDirectory, $"{t.Name}.csv");
        

        //ToDo: What if columns are not in order that properties are defined in the class?
        //TODO: What if primary key has different name
        private static Func<string[], T> GetInstantiationFunc<T>()
        {

            var objType = typeof(T);
                       
            var propertyInstantiators = objType.GetProperties()
                .Select(p => p.GetType())
                .Select((propType, index) => PropertyInstantiatorMapping.ContainsKey(propType) 
                    ? PropertyInstantiatorMapping[propType].Curry(index) 
                    : GetNavigationPropertyInstantiator(objType, propType));           

        }

        private Func<string[], object> GetNavigationPropertyInstantiator(Type objectType, Type propertyType)
        {
            if(typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType.IsGenericType)
            {
                var childType = propertyType.GetGenericArguments()[0];
                var primaryKeyFieldName = GetPrimaryKeyFieldName(objectType);

                var primaryKeyIndex = GetPropertyIndex(objectType, primaryKeyFieldName);

                if (primaryKeyIndex == null)
                    throw new ArgumentException($"Cannot find primary key {primaryKeyFieldName} in type {objectType.Name}");

                var foreignKeyInChildTypeIndex = GetPropertyIndex(childType, primaryKeyFieldName);

                if(foreignKeyInChildTypeIndex == null)
                    throw new ArgumentException($"Cannot find foreign key reference {primaryKeyFieldName} to {objectType.Name} in child type {childType.Name}");

                return cells =>
                {
                    Func<object, bool> predicate = obj => string.Equals(
                        obj.GetType().GetProperties()[foreignKeyInChildTypeIndex.Value].GetValue(obj).ToString(),
                        cells[primaryKeyIndex.Value]);

                    return GetInternal(childType, predicate);
                };
            }

            var foreignKeyIndex = GetPropertyIndex(objectType, GetPrimaryKeyFieldName(propertyType));              
            
            if (foreignKeyIndex != null)
                return MakeChildToParentInstantiationFunc(propertyType, foreignKeyIndex.Value);
                       

        }


        private Func<string[], object> MakeChildToParentInstantiationFunc(Type parentType, int foreignKeyIndex)
            => line => GetInternal(parentType, line[foreignKeyIndex]);

        private static int? GetPropertyIndex(Type type, string propertyName)
            => type.GetProperties()
                .Select((p, i) => new { property = p, index = i })
                .SingleOrDefault(a => a.property.Name == propertyName)
            ?.index;               

        private static string GetPrimaryKeyFieldName(Type type)
            => $"{type.Name}Id";




        private static readonly IDictionary<Type, Func<int, string[], object>> PropertyInstantiatorMapping
            = new Dictionary<Type, Func<int, string[], object>>
        {
            { typeof(int), (index, cells) => int.TryParse(cells[index], out int result) ? result : 0 }
        };
    

    private string[] Split(string line)
        => line.Trim('"').Replace("\",\"", "~").Split('~')
    }
}
