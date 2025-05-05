using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class BasketItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Basket")]
        public int BasketId { get; set; }
        public Basket Basket { get; set; }

        [ForeignKey("ProductVariant")]
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }

        public int Quantity {  get; set; }
        
    }
}
