using FinalYearProject.Data;
using FinalYearProject.Models.Data_Transfer_Objects;
using FinalYearProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenSearch.Client;
using System.Security.Claims;


namespace FinalYearProject.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {

        private readonly ApplicationDbContext _db;
        public CheckoutController(ApplicationDbContext db)
        {
            _db =db;
        }


        
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _db.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int customerId = customer.Id;

            var customerBasket = _db.Baskets
                .Include(ba => ba.BasketItems)
                    .ThenInclude(bi => bi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefault(ba => ba.CustomerId == customerId);

            if (customerBasket == null || !customerBasket.BasketItems.Any())
            {
                return RedirectToAction("Index", "Basket");
            }

            var customerAddresses = _db.Addresses.Where(a => a.CustomerId == customerId).ToList();
            var deliveryChoice = _db.Deliveries.ToList();

            int? chosenDeliveryOptionId = HttpContext.Session.GetInt32("SelectedDeliveryChoiceId");
            var chosenDelivery = deliveryChoice.FirstOrDefault(d => d.Id == chosenDeliveryOptionId);
            decimal? deliveryPrice = chosenDelivery?.price;
            string deliveryLabel = chosenDelivery?.option ?? "Delivery option not selected";

            int? chosenDeliveryId = HttpContext.Session.GetInt32("DeliveryAddressId");
            int? chosenBillingId = HttpContext.Session.GetInt32("BillingAddressId");

            var orderedItems = customerBasket.BasketItems.Select(product => new OrderItem
            {
                productName = product.ProductVariant.Product.Name,
                colour = product.ProductVariant.Colour,
                size = product.ProductVariant.Size,
                price = product.ProductVariant.Price,
                quantity = product.Quantity
            }).ToList();

            var cvm = new CheckoutViewModel
            {
                basketSummary = new Order
                {
                    items = orderedItems,
                    subTotalofProducts = orderedItems.Sum(a => a.price * a.quantity),
                    deliveryPrice = deliveryPrice,
                    deliveryLabel = deliveryLabel,
                },
                basketItems = customerBasket.BasketItems.ToList(),
                chosenDeliveryAddressId = chosenDeliveryId,
                chosenBillingAddressId = chosenBillingId,
            };

            ViewBag.Addresses = customerAddresses;
            ViewBag.DeliveryChoice = deliveryChoice;

            return View(cvm);
        }



        [HttpPost]
        public IActionResult saveChosenDeliveryAddress(int id)
        {
            HttpContext.Session.SetInt32("DeliveryAddressId", id);
            return RedirectToAction("Index");
        }
        


        public IActionResult saveChosenBillingAddress(int id)
        {
            HttpContext.Session.SetInt32("BillingAddressId", id);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult saveDeliveryChoice(int id)
        {
            HttpContext.Session.SetInt32("SelectedDeliveryChoiceId", id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult confirmOrder(CheckoutViewModel cvm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _db.Customers.FirstOrDefault(c => c.UserId == userId);

            int? deliveryAddressId = HttpContext.Session.GetInt32("DeliveryAddressId");
            int? billingAddressId = HttpContext.Session.GetInt32("BillingAddressId");
            int? deliveryChoiceId = HttpContext.Session.GetInt32("SelectedDeliveryChoiceId");

            if (customer == null || deliveryAddressId == null || billingAddressId == null || deliveryChoiceId == null)
            {
                TempData["Error"] = "Please complete all checkout steps.";
                return RedirectToAction("Index");
            }

            int customerId = customer.Id;

            var deliveryOption = _db.Deliveries.FirstOrDefault(d => d.Id == deliveryChoiceId);
            var basket = _db.Baskets
                .Include(b => b.BasketItems)
                    .ThenInclude(bi => bi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefault(b => b.CustomerId == customerId);

            if (basket == null || !basket.BasketItems.Any())
            {
                TempData["Error"] = "Your basket is empty.";
                return RedirectToAction("Index", "Basket");
            }

            decimal subtotal = basket.BasketItems.Sum(item => item.Quantity * item.ProductVariant.Price);
            decimal deliveryCost = deliveryOption?.price ?? 0;
            decimal total = subtotal + deliveryCost;

            
            var userEmail = _db.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefault();

            var order = new FinalYearProject.Models.Order
            {
                CustomerId = customerId,
                inv_email = userEmail,
                order_status = "New",
                tracking_url = "",
                OrderDate = DateTime.Now,
                DeliveryAddressId = deliveryAddressId,
                BillingAddressId = billingAddressId,
                DeliveryOption = deliveryOption?.option ?? "Not specified",
                DeliveryCost = deliveryCost,
                TotalAmount = total
            };

            _db.Orders.Add(order);
            _db.SaveChanges();

            foreach (var item in basket.BasketItems)
            {
                var orderItem = new FinalYearProject.Models.OrderItem
                {
                    OrderId = order.Id,
                    ProductName = item.ProductVariant.Product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.ProductVariant.Price,
                    LineTotal = item.Quantity * item.ProductVariant.Price
                };

                _db.OrderItems.Add(orderItem);
            }

            _db.SaveChanges();

            _db.BasketItems.RemoveRange(basket.BasketItems);
            _db.Baskets.Remove(basket);
            _db.SaveChanges();

            order = _db.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.BillingAddress)
                .FirstOrDefault(o => o.Id == order.Id);

            return View("Confirmation", order);
        }


        [HttpPost]
        public IActionResult ClearBasketAndReturn()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _db.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer != null)
            {
                var basket = _db.Baskets
                    .Include(b => b.BasketItems)
                    .FirstOrDefault(b => b.CustomerId == customer.Id);

                if (basket != null)
                {
                    _db.BasketItems.RemoveRange(basket.BasketItems);
                    _db.Baskets.Remove(basket);
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Home");
        }




    }




}
