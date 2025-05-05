using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]

        [MaxLength(50)]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; }
        public decimal AvgRating { get; set; }
        public int NumOfReviews {  get; set; }
        [ForeignKey("Category")]
        public string? CategoryCode {  get; set; }
        public Category Category { get; set; }

        public ICollection<ProductVariant> ProductVariants { get; set; }
        public ICollection<ProductCategory> ProductCategories { get; set; }

    }
}
