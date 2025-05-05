using FinalYearProject.Data;
using FinalYearProject.Libs;
using FinalYearProject.Models;
using FinalYearProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Linq;

namespace FinalYearProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int pageNumber = 0;
        private int pageSize = 4;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult List()
        {
            // Fetch all categories and pass them to the view
            List<Category> categories = _db.Category.ToList();
            return View(categories);
        }

        public IActionResult Index(string? id, string? selectedCategoryCode, string? facetcolour, string? facetsize, int? page = 1)
        {
            // Fetch all categories
            var categories = _db.Category.ToList();
            CategoryViewModel categoryview = new CategoryViewModel();

            // Add products for each category
            //            foreach (var category in categories)
            var category = categories.Where(c => c.Code == id).FirstOrDefault();
            if (category != null)
            {
                categoryview.Id = category.Id;
                categoryview.Code = category.Code;
                categoryview.Name = category.Name;
                categoryview.PagePath = Util.GetCategoryPath(_db, category.Path);
                var products = _db.Product.Where(p => p.CategoryCode == category.Code).ToList();

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
                categoryview.subcategories = categories.Where(a => a.ParentCode == category.Code).ToList();




            }

            // Set ViewBag for the selected category
            //ViewBag.SelectedCategoryCode = selectedCategoryCode;

            // Return all categories to the view
            return View(categoryview);
        }


    }
}
