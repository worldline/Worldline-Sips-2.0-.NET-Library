using System.Collections.Generic;
using System.Collections.Specialized;

namespace StarringJane.Worldline.Sips.Models {
    public class PaymentInfo {
        public PaymentInfo() {
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a PaymentInfo object based on the form data received from the Worldline Sips payment service
        /// </summary>
        /// <param name="form">Collection of form values received by the request from Worldline</param>
        public PaymentInfo(NameValueCollection form) {
            Data = new Dictionary<string, string>();
            string[] keys = form.AllKeys;
            var data = string.Empty;
            for (int i = 0; i < keys.Length; i++) {
                if (keys[i] == "Data") data = form[keys[i]];
                else if (keys[i] == "Encode") Encode = form[keys[i]];
                else if (keys[i] == "Seal") Seal = form[keys[i]];
                else if (keys[i] == "InterfaceVersion") InterfaceVersion = form[keys[i]];
            }
            if (!string.IsNullOrEmpty(data)) {
                var paymentData = data.Split('|');
                var dict = new Dictionary<string, string>();
                foreach (var s in paymentData) {
                    if (s.Split('=').Length > 1) dict.Add(s.Split('=')[0], s.Split('=')[1]);
                    if (s.Split('=')[0] == "responseCode") dict.Add("responseMessage", _getResponseMessage(s.Split('=')[1]));
                }
                Data = dict;
            }
        }

        public Dictionary<string, string> Data { get; set; }
        public string Encode { get; set; }
        public string Seal { get; set; }
        public string InterfaceVersion { get; set; }

        #region Utilities

        private string _getResponseMessage(string code) {
            switch (code) {
                case "00":
                    return "Authorisation accepted.";
                case "02":
                    return "Authorisation request to be performed via telephone with the issuer, as the card authorisation threshold has been exceeded. You need to be authorised to force transactions.";
                case "03":
                    return "Invalid Merchant contract";
                case "05":
                    return "Authorisation refused";
                case "11":
                    return "Used for differed check. The PAN is blocked.";
                case "12":
                    return "Invalid transaction, check the request parameters.";
                case "14":
                    return "Invalid PAN or payment mean data(ex: card security code).";
                case "17":
                    return "Buyer cancellation.";
                case "24":
                    return "Operation not authorized. The operation you wish to perform is not compliant with the transaction status.";
                case "25":
                    return "Transaction unknown by Sips.";
                case "30":
                    return "Format error.";
                case "34":
                    return "Fraud suspicion(seal erroneous).";
                case "40":
                    return "Function not supported: the operation that you wish to perform is not part of the operation type for which you are authorised.";
                case "51":
                    return "Amount too high.";
                case "54":
                    return "Payment mean expiry date is past.";
                case "60":
                    return "Transaction pending.";
                case "63":
                    return "Security rules not observed, transaction stopped.";
                case "75":
                    return "Exceeded number of PAN attempts.";
                case "90":
                    return "Service temporarily not available.";
                case "94":
                    return "Duplicated transaction: the transactionReference has been used previously.";
                case "97":
                    return "Time frame exceeded, transaction refused.";
                case "99":
                    return "Temporary problem at the Sips server level.";
                default:
                    return "Responsecode unknown.";
            }
        }

        #endregion
    }
}