using StarringJane.Worldline.Sips.Models;
using StarringJane.Worldline.Sips.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StarringJane.Worldline.Sips.Test.Controllers
{
    public class CartController : Controller
    {
        public ActionResult Completed()
        {
            var paymentInfo = new PaymentInfo(Request.Form);
            if (paymentInfo.Data["responseCode"] == "17")
            {
                // Order cancelled
            } else if (paymentInfo.Data["responseCode"] == "00")
            {
                // Order completed
            }
            return View(paymentInfo);
        }
        public ActionResult OrderPaid() {
            var paymentInfo = new PaymentInfo(Request.Form);
            if (paymentInfo.Data["responseCode"] == "17") {
                // Order cancelled
            } else if (paymentInfo.Data["responseCode"] == "00") {
                // Order completed
            }
            return View(paymentInfo);
        }
    }
}