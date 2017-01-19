using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StarringJane.Worldline.Sips.Models {
    public class BaseModel : IModel {
        public virtual SortedDictionary<string, string> ToSortedDictionary() {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var sd = new SortedDictionary<string, string>();
            foreach (var property in properties) {
                if (property.PropertyType.Name.StartsWith("List")) continue;
                if (property.PropertyType.Name == "String") {
                    var value = property.GetValue(this)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                        sd.Add(property.Name, value);
                } else {
                    var childProperties = property.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    if (childProperties.Any()) {
                        var childSortedDictionary = (property.GetValue(this) as IModel)?.ToSortedDictionary();
                        if (childSortedDictionary != null) {
                            foreach (var item in childSortedDictionary) {
                                sd.Add($"{property.Name}.{item.Key}", item.Value);
                            }
                        }
                    }
                }
            }
            return sd;
        }
    }
}