using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class HomeController : CustomController
    {
        private readonly CarDealersContext _context;

        public HomeController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }
        public IActionResult AdminHomePage()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }

            //get total profit
            var totalProfitCars = _context.OrderDetails.Sum(o => o.TotalPrice ?? 0);
            var totalProfitAccessories = _context.OrderAccessoryDetails.Sum(o => o.TotalPrice ?? 0);
            ViewBag.totalProfit = totalProfitCars + totalProfitAccessories;

            //get total customer
            var totalCustomers = _context.Customers.Count();
            ViewBag.totalCustomers = totalCustomers;

            //get total vehicles
            var totalCars = _context.Cars.Count();
            ViewBag.totalCars = totalCars;

            //get total order
            var totalOrderCars = _context.OrderDetails.Count();
            var totalOrderAccessories = _context.OrderAccessoryDetails.Count();
            ViewBag.totalOrders = totalOrderCars + totalOrderAccessories;

            return View();
        }
    }
}
