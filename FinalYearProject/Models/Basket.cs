using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class Basket
    {

        [Key]
        public int Id {  get; set; }


        
        public int CustomerId {  get; set; }
        

        public ICollection<BasketItem> BasketItems { get; set; }

        //public int deliveryAddId {  get; set; }
        //public int billingAddId { get; set; }
    }
}
