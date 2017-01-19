using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StarringJane.Worldline.Sips.Models;

namespace StarringJane.Worldline.Sips {
    public class SipsClient {
        #region Useful objects and data

        /// <summary>
        /// Gets the transaction reference based on a prefix and an order id (max length = 35). The transaction
        /// reference should be unique.
        /// </summary>
        /// <param name="prefix">Prefix: this could f.e. be your store name</param>
        /// <param name="orderId">OrderId</param>
        /// <returns>String with the concatenated transaction reference</returns>
        public string GetTransactionReference(string prefix, int orderId) {
            var trans = string.Format("{0}{1}", prefix, orderId);
            return trans.Substring(0, Math.Min(35, trans.Length));
        }

        /// <summary>
        /// Get the payment request url (useSandbox = 'true' gives the test payment request url)
        /// </summary>
        /// <param name="useSandbox">If true, you'll receive the test url for payment requests. Default = false.</param>
        /// <returns></returns>
        public string GetPaymentRequestUrl(bool useSandbox = false) {
            return useSandbox ? "https://payment-webinit.simu.sips-atos.com/rs-services/v2/paymentInit/"
                : "https://payment-webinit.sips-atos.com/rs-services/v2/paymentInit/";
        }

        /// <summary>
        /// Get a basic PaymentRequestModel for sending the payment request to Worldline.
        /// </summary>
        /// <returns>Empty PaymentRequestModel</returns>
        public PaymentRequest GetPaymentRequest() {
            var request = new PaymentRequest();
            return request;
        }

        /// <summary>
        /// Get a PaymentRequestModel for sending the payment request to Worldline. If the payment request is valid a RedirectionModel 
        /// will be retrieved from Worldline with the payment redirection url.
        /// </summary>
        /// <param name="merchantId"></param>
        /// <param name="interfaceVersion"></param>
        /// <param name="transactionReference"></param>
        /// <param name="normalReturnUrl"></param>
        /// <param name="automaticResponseUrl"></param>
        /// <returns>PaymentRequestModel with merchantId, interfaceVersion, normalReturnUrl and the transactionReference</returns>
        public PaymentRequest GetPaymentRequest(string merchantId, string interfaceVersion, string transactionReference, string normalReturnUrl = null, string automaticResponseUrl = null) {
            var request = GetPaymentRequest();
            IPHostEntry hostInfo = Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            var ipAddress = hostInfo.AddressList.FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            request.MerchantId = merchantId;
            request.InterfaceVersion = interfaceVersion;
            request.NormalReturnUrl = normalReturnUrl;
            request.AutomaticResponseUrl = automaticResponseUrl;
            request.TransactionReference = transactionReference;
            request.CustomerIpAddress = ipAddress;
            return request;
        }

        /// <summary>
        /// Converts json result string from payment request to a redirection model.
        /// </summary>
        /// <param name="jsonresult">The json result string retrieved from the Worldline payment request</param>
        /// <returns>RedirectionModel</returns>
        public RedirectionModel SendPaymentRequest(string jsonresult) {
            var json = JsonConvert.DeserializeObject<RedirectionModel>(
                jsonresult,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}
                );

            if (json != null) return json;

            return new RedirectionModel {RedirectionStatusCode = "01", RedirectionStatusMessage = "Worldline payment request failed: invalid json result string."};
        }

        /// <summary>
        /// Gets the payment info object from a successful payment request.
        /// </summary>
        /// <param name="form">Form-data retrieved from the payment request to Worldline</param>
        /// <returns>PaymentInfoModel with data as a KeyValue dictionary</returns>
        public PaymentInfo GetPaymentInfo(NameValueCollection form) {
            var data = string.Empty;
            var encode = string.Empty;
            var seal = string.Empty;
            var interfaceVersion = string.Empty;

            string[] keys = form.AllKeys;
            for (int i = 0; i < keys.Length; i++) {
                if (keys[i] == "Data") data = form[keys[i]];
                else if (keys[i] == "Encode") encode = form[keys[i]];
                else if (keys[i] == "Seal") seal = form[keys[i]];
                else if (keys[i] == "InterfaceVersion") interfaceVersion = form[keys[i]];
            }
            return GetPaymentInfo(data, encode, seal, interfaceVersion);
        }

        /// <summary>
        /// Gets the payment info object from a successful payment request.
        /// </summary>
        /// <param name="data">Concatenation of fields in response ( format: [key1]=[value1]|[key2]=[value2]|.. )</param>
        /// <param name="encode">Type of encoding used</param>
        /// <param name="seal">Signature of the message</param>
        /// <param name="interfaceVersion">Version of the context message</param>
        /// <returns>PaymentInfoModel with data as a KeyValue dictionary</returns>
        public PaymentInfo GetPaymentInfo(string data, string encode, string seal, string interfaceVersion) {
            var paymentInfo = new PaymentInfo() {
                Encode = encode,
                Seal = seal,
                InterfaceVersion = interfaceVersion
            };
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(data)) {
                var paymentData = data.Split('|');
                foreach (var s in paymentData) {
                    if (s.Split('=').Length > 1) dict.Add(s.Split('=')[0], s.Split('=')[1]);
                }
            }
            paymentInfo.Data = dict;
            return paymentInfo;
        }

        #endregion

        #region Interaction with Worldline

        /// <summary>
        /// Send a payment request to Worldline. If successful Worldline returns a redirection model with the data to call the Worldline payment provider.
        /// </summary>
        /// <param name="request">PaymentRequestModel</param>
        /// <param name="useSandbox">Flag indicating whether the Worldline test url should be used</param>
        /// <returns>RedirectionModel</returns>
        public RedirectionModel SendPaymentRequest(PaymentRequest request, bool useSandbox = false) {
            var url = GetPaymentRequestUrl(useSandbox);
            return SendPaymentRequest(request, url);
        }

        /// <summary>
        /// Send a payment request to Worldline. If successful Worldline returns a redirection model with the data to call the Worldline payment provider.
        /// </summary>
        /// <param name="request">PaymentRequestModel</param>
        /// <param name="paymentRequestUrl">Worldline url where the payment request will be sent to</param>
        /// <returns>RedirectionModel</returns>
        public RedirectionModel SendPaymentRequest(PaymentRequest request, string paymentRequestUrl) {
            var result = _sendPaymentRequest(request, paymentRequestUrl);
            if (!string.IsNullOrEmpty(result))
                return SendPaymentRequest(result);
            return null;
        }

        private string _sendPaymentRequest(PaymentRequest request, string paymentRequestUrl) {
            var dataToSend = JsonConvert.SerializeObject(
                request,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}
                );
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(paymentRequestUrl);

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = Encoding.UTF8.GetByteCount(dataToSend);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "application/json";

            var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());

            streamWriter.Write(dataToSend);
            streamWriter.Close();

            var reponseStream = (httpWebRequest.GetResponse() as HttpWebResponse)?
                .GetResponseStream();
            if (reponseStream != null) {
                var streamReader = new StreamReader(reponseStream);
                var stringResult = streamReader.ReadToEnd();
                return stringResult;
            }
            return null;
        }

        /// <summary>
        /// Redirects to the Worldline payment module. You can only do this after you've received a redirectionUrl, redirectionData and redirectionVersion
        /// from a Worldline payment request.
        /// </summary>
        /// <param name="redirect">RedirectionModel</param>
        public void RedirectToWorldline(RedirectionModel redirect) {
            RedirectToWorldline(redirect.RedirectionUrl, redirect.RedirectionVersion, redirect.RedirectionData);
        }

        /// <summary>
        /// Redirects to the Worldline payment module. You can only do this after you've received a redirectionUrl, redirectionData and redirectionVersion 
        /// from a Worldline payment request.
        /// </summary>
        /// <param name="redirectionUrl">The Url to wich to redirect</param>
        /// <param name="redirectionVersion"></param>
        /// <param name="redirectionData"></param>
        public void RedirectToWorldline(string redirectionUrl, string redirectionVersion, string redirectionData) {
            var url = string.Format("{0}?redirectionVersion={1}&redirectionData={2}", redirectionUrl, HttpUtility.UrlEncode(redirectionVersion), HttpUtility.UrlEncode(redirectionData));
            HttpContext.Current.Response.Redirect(url);
        }

        #endregion
    }
}