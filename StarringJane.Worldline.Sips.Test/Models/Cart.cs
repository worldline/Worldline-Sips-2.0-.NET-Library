using StarringJane.Worldline.Sips.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarringJane.Worldline.Sips.Test.Models
{
    public class Cart
    {
        public Cart()
        {
            CartItems = new List<CartItem>();
        }

        public List<CartItem> CartItems;

        public string CustomerId { get; set; }
        public Contact BillingContact { get; set; }
        public Address BillingAddress { get; set; }

        public Contact ShippingContact { get; set; }
        public Address ShippingAddress { get; set; }

        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }

        public string Error { get; set; }
    }
}