using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackMe01.Models
{
    public class OrderCardListReponse
    {
        public  int Id { get; set; }
        public  int ProductId { get; set; }
        public  int ProductName { get; set; }
        public decimal Price { get; set; }
        public string OrderByUserName { get; set; }
        public  DateTimeOffset OrderDate { get; set; }
    }
}
