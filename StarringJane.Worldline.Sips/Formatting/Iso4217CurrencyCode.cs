using System.Collections.Generic;
using System.Linq;

namespace StarringJane.Worldline.Sips.Formatting {
    /// <summary>
    /// Helper class for working with Iso 4217 currency codes
    /// </summary>
    internal static class Iso4217CurrencyCode {
        private static readonly Dictionary<string, int> _currencyConversion = new Dictionary<string, int> {
            {"AFN", 971}, {"ALL", 8}, {"DZD", 12}, {"USD", 840}, {"EUR", 978}, {"ADP", 20}, {"AOA", 973}, {"XCD", 951}, {"ARS", 32}, {"AMD", 51}, {"AWG", 533}, {"AUD", 36},
            {"ATS", 40}, {"AZN", 31}, {"BSD", 44}, {"BHD", 48}, {"BDT", 50}, {"BBD", 52}, {"BYR", 974}, {"BEF", 56}, {"BZD", 84}, {"XOF", 952}, {"BMD", 60}, {"BTN", 64},
            {"INR", 356}, {"BOB", 68}, {"BAM", 977}, {"BWP", 72}, {"BRL", 986}, {"BND", 96}, {"BGN", 975}, {"BIF", 108}, {"KHR", 116}, {"XAF", 950}, {"CAD", 124}, {"CVE", 132},
            {"KYD", 136}, {"CLP", 152}, {"CNY", 156}, {"COP", 170}, {"KMF", 174}, {"CDF", 976}, {"NZD", 554}, {"CRC", 188}, {"HRK", 191}, {"CUP", 192}, {"TRY", 949}, {"CZK", 203},
            {"DKK", 208}, {"DJF", 262}, {"DOP", 214}, {"EGP", 818}, {"SVC", 222}, {"ERN", 232}, {"EEK", 233}, {"ETB", 230}, {"FKP", 238}, {"FJD", 242}, {"FIM", 246}, {"FRF", 250},
            {"XPF", 953}, {"GMD", 270}, {"GEL", 981}, {"DEM", 280}, {"GHC", 288}, {"GIP", 292}, {"GRD", 300}, {"GTQ", 320}, {"GNF", 324}, {"GWP", 624}, {"GYD", 328}, {"HTG", 332},
            {"ITL", 380}, {"HNL", 340}, {"HKD", 344}, {"HUF", 348}, {"ISK", 352}, {"IDR", 360}, {"IRR", 364}, {"IQD", 368}, {"IEP", 372}, {"ILS", 376}, {"JMD", 388}, {"JPY", 392},
            {"JOD", 400}, {"KZT", 398}, {"KES", 404}, {"KPW", 408}, {"KRW", 410}, {"KWD", 414}, {"KGS", 417}, {"LAK", 418}, {"LBP", 422}, {"LSL", 426}, {"LRD", 430}, {"LYD", 434},
            {"CHF", 756}, {"LTL", 440}, {"LUF", 442}, {"MOP", 446}, {"MKD", 807}, {"MGF", 450}, {"MWK", 454}, {"MYR", 458}, {"MVR", 462}, {"MTL", 470}, {"MRO", 478}, {"MUR", 480},
            {"MXN", 484}, {"MXV", 979}, {"MDL", 498}, {"MNT", 496}, {"MAD", 504}, {"MZM", 508}, {"MMK", 104}, {"ZAR", 710}, {"NAD", 516}, {"NPR", 524}, {"NLG", 528}, {"ANG", 532},
            {"NIO", 558}, {"NGN", 566}, {"NOK", 578}, {"OMR", 512}, {"PKR", 586}, {"PAB", 590}, {"PGK", 598}, {"PYG", 600}, {"PEN", 604}, {"PHP", 608}, {"PLN", 985}, {"PTE", 620},
            {"QAR", 634}, {"ROL", 642}, {"RUR", 810}, {"RUB", 643}, {"RWF", 646}, {"SHP", 654}, {"WST", 882}, {"STD", 678}, {"SAR", 682}, {"CSD", 891}, {"SCR", 690}, {"SLL", 694},
            {"SGD", 702}, {"SKK", 703}, {"SIT", 705}, {"SBD", 90}, {"SOS", 706}, {"SSP", 728}, {"ESP", 724}, {"LKR", 144}, {"SDD", 736}, {"SDG", 938}, {"SRG", 740}, {"SZL", 748},
            {"SEK", 752}, {"SYP", 760}, {"TWD", 901}, {"TJS", 972}, {"TZS", 834}, {"THB", 764}, {"TOP", 776}, {"TTD", 780}, {"TND", 788}, {"TMM", 795}, {"UGX", 800}, {"UAH", 980},
            {"AED", 784}, {"GBP", 826}, {"USS", 998}, {"USN", 997}, {"UYU", 858}, {"UZS", 860}, {"VUV", 548}, {"VEF", 937}, {"VEB", 862}, {"VND", 704}, {"YER", 886}, {"ZMK", 894},
            {"ZWD", 716}
        };

        /// <summary>
        /// Check if the provided code is a valid Iso 4217 currency code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsDefined(string code) {
            return _currencyConversion.ContainsKey(code.Trim().ToUpperInvariant());
        }

        /// <summary>
        /// Check if the provided code isv a valid Iso 4217 currency code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool IsDefined(int code) {
            return _currencyConversion.ContainsValue(code);
        }

        /// <summary>
        /// Get the numeric representation of an Iso 4217 currency code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static int GetNumeric(string code) {
            if (IsDefined(code))
                return _currencyConversion[code.Trim().ToUpperInvariant()];
            return 0;
        }

        /// <summary>
        /// Get the string representation of a numeric Iso 4217 currency code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetAlpha(int code) {
            if (IsDefined(code))
                return _currencyConversion.FirstOrDefault(x => x.Value == code).Key;
            return string.Empty;
        }
    }
}