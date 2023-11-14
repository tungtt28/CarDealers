using CarDealers.Entity;
using CarDealers.Models;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace CarDealers.Controllers
{
    public class ViewOrderController : Controller
    {
        CarDealersContext _context = new CarDealersContext();
        public IActionResult ViewOrder()
        {
            
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

            if (customerId != null)
            {
                var orders = _context.Orders.Where(e => e.CustomerId.ToString() == customerId && e.DeleteFlag == false)
                    .Include(x => x.OrderDetails).Include(x => x.OrderAccessoryDetails).Include(x => x.Customer)
                    .OrderBy(p => p.OrderId).ToList();
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return View(orders);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View();
        }


        [HttpGet]
        public ActionResult ViewDetails(int id)
        {
            if (ModelState.IsValid)
            {
                var orderCarDetails = _context.OrderDetails.Where(x => x.OrderId == id && x.DeleteFlag == false)
                    .Include(x => x.Car)
                    .Include(x => x.Seller)
                    .Include(x => x.Order)
                    .OrderBy(p => p.Car.Model).ToList();
                var orderAssDetails = _context.OrderAccessoryDetails.Where(x => x.OrderId == id && x.DeleteFlag == false)
                    .Include(x => x.Accessory)
                    .Include(x => x.Seller)
                    .Include(x => x.Order)
                    .ToList();
                var listViewModel = new List<OrderDetialsViewModel>();

                
                if (orderCarDetails.Count() != 0)
                {
                    listViewModel.AddRange(orderCarDetails.Select(x => new OrderDetialsViewModel
                    {
                        OrderId = x.OrderId,
                        ProductName = x.Car.Model,
                        SellerName = x.SellerId.HasValue ? x.Seller.FullName : "",
                        Quantity =x.Quantity,
                        TotalPrice = x.TotalPrice,
                    }));
                }
                else if (orderAssDetails.Count() != 0)
                {
                    listViewModel.AddRange(orderAssDetails.Select(x => new OrderDetialsViewModel
                    {
                        OrderId = x.OrderId,
                        ProductName = x.Accessory.AccessoryName,
                        SellerName = x.SellerId.HasValue ? x.Seller.FullName : "",
                        Quantity = x.Quantity,
                        TotalPrice = x.TotalPrice,
                    }));
                }
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return View(listViewModel);

            }
            return RedirectToAction("RecordNotFound");
        }

        protected List<News> GetDefaultFooterText()
        {
            var footers = _context.News.Include(e => e.NewsType).Where(e => e.DeleteFlag == false && e.NewsType.NewsTypeName.Equals(Constant.FOOTER)).OrderBy(e => e.Order).ToList();
            return footers;
        }
        protected List<News> GetDefaultMenu()
        {
            var menus = _context.News.Include(e => e.NewsType).Where(e => e.DeleteFlag == false && e.NewsType.NewsTypeName.Equals(Constant.MENU)).OrderBy(e => e.Order).ToList();
            return menus;
        }
    }
}
