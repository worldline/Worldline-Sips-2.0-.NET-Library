using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarringJane.Worldline.Sips.Test.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal VAT { get; set; }
        public bool VATIncluded { get; set; }
        public decimal TotalPrice { get; set; }

        public decimal ItemPrice()
        {
            if (this.VATIncluded) return TotalPrice;
            return TotalPrice + VAT;
        }
    }
}