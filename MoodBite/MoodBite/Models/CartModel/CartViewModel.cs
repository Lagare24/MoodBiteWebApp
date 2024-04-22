using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MoodBite.Models.CartModel
{
    public class CartViewModel
    {
        public IEnumerable<string> productName;
        public IEnumerable<decimal> unitPrice;
        public IEnumerable<int> qty;
        public IEnumerable<decimal> totalPrice;
    }
}