using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OpenSearch.Client;
using System.Drawing;
using System.Text;
using FinalYearProject.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace FinalYearProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        // GET: AdminController

        private readonly ILogger<HomeController> _logger;
        private readonly IOpenSearchClient _client;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public AdminController(ILogger<HomeController> logger, IOpenSearchClient client, ApplicationDbContext db, IConfiguration configuration)
        {
            _logger = logger;
            _client = client;
            _db = db;
            _configuration = configuration;

        }
        



        public async Task<IActionResult> Index(string id)
        {
            if (id== "IndexAWS")
            {
                ProductSearchService searchService = new ProductSearchService(_client, _db, _configuration);
                //var results = await searchService.SearchProductsAsync(q);
                await searchService.IndexAllProductAsync();

                
               
            }
            var orders = _db.Orders
                    .Select(o => new AdminOrderViewModel
                    {
                        Id = o.Id,
                        DeliveryOption = o.DeliveryOption,
                        BillingAddressId = o.BillingAddressId,
                        CustomerId = o.CustomerId,
                        DeliveryAddressId = o.DeliveryAddressId,
                        DeliveryCost = o.DeliveryCost,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        DespatchDate = o.despatch_date,
                        InvEmail = o.inv_email,
                        OrderStatus = o.order_status,
                        TrackingUrl = o.tracking_url
                    }).ToList();

            return View(orders);


        }

        public ActionResult ChatBase()
        {
            /*
             * 
https://localhost:7000/nike-air-max-90-trainer-black-smoke-grey-iron-grey-4108141.html: Nike - Air Max 90 Trainer
    <h1>
        Nike
        Air Max 90 Trainer
        black / smoke grey / iron grey

        £135.00
    </h1>
    <h3>
        Product Description
        Product Code: 4108141
        Nike Air Max 90 Trainers in Black, Smoke Grey and Iron Grey. Originally designed for performance running, feel the purrrr of technical brilliance slot seamlessly into your everyday casual wear. Epitomising the 1990’s world of art, music and culture, the striking aesthetics combine with a grippy Waffle outsole and visible lightweight Air cushioning to leave you walking on a cloud. Air technology first debuted in 1978, with its visible cousin later appearing in 1987 in the legendary Air Max 1’s.

        Fabric: Suede and Textile upper, Textile lining and Other Materials Sole.

        Product Care: To maintain appearance we recommend protecting with a suitable leather protector.

        Style: Running.
    </h3>



             * 
             * 
             * */

            

                var products = _db.Product.ToList();

            StringBuilder oStr = new StringBuilder();

            foreach (var product in products)
            {
                var Variants = _db.ProductVariant.Where(p => p.ProductId == product.Id).ToList();
                product.ProductVariants = Variants;
                string Name = product.Name;
                string Description = product.Description;
                string Colours = string.Join(" ",product.ProductVariants.Select(v => v.Colour).Distinct().ToArray());
                string[] Sizes = product.ProductVariants.Select(v => v.Size).Distinct().ToArray();
                string Sizes2 = "";
                foreach (var size1 in Sizes)
                {
                    switch (size1.ToUpper())
                    {
                        case "XS":
                            Sizes2 += "XS - Small : ";
                            break;
                        case "S":
                            Sizes2 += "S - Small : ";
                            break;
                        case "M":
                            Sizes2 += "M - Medium : ";
                            break;
                        case "L":
                            Sizes2 += "L - large : ";
                            break;
                        case "XL":
                            Sizes2 += "XL - Extra large XLarge : ";
                            break;
                        case "XXL":
                            Sizes2 += "XXL - Extra large XXLarge : ";
                            break;
                    }
                    
                }
                decimal minPrice = product.ProductVariants.Min(v=>v.Price);
                decimal maxPrice = product.ProductVariants.Max(v => v.Price);
                string url = "https://" + Request.Host.ToString() + Url.Action("Index", "Product", new { id = product.Id });


                string sProdTemplate = $"{url}: {Name} <h1>{Name}\r\n Available colours {Colours}\r\n Available sizes {Sizes2}\r\n Price range £{minPrice} £{maxPrice}</h1>\r\n <h2>Product Description \r\n {Description} \r\n  \r\n ";

                oStr.AppendLine(sProdTemplate);
            }

            


            return Ok(oStr.ToString());
        }


        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


    
    }
}
