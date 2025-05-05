using FinalYearProject.Models;
using FinalYearProject.Models.Data_Transfer_Objects;
using System.ComponentModel.DataAnnotations;
namespace FinalYearProject.ViewModels
{
    public class CheckoutViewModel
    {

        public FinalYearProject.Models.Data_Transfer_Objects.Order basketSummary { get; set; }
        public List<BasketItem> basketItems {  get; set; }
        [Required(ErrorMessage="Delivery address is required" )]
        public int? chosenDeliveryAddressId { get; set; }
        [Required(ErrorMessage = "Billing address is required")]
        public int? chosenBillingAddressId { get; set; }
        [Required(ErrorMessage = "Delivery option is required")]

        public int? selectedDeliveryOptionId { get; set; }

       
    }
}
