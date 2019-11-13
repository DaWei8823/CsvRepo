using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvRepo.Sample
{
    public class Order
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int CustomerId { get; set; }
        public Item Item { get; set; }
        public Customer Customer { get; set; }
    }
}
