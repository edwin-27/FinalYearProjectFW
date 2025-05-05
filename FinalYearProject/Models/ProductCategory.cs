using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("Category")]
        public int CategoryCode { get; set; }

        public Product Product { get; set; }
        public Category Category { get; set; }


    }
}
