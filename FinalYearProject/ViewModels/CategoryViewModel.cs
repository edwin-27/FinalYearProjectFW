using FinalYearProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.ViewModels
{
    public class CategoryViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Code { get; set; }
        [Required]
        public string Name { get; set; }
        public string? ParentCode { get; set; }
        public string? Path { get; set; }
        public ICollection<ProductCatViewModel> Products { get; set; }
        public ICollection<Category> subcategories { get; set; }

        public List<CategoryViewPath> PagePath { get; set; }

        public List<ColourFacet> ColourFacets { get; set; }
        public List<SizeFacet>  SizeFacets { get; set; }
        public PriceFacet PriceFacet { get; set; }

        public Page Page { get; set; }

        public string SearchTerm { get; set; }

        //public List<ColourFacet> AvailableColourFacets { get; set; }
        //public List<SizeFacet> AvailableSizeFacets { get; set; }


        //public string? selected_facetcolour {  get; set; }
        //public string? selected_facetsize { get; set; }

        public CategoryViewModel()
        {
            Products = new List<ProductCatViewModel>();
            subcategories = new List<Category>();
            ColourFacets = new List<ColourFacet>();
            SizeFacets = new List<SizeFacet>();
            PriceFacet = new PriceFacet();
            Page = new Page();

          //  AvailableColourFacets = new List<ColourFacet>();
          //AvailableSizeFacets = new List<SizeFacet>();

        }
    }

    public class CategoryViewPath
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }


    public class ColourFacet
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public bool isAvailable { get; set; }
        public bool isSelected { get; set; }
    }
    public class SizeFacet
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public bool isAvailable { get; set; }
        public bool isSelected { get; set; }
    }

    public class PriceFacet
    {
        public double MinimumPrice { get; set; }
        public double MaximumPrice { get; set; }
    }

    public class Page
    {
        public double Maximum { get; set; }
        public double CurrentPage { get; set; }
    }




}
