using FinalYearProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        [StringLength(100)]
        

        [Column(TypeName = "decimal(10, 2)")]
        public decimal DeliveryCost { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        public int? DeliveryAddressId { get; set; }
        public int? BillingAddressId { get; set; }

        public Address DeliveryAddress { get; set; }
        public Address BillingAddress { get; set; }

        public string DeliveryOption {  get; set; }

        public string order_status { get; set; }
        public DateTime? despatch_date { get; set; }
        public string tracking_url { get; set; }

        public string inv_email { get; set; }



        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
