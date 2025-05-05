using FinalYearProject.Data;
using FinalYearProject.Models;
using FinalYearProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalYearProject.Controllers
{
    public class AddressController : Controller
    {

        private readonly ApplicationDbContext _db;

        public AddressController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddAddress()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddAddress(AddressViewModel addressViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addressViewModel);
            }

            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            var customer = _db.Customers.FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                
                return RedirectToAction("Index", "Home");
            }

            
            var customerAddresses = new Address
            {
                CustomerId = customer.Id,
                firstName = addressViewModel.firstName,
                lastName = addressViewModel.lastName,
                line1 = addressViewModel.line1,
                line2 = addressViewModel.line2,
                townOrCity = addressViewModel.townOrCity,
                postcode = addressViewModel.postcode
            };

            _db.Addresses.Add(customerAddresses);
            _db.SaveChanges();

            return RedirectToAction("Index", "Checkout");
        }



    }
}
