using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackMe01.Models
{
    public class AddToCardRequest
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public  string Note { get; set; }

        public string Validate(decimal userBalance)
        {
            if (Price > userBalance)
            {
                return "Your remaining balance is not enough to buy this product.";
            }
            return  String.Empty;
        }
    }
}
