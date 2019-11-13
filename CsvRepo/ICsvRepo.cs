using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public interface IRepo
    {
        IEnumerable<TItems> GetAll<TItems>();
        TItem Get<TItem, TKey>(TKey key);
        IEnumerable<TItem> Get<TItem>(Func<TItem, bool> pred);

        void Add<TItem>(TItem item);
        void Update<TItem>(TItem item);
        void Delete<TItem>(TItem item);
        void Delete<TItem, TKey>(TKey key);
    }
}
