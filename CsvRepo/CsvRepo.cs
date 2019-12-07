using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvRepo
{

    //BIG TODO: Seperate reflection logic from other file reading logic 
    public class CsvRepo : ICsvRepo
    {
        private readonly string _baseDirectory;
        private readonly IFileProvider _fileProvider;

        public CsvRepo(string baseDirectory, IFileProvider fileProvider)
        {
            _baseDirectory = baseDirectory;
            _fileProvider = fileProvider;
        }

        public CsvRepo(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
            _fileProvider = new FileProvider();
        }

        public ICollection<TItem> Get<TItem>() where TItem : class
        {
            var path = GetFilePath(typeof(TItem));

            if (!_fileProvider.Exists(path))
                return Enumerable.Empty<TItem>().ToList();

            var items = new List<TItem>();

            var instantiator = GetInstantiationFunc(typeof(TItem));

            using (var file = _fileProvider.GetFile(path))
            {
                string line;
                file.ReadLine(); // advance reader past the header line
                while ((line = file.ReadLine()) != null)
                    items.Add(instantiator(Split(line)) as TItem);
            }

            return items;
        }
        

        public TItem Get<TItem, TKey>(TKey key) where TItem : class
            => GetInternal(typeof(TItem), key.ToString()) as TItem;        

        public void Add<TItem>(TItem item)
        {
            var itemType = typeof(TItem);
            var path = GetFilePath(itemType);

            if (_fileProvider.Exists(path))
                using (var file = _fileProvider.GetFile(path))
                    file.AppendLine(GetCsvLine(item));
            
            //ToDo: primary key constraint!
            using (var file = _fileProvider.Create(path))
            {
                file.AppendLine(GetHeader(itemType));
                file.AppendLine(GetCsvLine(item));
            }   
        }

        public void AddRange<TItem>(IEnumerable<TItem> items)
        {
            var itemType = typeof(TItem);
            var path = GetFilePath(itemType);
            
            if (_fileProvider.Exists(path))
                using (var file = _fileProvider.GetFile(path))
                    items.Select(i => GetCsvLine(i)).ToList().ForEach(file.AppendLine);
            
            using (var file = _fileProvider.Create(path))
            {
                file.AppendLine(GetHeader(itemType));
                items.Select(i => GetCsvLine(i)).ToList().ForEach(file.AppendLine);
            }
        }

        public void Delete<TItem, TKey>(TKey key)
            => Delete<TItem>(key);

        public void Update<TItem>(TItem item)
        {
            var primaryKey = GetPrimaryKeyValueOfItem(item);
            Delete<TItem>(item);
            Add(item);
        }


        private object GetInternal(Type itemType, string key)
        {

            var path = GetFilePath(itemType);

            if (!_fileProvider.Exists(path))
                throw new ArgumentException($"Cannot find file {path}");

            var instantiator = GetInstantiationFunc(itemType);

            var primaryKeyFieldName = GetPrimaryKeyFieldName(itemType);
            var primaryKeyIndex = GetPropertyIndex(itemType, primaryKeyFieldName);

            if (!primaryKeyIndex.HasValue)
                throw new ArgumentException($"Excepted primary key {primaryKeyFieldName} missing on  type {itemType.Name}");

            using (var file = _fileProvider.GetFile(path))
            {
                string line;
                file.ReadLine(); // advance reader past the header line
                while ((line = file.ReadLine()) != null) 
                {
                    var cells = Split(line);
                    if (string.Equals(cells[primaryKeyIndex.Value], key.ToString()))
                        return instantiator(cells);
                }
            }

            throw new InvalidOperationException($"Could not find {itemType.Name}  value of {key.ToString()} for primary key {primaryKeyFieldName}");
        }


        private void Delete<TItem>(object key)
        {
            var itemType = typeof(TItem);
            var primaryKeyFieldName = GetPrimaryKeyFieldName(typeof(TItem));
            var primaryKeyIndex = GetPropertyIndex(itemType, primaryKeyFieldName);

            if (!primaryKeyIndex.HasValue)
                throw new ArgumentException($"Excepted primary key {primaryKeyFieldName} missing on  type {itemType.Name}");

            var path = GetFilePath(itemType);

            using (var file = _fileProvider.GetFile(path))
            {
                var index = 0;
                var lookupString = key.ToString();
                string line;

                while ((line = file.ReadLine()) != null)
                    if (string.Equals(Split(line)[primaryKeyIndex.Value], lookupString))
                        break;

                file.DeleteLine(index);
            }
        }
        
        private string GetFileName<TItem>()
            => GetFilePath(typeof(TItem));

        private string GetFilePath(Type t)
            => Path.Combine(_baseDirectory, $"{t.Name}.csv");
                       
        //ToDo: What if columns are not in order that properties are defined in the class?
        //TODO: What if primary key has different name
                             
        private Func<string[], object> GetInstantiationFunc(Type objectType)
        {
            var propTypeAndInstantiators = objectType.GetProperties()
                .Select((property, index) =>
                    new
                    {
                        instantiator = PropertyInstantiatorMapping.ContainsKey(property.PropertyType)
                                ? PropertyInstantiatorMapping[property.PropertyType].Curry(index)
                                : GetNavigationPropertyInstantiator(objectType, property.PropertyType),
                        property
                    }
                ).ToList();

            return cells =>
            {
                var instance = Activator.CreateInstance(objectType);
                propTypeAndInstantiators.ForEach(pti => pti.property.SetValue(instance, pti.instantiator(cells)));
                return instance;
            };
        }


        //To Do: Implement two way navigation properties
        private Func<string[], object> GetNavigationPropertyInstantiator(Type objectType, Type propertyType)
        {
            var foreingKey = GetPrimaryKeyFieldName(propertyType);

            var foreignKeyIndex = GetPropertyIndex(objectType, foreingKey);

            if (foreignKeyIndex == null)
                throw new ArgumentException($"Cannot find foreign key {foreingKey} reference to {objectType.Name} on type {propertyType.Name}");

            return cells => GetInternal(propertyType, cells[foreignKeyIndex.Value]);
        }


        private static object GetPrimaryKeyValueOfItem(object obj)
        {
            var itemType = obj.GetType();
            var primaryKeyFieldName = GetPrimaryKeyFieldName(itemType);
            var primaryKeyIndex = GetPropertyIndex(itemType, primaryKeyFieldName);

            if (!primaryKeyIndex.HasValue)
                throw new ArgumentException($"Excepted primary key {primaryKeyFieldName} missing on  type {itemType.Name}");

            return itemType.GetProperties()[primaryKeyIndex.Value].GetValue(obj);
        }

        private static int? GetPropertyIndex(Type type, string propertyName)
            => type.GetProperties()
                .Select((p, i) => new { property = p, index = i })
                .SingleOrDefault(a => a.property.Name == propertyName)
            ?.index;

        private static string GetPrimaryKeyFieldName(Type type)
            => $"{type.Name}Id";

        //ToDo: Fill Dictionary!!
        private static readonly IDictionary<Type, Func<int, string[], object>> PropertyInstantiatorMapping
            = new Dictionary<Type, Func<int, string[], object>>
        {
            { typeof(int), (index, cells) => int.TryParse(cells[index], out int result) ? result : 0 },
            { typeof(int?), (index, cells) => int.TryParse(cells[index], out int result) ? result : (int?)null },
            { typeof(string), (index, cells) => cells[index] },
            { typeof(float), (index, cells) => float.TryParse(cells[index], out float result) ? result : 0 },
            { typeof(float?), (index, cells) => float.TryParse(cells[index], out float result) ? result : (float?)null },
            { typeof(double), (index, cells) => double.TryParse(cells[index], out double result) ? result : 0 },
            { typeof(double?), (index, cells) => double.TryParse(cells[index], out double result) ? result : (double?)null },
            { typeof(decimal), (index, cells) => decimal.TryParse(cells[index], out decimal result) ? result : 0 },
            { typeof(decimal?), (index, cells) => decimal.TryParse(cells[index], out decimal result) ? result : (decimal?)null },
            { typeof(char), (index, cells) => char.TryParse(cells[index], out char result) ? result : ' ' },
            { typeof(char?), (index, cells) => char.TryParse(cells[index], out char result) ? result : (char?)null },
            { typeof(bool), (index, cells) => bool.TryParse(cells[index], out bool result) ? result : false },
            { typeof(bool?), (index, cells) => bool.TryParse(cells[index], out bool result) ? result : (bool?)null },
            { typeof(DateTime), (index, cells) => DateTime.TryParse(cells[index], out DateTime result) ? result : DateTime.MinValue },
            { typeof(DateTime?), (index, cells) => DateTime.TryParse(cells[index], out DateTime result) ? result : (DateTime?)null }
        };

        private static string GetHeader(Type type)
        {
            var nonNavigationProps = type.GetProperties()
                .Where(p => PropertyInstantiatorMapping.ContainsKey(p.PropertyType))
                .Select(p => p.Name);

            return Csvify(nonNavigationProps);
        }
        
        private static string GetCsvLine(object obj)
        {
            var propValues = obj.GetType()
                .GetProperties()
                .Where(p => PropertyInstantiatorMapping.ContainsKey(p.PropertyType))
                .Select(p => p.GetValue(obj).ToString());

            return Csvify(propValues);
        }

        private static string Csvify(IEnumerable<string> items)
            => string.Join(",", items.Select(p => $"\"{p.ToString()}\""));

        private string[] Split(string line)
            => line.Trim('"').Replace("\",\"", "~").Split('~');
    }
}
