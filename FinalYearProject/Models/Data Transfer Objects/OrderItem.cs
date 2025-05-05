namespace FinalYearProject.Models.Data_Transfer_Objects
{
    public class OrderItem
    {
       
        public string productName { get; set; }
        public string colour {  get; set; }
        public string size {  get; set; }
        public decimal price { get; set; }
        public int quantity {  get; set; }

        public decimal total => price * quantity;
        public string imgUrl {  get; set; }

        public int basketItemId {  get; set; }

    }
}
