using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal LineTotal { get; set; }

        
        public Order Order { get; set; }
    }
}
