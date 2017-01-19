# Sips .NET library
The Sips .NET library allows developers to connect easily with the **Worldline Sips** platform in their .NET projects. The library provides you with the necessary objects and methods for sending and receiving calls from the [Sips] platform and to complete the payment flow successfully.

Requirements:
* .NET Framework 4.5.1
* Account settings for [Sips]
Worldline also provides us with a test-account:
-- Merchant ID: 002001000000001
-- Version of the key: 1
-- Secret key: 002001000000001_KEY1
This account can only be used through the following URL:
[https://payment-webinit.test.sips-atos.com/paymentInit](https://payment-webinit.test.sips-atos.com/paymentInit)

## Overview
The library provides you with the necessary components to complete a correct payment flow with the Sips platform:
* Create a *PaymentRequest* with all the data needed to retrieve a redirection URL from the Sips platform
* Retrieve the *Redirection* model with data and URL to redirect the user to [Sips]
* Receive a *PaymentInfo* response object from [Sips]

The PaymentRequest is authenticated by comparing the SHA seal: this is a hash of the parameters and a secret passphrase.

## Payment flow
The payment flow consists of three steps between the Merchant's website and the payment server:
1. The user proceeds to payment: the website sends a PaymentRequest to the Sips Paypage JSON gateway
2. The gateway redirects the user to the payment page
3. The internet user returns to the Merchant's website (manual response)
or:
The gateway sends an automatic response to the Merchant's website (automatic response)

### PaymentRequest
The first step to implement a Sips integration in your .NET project is to provide the payment service with the customer and order parameters.
```CSharp
var client = new SipsClient();

// 1. Generate a unique identifier for the transaction (parameters: prefix & orderId)
var transactionReference = client.GetTransactionReference("SipsTest", 12345);

// 2. Create and initialize a PaymentRequest
var merchantId = "002001000000001";
var interfaceVersion = "IR_WS_2.14";
var customerId = "123";

var paymentRequest = client.GetPaymentRequest(merchantId, interfaceVersion, transactionReference);
paymentRequest.SetCustomer(customerId, "nl");
paymentRequest.SetOrderDetails("12345", 1000, "EUR");
paymentRequest.PaymentMeanBrandList.Add("VISA");
// ...

// 3. Set the SecretKey, Seal, SealAlgorithm (default = HMAC-SHA-256) & KeyVersion (default = 1)
paymentRequest.SetSealAndKeyVersion("002001000000001_KEY1");

// 4. Send the PaymentRequest (2nd parameter indicates whether or not to use the test-url)
var redirection = client.SendPaymentRequest(paymentRequest, true);
```

First we create a unique identifier for our transactions. This allows the merchant to keep track of the transaction history. You can also use your own format for the transaction reference.

Then a basic PaymentRequest object is created. The current interface version used by the Sips platform is **IR_WS_2.14**. 

When all other parameters are set, a hash is created for these parameters based on a secret key, the seal algorithm and the key version. This hash parameter is called the *Seal*.

### Redirection
When the PaymentRequest parameters are all valid, the Sips Paypage JSON gateway will provide you with the redirection instructions.
```CSharp
// 4. Send the PaymentRequest (2nd parameter indicates whether or not to use the test-url)
var redirection = client.SendPaymentRequest(paymentRequest, true);

// 5. If the payment request was successful (RedirectionStatusCode == "00") redirect to Worldline
if (redirection != null && redirection.RedirectionStatusCode == "00")
    client.RedirectToWorldline(redirection);
else
{
    // Error handling ...
}
```

### PaymentInfo
The response from the payment flow is a PaymentInfo object. This object is typically used in a separate endpoint that is available to Sips. These endpoints can be defined in your Sips account, or you can pass them as parameters with your PaymentRequest.
```CSharp
var normalReturnUrl = "https://your.website.com/payment/response/normal";
var automaticResponseUrl = "https://your.website.com/payment/response/automatic";
var paymentRequest = client.GetPaymentRequest(merchantId, interfaceVersion, transactionReference, normalReturnUrl, automaticResponseUrl);
```
When the payment is completed and the user clicks on "Return to shop" the Sips platform will send a HTTP(s) POST request with the response data to the *normal response URL*. Because there is no guarantee that the user will click on this link, consequently there is no guarantee that the manual response will be received.

If the payment was completed without clicking on the link, the Sips payment servers will send an automatic response to the *automatic response URL*.
```CSharp
var paymentInfo = new PaymentInfo(Request.Form);
if (paymentInfo.Data["responseCode"] == "00" || paymentInfo.Data["responseCode"] == "60")
{
    // Order successful (accepted or pending)
    // Save the payment data in your website's database, f.e. paymentInfo.Data["transactionReference"]
} else if (paymentInfo.Data["responseCode"] == "17")
{
    // Order cancelled
} else
{
    // You can find more information on the response codes in the appendix
}
```
## Appendix
### responseCode
| Values | Description |
|--------|-------------|
| 00 | Authorisation accepted |  
| 02 | Authorisation request to be performed via telephone with the issuer, as the card authorisation threshold has been exceeded. You need to be authorised to force transactions |
| 03 | Invalid Merchant contract |
| 05 | Authorisation refused |
| 11 | Used for differed check. The PAN is blocked. |
| 12 | Invalid transaction, check the request parameters | 
| 14 | Invalid PAN or payment mean data (ex: card security code) |
| 17 | Buyer cancellation |
| 24 | Operation not authorized. The operation you wish to perform is not compliant with the transaction status |
| 25 | Transaction unknown by Sips |
| 30 | Format error |
| 34 | Fraud suspicion (seal erroneous) |
| 40 | Function not supported: the operation that you wish to perform is not part of the operation type for which you are authorised |
| 51 | Amount too high |
| 54 | Payment mean expiry date is past |
| 60 | Transaction pending |
| 63 | Security rules not observed, transaction stopped |
| 75 | Exceeded number of PAN attempts |
| 90 | Service temporarily not available |
| 94 | Duplicated transaction: the transactionReference has been used previously |
| 97 |Time frame exceeded, transaction refused |
| 99 | Temporary problem at the Sips server level |
 