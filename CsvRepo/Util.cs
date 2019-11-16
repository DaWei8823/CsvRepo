using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo
{
    public static class Util
    {
        public static Func<P2, R> Curry<P1, P2, R>(this Func<P1, P2, R> func, P1 x)
            => p2 => func(x, p2); 
    }
}
