using System.Collections.Generic;

namespace CsvRepo
{
    public interface ICsvRepo
    {
        ICollection<TItem> Get<TItem>() where TItem : class;
        TItem Get<TItem>(object key) where TItem : class;

        void Add<TItem>(TItem item);
        void AddRange<TItem>(IEnumerable<TItem> items);
        void Update<TItem>(TItem item);
        void Delete<TItem, TKey>(TKey key);
    }
}
