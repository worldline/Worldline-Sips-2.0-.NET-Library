using System.Collections.Generic;
using Newtonsoft.Json;
using StarringJane.Worldline.Sips.Formatting;

namespace StarringJane.Worldline.Sips.Models {
    public class Contact : BaseModel {
        public string Email { get; set; }

        [JsonConverter(typeof (NumericStringConvertor))]
        public string Phone { get; set; }

        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public override SortedDictionary<string, string> ToSortedDictionary() {
            var baseDictionary = base.ToSortedDictionary();
            if (baseDictionary.ContainsKey("Phone")) baseDictionary["Phone"] = RegularExpressions.NonNumeric.Replace(this.Phone, string.Empty);
            return baseDictionary;
        }
    }
}