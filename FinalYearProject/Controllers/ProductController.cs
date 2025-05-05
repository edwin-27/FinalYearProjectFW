using FinalYearProject.Data;
using FinalYearProject.Libs;
using FinalYearProject.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FinalYearProject.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _environment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }
        public IActionResult Index(int? id, string? colourSelected, string? sizeSelected)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var prodObj = _db.Product.FirstOrDefault(x => x.Id == id.Value);
            //var prodObj = _db.Product.Where(x => x.Id == id.Value).ToList();
            if (prodObj == null)
            {
                return NotFound();
            }


            prodObj.ProductVariants = _db.ProductVariant.Where(v => v.ProductId == id).ToList();

            // Fetch 5 other "Men's Shirts" products (excluding the current one)
            var recommendedShirts = _db.Product
                .Where(p => p.CategoryCode == "MSHIRT" && p.Id != prodObj.Id)
                .Take(5)
                .ToList();

            // Attach variants and images
            foreach (var shirt in recommendedShirts)
            {
                shirt.ProductVariants = _db.ProductVariant.Where(v => v.ProductId == shirt.Id).ToList();

                var colour = shirt.ProductVariants.FirstOrDefault()?.Colour ?? "default";
                var imagePath = Path.Combine(_environment.WebRootPath, "images", $"{shirt.Code}_{colour}.png");
                if (!string.IsNullOrEmpty(shirt.Code) && System.IO.File.Exists(imagePath))
                {
                    shirt.Image = $"/images/{shirt.Code}_{colour}.png";
                }
                else
                {
                    shirt.Image = "/images/placeholder.png"; // fallback
                }
            }

            ViewBag.RecommendedShirts = recommendedShirts;


            //
            List<CategoryViewPath> PagePath = new List<CategoryViewPath>();
            if (_db.Category.FirstOrDefault(c => c.Code == prodObj.CategoryCode) != null)
            {
                ViewBag.PagePath = Util.GetCategoryPath(_db, _db.Category.FirstOrDefault(c => c.Code == prodObj.CategoryCode).Path);
            }



            if (string.IsNullOrEmpty(colourSelected) && prodObj.ProductVariants.Any())
            {
                colourSelected = prodObj.ProductVariants.First().Colour;

            }
            var wwwroot = _environment.WebRootPath;
            string ImageFilePath = wwwroot + @"\images\" + (prodObj.Code ?? "") + @"_" + colourSelected + ".png";

            if ((prodObj.Code ?? "") != "" && System.IO.File.Exists(ImageFilePath))
                prodObj.Image = @"/images/" + (prodObj.Code ?? "") + @"_" + colourSelected + ".png";


            ViewBag.selectedColour = colourSelected;
            ViewBag.selectedSize = sizeSelected;
            return View(prodObj);




        }
    }
}
