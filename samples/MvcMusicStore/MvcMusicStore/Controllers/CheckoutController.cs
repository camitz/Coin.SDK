using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Coin.SDK;
using Coin.SDK.Model;
using MvcMusicStore.Models;
using Order = MvcMusicStore.Models.Order;

namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();
        const string PromoCode = "FREE";

        //
        // GET: /Checkout/AddressAndPayment

        public ActionResult AddressAndPayment()
        {
            return View();
        }

        //
        // POST: /Checkout/AddressAndPayment

        [HttpPost]
        public ActionResult AddressAndPayment(FormCollection values, string api, string basic, string basic_sdk, string submit)
        {
            var order = new Order();
            TryUpdateModel(order);

            try
            {
                order.Username = User.Identity.Name;
                order.OrderDate = DateTime.Now;

                //Save Order
                storeDB.Orders.Add(order);
                storeDB.SaveChanges();

                //Process the order
                var cart = ShoppingCart.GetCart(this.HttpContext);
                cart.CreateOrder(order);

                if (!string.IsNullOrEmpty(api))
                {
                    //Pre create the order

                    try
                    {
                        var coinorder = new SignedOrder(new CompleteOrder(new Coin.SDK.Order
                                            {
                                                OrderRef = order.OrderId.ToString(),
                                                Amount = order.Total,
                                                Customer = new CustomerByEmail(values["Email"]),
                                                DueDate = HttpContext.Timestamp.AddMonths(1)
                                            }));

                        var client = APIClientFactory.CreateOrderClient();
                        var response = client.PutOrder(coinorder);

                        //Redirect to Cocoin
                        if (response.Status == ResponseStatus.Success)
                            return new RedirectResult(response.CustomerRedirectUrl);
                    }
                    catch (CocoinAPIException e)
                    {
                        throw;
                    }


                    throw new InvalidOperationException("There was an error precreating the order");
                }

                //SDK
                ViewBag.BlankOrder = new Coin.SDK.Order
                                             {
                                                 MerchantID =
                                                     Int32.Parse(
                                                         ConfigurationManager.AppSettings["CocoinMerchantID"]),
                                                 OrderRef = order.OrderId.ToString()
                                             };

                var signedBlankOrder = new SignedOrder(new Coin.SDK.Order
                                                        {
                                                            //MerchantID is collected automatically from AppSettings
                                                            //MerchantID =
                                                            //    Int32.Parse(
                                                            //        ConfigurationManager.AppSettings["CocoinMerchantID"]),
                                                            OrderRef = order.OrderId.ToString()
                                                        });

                signedBlankOrder.Sign();
                ViewBag.SignedBlankOrder = signedBlankOrder;

                ViewBag.SpecifiedOrder = new CompleteOrder(new Coin.SDK.Order
                                                               {
                                                                   MerchantID =
                                                                       Int32.Parse(
                                                                           ConfigurationManager.AppSettings[
                                                                               "CocoinMerchantID"]),
                                                                   OrderRef = order.OrderId.ToString(),
                                                                   Amount = order.Total,
                                                                   Customer = new CustomerByEmail(values["Email"]),
                                                                   DueDate = HttpContext.Timestamp.AddMonths(1)
                                                               });
                

                var signedSpecifiedOrder = new SignedOrder(new CompleteOrder(new Coin.SDK.Order
                                                                                 {
                                                                                     MerchantID =
                                                                                         Int32.Parse(
                                                                                             ConfigurationManager.
                                                                                                 AppSettings[
                                                                                                     "CocoinMerchantID"]),
                                                                                     OrderRef = order.OrderId.ToString(),
                                                                                     Amount = order.Total,
                                                                                     Customer =
                                                                                         new CustomerByEmail(
                                                                                         values["Email"]),
                                                                                     DueDate =
                                                                                         HttpContext.Timestamp.AddMonths
                                                                                         (1)
                                                                                 }));
                signedSpecifiedOrder.Sign(); //Keys collected from web.config/appsettings

                ViewBag.SignedSpecifiedOrder = signedSpecifiedOrder;



                //Manual
                ViewBag.order_ref = order.OrderId.ToString();
                ViewBag.amount = order.Total.ToString();
                ViewBag.email = values["Email"];
                ViewBag.due_date = HttpContext.Timestamp.AddMonths(1).ToShortDateString();

                var unixRef = new DateTime(1970, 1, 1, 0, 0, 0);

                var ts = (HttpContext.Timestamp.Ticks - unixRef.Ticks) / 10000000;
                var nonce = (HttpContext.Timestamp.Ticks ^ order.OrderId ^ new Random().Next()).ToString();

                ViewBag.timestamp = ts;
                ViewBag.nonce = nonce;

                var stringToSign = "amount=" + order.Total.ToString() + "&" +
                                   "consumer_key=" + ConfigurationManager.AppSettings["CocoinConsumer"] + "&" +
                                   "due_date=" + HttpContext.Timestamp.AddMonths(1).ToShortDateString() + "&" +
                                   "email=" + values["Email"] + "&" +
                                   "merchant_id=" + ConfigurationManager.AppSettings["CocoinMerchant"] + "&" +
                                   "nonce=" + nonce + "&" +
                                   "order_ref=" + order.OrderId.ToString() + "&" +
                                   "timestamp=" + ts;

                byte[] keyByte = new ASCIIEncoding().GetBytes(ConfigurationManager.AppSettings["CocoinConsumerSecret"]);

                var hmacsha256 = new HMACSHA256(keyByte);

                ViewBag.signature1 = HttpUtility.UrlEncode(Convert.ToBase64String(hmacsha256.ComputeHash(new UTF8Encoding().GetBytes(stringToSign))));

                stringToSign =
                    "consumer_key=" + ConfigurationManager.AppSettings["CocoinConsumer"] + "&" +
                    "merchant_id=" + ConfigurationManager.AppSettings["CocoinMerchant"] + "&" +
                    "nonce=" + nonce + "&" +
                    "order_ref=" + order.OrderId.ToString() + "&" +
                    "timestamp=" + ts;

                ViewBag.signature2 = HttpUtility.UrlEncode(Convert.ToBase64String(hmacsha256.ComputeHash(new UTF8Encoding().GetBytes(stringToSign))));

                if (!string.IsNullOrEmpty(basic_sdk))
                    return View("PaymentOptionsSDK");

                return View("PaymentOptions");
            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete

        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDB.Orders.Any(
                o => o.OrderId == id &&
                o.Username == User.Identity.Name);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}