using FinalYearProject.Models.Data_Transfer_Objects;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FinalYearProject.Models
{
    public class Customer
    {

        public int Id { get; set; }
        [Required]

        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email {  get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}
