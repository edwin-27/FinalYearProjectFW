using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Models.Data_Transfer_Objects;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace FinalYearProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BasketController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult AddToBasket(int productVariantId, string colourSelected, string sizeSelected)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _db.Customers.FirstOrDefault(c => c.UserId == userId);

            var prodVariant = _db.ProductVariant
                .Include(pv => pv.Product)
                .FirstOrDefault(pv => pv.Id == productVariantId);

            if (prodVariant == null)
                return NotFound();

            if (userId != null && customer != null)
            {
                var customerBasket = _db.Baskets
                    .Include(b => b.BasketItems)
                    .FirstOrDefault(b => b.CustomerId == customer.Id);

                if (customerBasket == null)
                {
                    customerBasket = new Basket
                    {
                        CustomerId = customer.Id,
                        BasketItems = new List<BasketItem>()
                    };
                    _db.Baskets.Add(customerBasket);
                    _db.SaveChanges();
                }

                var existingItem = customerBasket.BasketItems
                    .FirstOrDefault(bi => bi.ProductVariantId == productVariantId);

                if (existingItem != null)
                    existingItem.Quantity++;
                else
                    _db.BasketItems.Add(new BasketItem
                    {
                        BasketId = customerBasket.Id,
                        ProductVariantId = productVariantId,
                        Quantity = 1
                    });

                _db.SaveChanges();
            }

            TempData["Notice"] = "Item added to basket.";
            return RedirectToAction("Index", "Product", new {id = prodVariant.Product.Id, colourSelected = prodVariant.Colour, sizeSelected = prodVariant.Size});
        }


        [AllowAnonymous]
        public IActionResult Index(int? id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _db.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
                //return View(new FinalYearProject.Models.Data_Transfer_Objects.Order());
            }

            int customerId = customer.Id;

            var customerBasket = _db.Baskets
                .Include(ba => ba.BasketItems)
                    .ThenInclude(bi => bi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefault(ba => ba.CustomerId == customerId);

            if (customerBasket == null || !customerBasket.BasketItems.Any())
            {
                return View(new FinalYearProject.Models.Data_Transfer_Objects.Order()); // FinalYearProject.Models.Data_Transfer_Objects.Order
            }

            var orderItems = customerBasket.BasketItems.Select(product => new FinalYearProject.Models.Data_Transfer_Objects.OrderItem
            {
                productName = product.ProductVariant.Product.Name,
                colour = product.ProductVariant.Colour,
                size = product.ProductVariant.Size,
                price = product.ProductVariant.Price,
                quantity = product.Quantity,
                imgUrl = $"/images/{product.ProductVariant.Product.Code}_{product.ProductVariant.Colour}.png",
                basketItemId = product.Id
            }).ToList();

            var basketOrder = new FinalYearProject.Models.Data_Transfer_Objects.Order
            {
                items = orderItems,
                subTotalofProducts = orderItems.Sum(it => it.price * it.quantity)
            };

            return View(basketOrder);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult DeleteItem(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = _db.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int customerId = customer.Id;

            var item = _db.BasketItems
                .Include(bi => bi.Basket)
                .FirstOrDefault(bi => bi.Id == id && bi.Basket.CustomerId == customerId);

            if (item != null)
            {
                _db.BasketItems.Remove(item);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
