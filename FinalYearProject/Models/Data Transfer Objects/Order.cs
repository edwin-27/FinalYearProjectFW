namespace FinalYearProject.Models.Data_Transfer_Objects
{
    public class Order
    {
        public List<OrderItem> items { get; set; } = new List<OrderItem>();
        public decimal subTotalofProducts {  get; set; }

        public string? deliveryLabel { get; set; }
        public decimal? deliveryPrice { get; set; }
        public decimal? finalPrice => subTotalofProducts + deliveryPrice;
        
        public Dictionary<string, List<OrderItem>> GroupedByProduct()
        {

            return items.GroupBy(product => product.productName).ToDictionary(di => di.Key, di => di.ToList());


        }

    }
}
