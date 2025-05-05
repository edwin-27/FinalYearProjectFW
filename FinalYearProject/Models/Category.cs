using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalYearProject.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public String Code {  get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("ParentCategory")]
        public string? ParentCode { get; set; }

        [StringLength(50)]
        public string? Path { get; set; }

        public Category ParentCategory { get; set; }

        public ICollection<Product> Products { get; set; }

       
    }
}
