using System.Collections.Generic;

namespace StarringJane.Worldline.Sips.Models {
    public interface IModel {
        SortedDictionary<string, string> ToSortedDictionary();
    }
}