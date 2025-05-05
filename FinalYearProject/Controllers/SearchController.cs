using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;
using FinalYearProject.ViewModels;
using FinalYearProject.Libs;

namespace FinalYearProject.Controllers
{
    public class SearchController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IOpenSearchClient _client;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        private int pageNumber = 0;
        private int pageSize = 4;

        public SearchController(ILogger<HomeController> logger, IOpenSearchClient client, ApplicationDbContext db, IConfiguration configuration)
        {
            _logger = logger;
            _client = client;
            _db = db;
            _configuration = configuration;

        }


        // GET: SearchController
        public async Task<IActionResult> Index(string searchterm, string? facetcolour, string? facetsize, int? page = 1)
        {

            string searchtermAI = "";
            AzureAI myazure = new AzureAI();
            searchtermAI = myazure.GetKeywords(searchterm);

            ProductSearchService searchService = new ProductSearchService(_client, _db, _configuration);
            List<AWSProduct> results = await searchService.SearchProductsAsync(searchterm);


            SearchViewModel viewModel = new SearchViewModel();
            viewModel.Products = results;
            viewModel.SearchTerm = searchtermAI;

            ViewBag.SearchKeyword = searchtermAI;

            CategoryViewModel categoryview = new CategoryViewModel();




            categoryview.Id = 0;
            categoryview.Code = "search";
            categoryview.Name = "Search";
            categoryview.PagePath = new List<CategoryViewPath> { new CategoryViewPath { Code = "Search", Name = "Search" } };

            var search_products = results;

            int[] product_ids = search_products.Select(s => s.Id).ToArray();


            var products = _db.Product.Where(p => product_ids.Contains(p.Id)).ToList();

            if (products != null)
            {

                foreach (var product in products)
                {
                    var Variants = _db.ProductVariant.Where(p => p.ProductId == product.Id).ToList();
                    product.ProductVariants = Variants;
                }

                //var Variants = _db.ProductVariant.Where(p => p.ProductId == product.Id).ToList();


                var distinctColours = products.SelectMany(p => p.ProductVariants).Select(vw => vw.Colour ?? "").Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

                var distinctSizes = products.SelectMany(p => p.ProductVariants).Select(vw => vw.Size).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

                foreach (var colour in distinctColours)
                {
                    categoryview.ColourFacets.Add(new ColourFacet { Code = colour, Name = colour, isAvailable = false, isSelected = false });
                }


                foreach (var size in distinctSizes)
                {
                    categoryview.SizeFacets.Add(new SizeFacet { Code = size, Name = size, isAvailable = false, isSelected = false });
                }



                List<Product> products1 = new List<Product>();
                List<Product> filtered_products = new List<Product>();
                filtered_products = products.ToList();
                if (facetcolour != null)
                {
                    //categoryview.selected_facetcolour = facetcolour;
                    //categoryview.selected_facetcolour = "#" + string.Join("#", facetcolour.Split(",", StringSplitOptions.None).ToArray()) + "#";

                    foreach (var colour in facetcolour.Split(",", StringSplitOptions.None))
                    {
                        var ofacet = categoryview.ColourFacets.Where(f => f.Code == colour).First();
                        ofacet.isSelected = true;
                        ofacet.isAvailable = true;
                    }

                    for (int i = filtered_products.Count - 1; i >= 0; i--)
                    {
                        var bFound = false;
                        var prod = filtered_products[i];


                        foreach (var colour in facetcolour.Split(",", StringSplitOptions.None))
                        {
                            var prod2 = prod.ProductVariants.Where(v => v.Colour == colour.Trim()).FirstOrDefault();
                            if (prod2 != null)
                            {
                                bFound = true;
                                break;
                            }
                        }
                        if (bFound)
                        {
                        }
                        else
                        {
                            filtered_products.Remove(prod);
                        }
                    }
                }

                if (facetsize != null)
                {
                    //if (products1.Count > 0)
                    //  filtered_products = products1.ToList();

                    //categoryview.selected_facetsize = facetsize;
                    //categoryview.selected_facetsize = "#" + string.Join("#", facetsize.Split(",", StringSplitOptions.None).ToArray()) + "#";

                    foreach (var size in facetsize.Split(",", StringSplitOptions.None))
                    {
                        var ofacet = categoryview.SizeFacets.Where(f => f.Code == size).First();
                        ofacet.isSelected = true;
                        ofacet.isAvailable = true;

                    }


                    for (int i = filtered_products.Count - 1; i >= 0; i--)
                    {
                        var bFound = false;
                        var prod = filtered_products[i];
                        foreach (var size in facetsize.Split(",", StringSplitOptions.None))
                        {
                            var prod2 = prod.ProductVariants.Where(v => v.Size == size.Trim()).FirstOrDefault();

                            if (prod2 != null)
                            {
                                bFound = true;
                                break;
                            }
                        }
                        if (bFound)
                        {
                            //if (products1.Where(p => p.Id == prod.Id).FirstOrDefault() == null)
                            //    products1.Add(prod);
                        }
                        else
                        {
                            filtered_products.Remove(prod);
                        }

                    }
                }

                if (filtered_products.Count > 0)
                {
                    products = filtered_products;
                }

                if ((facetcolour ?? "") == "" && (facetsize ?? "") != "")
                {
                    foreach (var facet in categoryview.SizeFacets)
                    {
                        //   facet.isSelected = true;
                        facet.isAvailable = true;
                    }
                }
                else if ((facetcolour ?? "") != "" && (facetsize ?? "") == "")
                {
                    foreach (var facet in categoryview.ColourFacets)
                    {
                        // facet.isSelected = true;
                        facet.isAvailable = true;
                    }
                }
                else if ((facetcolour ?? "") != "" && (facetsize ?? "") != "")
                {
                    //Do nothing. Factets have been picked up already
                }
                else
                {
                    foreach (var facet in categoryview.ColourFacets)
                    {
                        facet.isSelected = false;
                        facet.isAvailable = true;
                    }
                    foreach (var facet in categoryview.SizeFacets)
                    {
                        facet.isSelected = false;
                        facet.isAvailable = true;
                    }


                }



                #region get updated facet



                var available_distinctColours = products.SelectMany(p => p.ProductVariants).Select(vw => vw.Colour ?? "").Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

                var available_distinctSizes = products.SelectMany(p => p.ProductVariants).Select(vw => vw.Size).Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();

                foreach (var colour in available_distinctColours)
                {
                    categoryview.ColourFacets.Where(f => f.Code == colour).First().isAvailable = true;
                }


                foreach (var size in available_distinctSizes)
                {
                    categoryview.SizeFacets.Where(f => f.Code == size).First().isAvailable = true;
                }

                #endregion



                //if (products.Count > 0)
                //{
                //    categoryview.PriceFacet.MinimumPrice = (double)products.SelectMany(p => p.ProductVariants).Min(v => v.Price);
                //    categoryview.PriceFacet.MaximumPrice = (double)products.SelectMany(p => p.ProductVariants).Max(v => v.Price);
                //}

                categoryview.Page.Maximum = Convert.ToDouble(Math.Ceiling(products.Count() / (double)pageSize));

                products = products.Skip(((int)page - 1) * pageSize).Take(pageSize).ToList();

                foreach (var product in products)
                {
                    //var variant = product.ProductVariants.FirstOrDefault();
                    var variant = _db.ProductVariant.Where(p => p.ProductId == product.Id).FirstOrDefault();
                    var Variants = _db.ProductVariant.Where(p => p.ProductId == product.Id).ToList();


                    ProductCatViewModel CatProd = new ProductCatViewModel { Id = product.Id, Name = product.Name, Code = product.Code, Image = product.Image, Variants = new List<ProductCatVariant>() };

                    ProductCatVariant CatVar = new ProductCatVariant { Id = variant.Id, SKU = variant.SKU, Colour = variant.Colour, Size = variant.SKU, Price = variant.Price };


                    CatProd.MainVariant = CatVar;
                    foreach (var oVar in Variants)
                    {
                        CatProd.Variants.Add(new ProductCatVariant() { Id = variant.Id, SKU = variant.SKU, Colour = variant.Colour, Size = variant.Size, Price = variant.Price });
                    }
                    categoryview.Products.Add(CatProd);
                }



            }




            return View(categoryview);
        }

        // GET: SearchController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SearchController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SearchController/Create
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

        // GET: SearchController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SearchController/Edit/5
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

        // GET: SearchController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SearchController/Delete/5
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
