using FinalYearProject.Data;
using FinalYearProject.Models;
using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;
using System.Diagnostics;

namespace FinalYearProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOpenSearchClient _client;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        //private readonly ProductSearchService _searchService;

        public HomeController(ILogger<HomeController> logger, IOpenSearchClient client, ApplicationDbContext db, IConfiguration configuration)
        {
            _logger = logger;
            _client = client;
            _db = db;
            _configuration = configuration;

        }

        public IActionResult Index()
        {
            //HttpContext.Session.SetInt32("CustomerId", 1);


            //AzureAI myazure = new AzureAI();
            //myazure.GetKeywords();


            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
