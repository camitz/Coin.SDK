using System.Web.Mvc;
using Coin.SDK;
using Coin.SDK.Model;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class CocoinController : Controller
    {
        private MusicStoreEntities storeDB = new MusicStoreEntities();

        [HttpPost]
        public ActionResult Update(OrderUpdateModel update)
        {
            //check order status
            var client = APIClientFactory.CreateOrderClient();
            var orderStatus = client.GetOrder(update.OrderID);

            switch (orderStatus.OrderStatus)
            {
                case OrderStatus.PendingAccept:
                case OrderStatus.PendingSpecification:
                    //we probably know this since we're probably creating it at this instance.
                    break;
                case OrderStatus.Cancelled:
                    //Cancel the order
                    break;

                case OrderStatus.Accepted:
                    break;
                //Other stuff
            }

            switch (orderStatus.PaymentStatus)
            {
                case PaymentStatus.PaidInFull:
                    break;
            }


            return Content("1");
        }
    }
}