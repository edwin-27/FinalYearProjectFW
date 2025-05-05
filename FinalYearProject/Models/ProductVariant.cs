using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class ProductVariant
    {

        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public string Colour {  get; set; }
        public string ColourCode { get; set; }
        public string Size {  get; set; }
        public decimal Price {  get; set; }
        public Product Product { get; set; }

        public string SKU { get; set; }
    }
}
