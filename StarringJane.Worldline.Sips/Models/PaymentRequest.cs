using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using StarringJane.Worldline.Sips.Formatting;

namespace StarringJane.Worldline.Sips.Models {
    public class PaymentRequest : BaseModel {
        public PaymentRequest() {
            PaymentMeanBrandList = new List<string>();
            CustomerAddress = new Address();
            CustomerContact = new Contact();
            BillingAddress = new Address();
            BillingContact = new Contact();
            DeliveryAddress = new Address();
            DeliveryContact = new Contact();
            ShoppingCartDetail = new ShoppingCartDetail();
        }

        public string MerchantId { get; set; }
        public string InterfaceVersion { get; set; }
        public string NormalReturnUrl { get; set; }
        public string AutomaticResponseUrl { get; set; }
        public string TransactionReference { get; set; }
        public string Seal { get; set; }
        public string KeyVersion { get; set; }
        public string SealAlgorithm { get; set; }
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string OrderChannel { get; set; }
        public string CustomerId { get; set; }
        public string CustomerIpAddress { get; set; }
        public string CustomerLanguage { get; set; }
        public ShoppingCartDetail ShoppingCartDetail { get; set; }
        public Address CustomerAddress { get; set; }
        public Contact CustomerContact { get; set; }
        public Address BillingAddress { get; set; }
        public Contact BillingContact { get; set; }
        public Address DeliveryAddress { get; set; }
        public Contact DeliveryContact { get; set; }
        public List<string> PaymentMeanBrandList { get; set; }

        #region Utilities

        public override SortedDictionary<string, string> ToSortedDictionary() {
            var sd = base.ToSortedDictionary();
            for (var i = 0; i < PaymentMeanBrandList.Count; i++) {
                sd.Add($"PaymentMeanBrandList[{i}]", PaymentMeanBrandList[i]);
            }
            return sd;
        }

        private string _byteArrayToHEX(byte[] ba) {
            StringBuilder hex = new StringBuilder(ba.Length*2);
            foreach (byte b in ba) {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds customer id and language to the PaymentRequest object.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="customerLanguage">ISO 639-1 language code (f.e. en, nl, de)</param>
        public void SetCustomer(string customerId, string customerLanguage) {
            CustomerId = customerId;
            CustomerLanguage = customerLanguage;
        }

        /// <summary>
        /// Adds order details to the PaymentRequest object.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="totalAmount">Integer value for total amount (f.e. € 9,99 becomes 999)</param>
        /// <param name="currency">ISO 4217 numeric currency code (f.e. 978 for EUR)</param>
        public void SetOrderDetails(string orderId, int totalAmount, int currency) {
            OrderId = orderId;
            Amount = totalAmount.ToString(CultureInfo.InvariantCulture);
            if (Iso4217CurrencyCode.IsDefined(currency))
                CurrencyCode = currency.ToString();
            ShoppingCartDetail.ShoppingCartTotalAmount = totalAmount.ToString(CultureInfo.InvariantCulture);
            OrderChannel = "INTERNET";
        }

        /// <summary>
        /// Adds order details to the PaymentRequest object.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="totalAmount">Integer value for total amount (f.e. € 9,99 becomes 999)</param>
        /// <param name="currency">ISO 4217 currency code (f.e. EUR)</param>
        public void SetOrderDetails(string orderId, int totalAmount, string currency) {
            SetOrderDetails(
                orderId,
                totalAmount,
                Iso4217CurrencyCode.GetNumeric(currency)
                );
        }

        /// <summary>
        /// Adds address and contact information for customer to the PaymentRequest object.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="addressType">Enumeration with 3 possible values: Customer, Billing and Delivery</param>
        /// <returns></returns>
        public void SetAddressAndContactInfo(Contact contact, Address address, AddressType addressType) {
            if (addressType == AddressType.Billing) {
                BillingAddress = address;
                BillingContact = contact;
            } else if (addressType == AddressType.Delivery) {
                DeliveryAddress = address;
                DeliveryContact = contact;
            } else if (addressType == AddressType.Customer) {
                CustomerAddress = address;
                CustomerContact = contact;
            }
        }

        /// <summary>
        /// Get the seal value for the payment request. This is the hash-based message authentication code (HMAC) for the request.
        /// </summary>
        /// <param name="key">The mutual key for the merchant site and Worldline.</param>
        /// <param name="sealAlgorithm">Seal algorithm. Default value is "HMAC-SHA-256".</param>
        /// <returns>A string with the hashed seal value for the payment request.</returns>
        public string GetSeal(string key, string sealAlgorithm = "HMAC-SHA-256") {
            var sd = ToSortedDictionary();
            string sChain = string.Concat(sd.Values);
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(sChain);

            HMAC hmac = null;
            switch (sealAlgorithm) {
                case "HMAC-SHA-1":
                    hmac = new HMACSHA1();
                    break;
                case "HMAC-SHA-384":
                    hmac = new HMACSHA384();
                    break;
                case "HMAC-SHA-512":
                    hmac = new HMACSHA512();
                    break;
                default:
                    hmac = new HMACSHA256();
                    break;
            }
            hmac.Key = utf8.GetBytes(key);
            hmac.Initialize();

            byte[] shaResult = hmac.ComputeHash(encodedBytes);
            return _byteArrayToHEX(shaResult);
        }

        /// <summary>
        /// Add seal information and key version to the PaymentRequestModel.
        /// </summary>
        /// <param name="seal">Payment request seal.</param>
        /// <param name="sealAlgorithm">Seal algorithm. Default value is "HMAC-SHA-256".</param>
        /// <param name="keyVersion">Key version. Default value is "1".</param>
        public void SetSealAndKeyVersion(string key, string seal = null, string sealAlgorithm = "HMAC-SHA-256", string keyVersion = "1") {
            if (string.IsNullOrEmpty(seal))
                seal = GetSeal(key, sealAlgorithm);
            Seal = seal;
            SealAlgorithm = sealAlgorithm;
            KeyVersion = keyVersion;
        }

        #endregion
    }
}