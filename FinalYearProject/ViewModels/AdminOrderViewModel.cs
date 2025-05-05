namespace FinalYearProject.ViewModels
{
    public class AdminOrderViewModel
    {
        public int Id { get; set; }
        public string DeliveryOption { get; set; }
        public int? BillingAddressId { get; set; }
        public int CustomerId { get; set; }
        public int? DeliveryAddressId { get; set; }
        public decimal DeliveryCost { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? DespatchDate { get; set; }
        public string InvEmail { get; set; }
        public string OrderStatus { get; set; }
        public string TrackingUrl { get; set; }

    }
}
