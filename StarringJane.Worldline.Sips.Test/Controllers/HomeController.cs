using StarringJane.Worldline.Sips.Test.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarringJane.Worldline.Sips.Test.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var cart = new Cart {
                CustomerId = new Guid().ToString().Substring(0,19), // CustomerId max length = 19
                BillingContact = new Sips.Models.Contact() { Firstname = "Jan", Lastname = "Janssens", Email = "dummy@dummy.be", Phone = "+329 235 82 82" },
                BillingAddress = new Sips.Models.Address() { Street = "Dendermondsesteenweg 39 A001", City = "Gent", ZipCode = "9000", Country = "BEL", Company = "Starring Jane" },
                ShippingContact = new Sips.Models.Contact() { Firstname = "Jan", Lastname = "Janssens", Email = "dummy@dummy.be", Phone = "003292358282" },
                ShippingAddress = new Sips.Models.Address() { Street = "Dendermondsesteenweg 39 A001", City = "Gent", ZipCode = "9000", Country = "BEL", Company = "Starring Jane" },
            };
            SetDummyShoppingCart(cart);

            return View(cart);
        }

        [HttpPost]
        public ActionResult Index(Cart cart)
        {
            // Set the dummy data for the shopping cart again (normally you can retrieve this data from your webshop database)
            SetDummyShoppingCart(cart);
            int orderId = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var client = new SipsClient();

            // 1. Generate a unique identifier for this transaction (so you keep track of the transaction history)
            //    GetTransactionReference concats your prefix and the order id, but you can use your own format for the transaction reference.
            var transactionReference = client.GetTransactionReference("WLSipsTest", orderId);

            // 2. Create and initialize a PaymentRequest
            var merchantId = ConfigurationManager.AppSettings["MerchantId"].ToString(); // Use "002001000000001" if you're using the Sandbox uri
            var interfaceVersion = ConfigurationManager.AppSettings["InterfaceVersion"].ToString(); // "IR_WS_2.14" is the current interface version
            var normalReturnUrl = Url.Action("Completed", "Cart", null, Request.Url.Scheme);
            var automaticResponseUrl = Url.Action("OrderPaid", "Cart", null, Request.Url.Scheme);
            var paymentRequest = client.GetPaymentRequest(merchantId, interfaceVersion, transactionReference, normalReturnUrl, automaticResponseUrl);

            // 3. Set your PaymentRequest data (not Seal, SealAlgorithm, Key & KeyVersion)
            paymentRequest.SetCustomer(cart.CustomerId, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            paymentRequest.SetAddressAndContactInfo(cart.BillingContact, cart.BillingAddress, Sips.Models.AddressType.Customer);
            paymentRequest.SetAddressAndContactInfo(cart.BillingContact, cart.BillingAddress, Sips.Models.AddressType.Billing);
            paymentRequest.SetAddressAndContactInfo(cart.ShippingContact, cart.ShippingAddress, Sips.Models.AddressType.Delivery);
            // Worldline only accepts integers for totalAmount, so we need to multiply the decimal value by 100
            paymentRequest.SetOrderDetails(orderId.ToString(), (int)Math.Round(cart.Total * 100), "EUR");
            foreach (var item in cart.CartItems)
                paymentRequest.ShoppingCartDetail.ShoppingCartItemList.Add(new Sips.Models.ShoppingCartItem()
                {
                    ProductSKU = item.Sku,
                    ProductName = item.ProductName,
                    ProductQuantity = item.Quantity.ToString(),
                    ProductUnitAmount = ((int)Math.Round(item.UnitPrice * 100)).ToString()
                });

            paymentRequest.PaymentMeanBrandList.Add("VISA");
            paymentRequest.PaymentMeanBrandList.Add("MASTERCARD");

            // 4. Now set Seal, SealAlgorithm, Key & KeyVersion for your PaymentRequest
            var secretKey = ConfigurationManager.AppSettings["SecretKey"].ToString();
            paymentRequest.SetSealAndKeyVersion(secretKey); // Use "002001000000001_KEY1" if you're using the Sandbox uri

            // 5. Send your PaymentRequest to Worldline & receive a RedirectionModel (with redirection Uri and data)
            var redirection = client.SendPaymentRequest(paymentRequest, true);

            // 6. If the payment request was successful (RedirectionStatusCode == "00") redirect to Worldline
            if (redirection != null && redirection.RedirectionStatusCode == "00")
                client.RedirectToWorldline(redirection);

            // 7. If the payment request was not succesful show an error message
            if (redirection != null)
            {
                switch (redirection.RedirectionStatusCode)
                {
                    case "30": cart.Error = "Request format is not valid."; break;
                    case "34": cart.Error = "There is a security problem: for example, the calculated seal is incorrect."; break;
                    case "94": cart.Error = "Transaction already exists."; break;
                    case "99": cart.Error = "Service temporarily unavailable."; break;
                    default: cart.Error = redirection.RedirectionStatusMessage; break;
                }
            }
            else
                cart.Error = "Unable to retrieve a redirection model.";

            return View(cart);
        }

        private void SetDummyShoppingCart(Cart cart)
        {
            var shippingCost = 5;

            var cartItems = new List<CartItem>()
            {
                new CartItem() { Id = 1, Sku = "1234", ImageUrl = "~/Images/Schoenen.jpg", ProductName = "Schoenen", Quantity = 2, UnitPrice = 35, TotalPrice = 70, VAT = 14.7m, VATIncluded = true },
                new CartItem() { Id = 2, Sku = "1235", ImageUrl = "~/Images/Laarzen.jpg", ProductName = "Laarzen", Quantity = 1, UnitPrice = 55, TotalPrice = 55, VAT = 11.55m, VATIncluded = true }
            };

            cart.CartItems = cartItems;
            cart.SubTotal = cartItems.Sum(x => x.TotalPrice);
            cart.ShippingCost = shippingCost;
            cart.VAT = cartItems.Sum(x => x.VAT);
            cart.Total = cartItems.Sum(x => x.ItemPrice()) + shippingCost;
        }
    }
}