using System.Text.RegularExpressions;

namespace StarringJane.Worldline.Sips.Formatting {
    internal struct RegularExpressions {
        public static readonly Regex NonNumeric = new Regex(@"[^0-9]", RegexOptions.Compiled);
    }
}