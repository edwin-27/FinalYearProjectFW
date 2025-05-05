using FinalYearProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.ViewModels
{
    public class ProductCatViewModel
    {
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public ProductCatVariant MainVariant { get; set; }
        public List<ProductCatVariant> Variants { get; set; }
    }


    public class ProductCatVariant
    {

        public int Id { get; set; }

        public int ProductId { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
    }
}
