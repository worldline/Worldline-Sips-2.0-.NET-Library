namespace StarringJane.Worldline.Sips.Models {
    public class Address : BaseModel {
        public string Street { get; set; }
        public string Company { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }
}