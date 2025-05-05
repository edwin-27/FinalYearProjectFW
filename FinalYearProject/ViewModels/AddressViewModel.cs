using System.ComponentModel.DataAnnotations;
namespace FinalYearProject.ViewModels
{
    public class AddressViewModel
    {

        public int customerId { get; set; }
        [Display(Name = "First Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }
        [Display(Name = "Address Line1")]
        public string line1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string? line2 { get; set; }
        [Display(Name = "Town or City")]
        public string townOrCity { get; set; }
        [Display(Name = "Postcode")]
        public string postcode { get; set; }
    }
}
