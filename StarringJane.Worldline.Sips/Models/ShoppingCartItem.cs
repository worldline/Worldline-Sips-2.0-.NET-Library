namespace StarringJane.Worldline.Sips.Models {
    public class ShoppingCartItem : BaseModel {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductSKU { get; set; }
        public string ProductUnitAmount { get; set; }
        public string ProductQuantity { get; set; }
        public string ProductUnitTaxAmount { get; set; }
        public string ProductCategory { get; set; }
        public string ProductTaxRate { get; set; }
    }
}