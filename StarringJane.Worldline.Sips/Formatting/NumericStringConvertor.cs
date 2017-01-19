using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StarringJane.Worldline.Sips.Formatting {
    internal class NumericStringConvertor : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return (objectType == typeof (string));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return JToken.Load(reader)?.ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value != null) {
                var numericValue = RegularExpressions.NonNumeric.Replace(value.ToString(), string.Empty);
                if (!string.IsNullOrWhiteSpace(numericValue)) {
                    var val = JToken.FromObject(numericValue);
                    val.WriteTo(writer);
                }
            }
        }
    }
}