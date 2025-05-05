using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalYearProject.Data;
using FinalYearProject.Models;

namespace Proj.Controllers
{
    public class ordersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ordersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: orders/Details/5

        public async Task<IActionResult> GetStatus(string email_id, string order_id)
        {
            if ( ( (email_id ??"") == "" ) || ((order_id ?? "") == ""))
            {
                return NotFound();
            }

            var order = await _context.Orders.Where(o=> o.Id == Convert.ToInt32(order_id) && o.inv_email == email_id).FirstOrDefaultAsync();
            if (order == null)
            {
                return Json(new { data = string.Format("Order not found"), status = "error" });
            }
            else if ((order.order_status ?? "").ToUpper() == "DESPATCHED" && ((order.tracking_url ?? "") !="" ))
            {
                return Json(new { data = string.Format(@"Your order {0} has been despatched on {1}. You can track it here <a href=""{2}"">{2}</a> ", order.Id, order.despatch_date.Value.ToString("dd/MM/yyyy"), order.tracking_url), status = "success" });
            }
            else if ((order.order_status ?? "").ToUpper() == "DESPATCHED")
            {
                return Json(new { data = string.Format("Your order {0} has been despatched on {1}", order.Id , order.despatch_date.Value.ToString("dd/MM/yyyy")), status = "success" });
            }
            else if ((order.order_status ?? "").ToUpper() == "CANCELLED")
                return Json(new { data = string.Format("Your order {0} has been cancelled", order.Id), status = "success" });
            else
                return Json(new { data = string.Format("Your order {0} is being processed", order.Id), status = "success" });



        }


     
    }
}
