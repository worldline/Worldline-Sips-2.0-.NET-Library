using System.Collections.Generic;

namespace StarringJane.Worldline.Sips.Models {
    public class ShoppingCartDetail : BaseModel {
        public ShoppingCartDetail() {
            ShoppingCartItemList = new List<ShoppingCartItem>();
        }

        public string ShoppingCartTotalAmount { get; set; }
        public List<ShoppingCartItem> ShoppingCartItemList { get; set; }

        #region Utilities

        public override SortedDictionary<string, string> ToSortedDictionary() {
            var sd = base.ToSortedDictionary();
            for (var i = 0; i < ShoppingCartItemList.Count; i++) {
                foreach (var item in ShoppingCartItemList[i].ToSortedDictionary()) {
                    sd.Add($"ShoppingCartItemList[{i}].{item.Key}", item.Value);
                }
            }
            return sd;
        }

        #endregion
    }
}