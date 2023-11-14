using CarDealers.Entity;
using CarDealers.Models;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace CarDealers.Controllers
{
    public class HomeController : CustomController
    {
        private readonly CarDealersContext _context;

        public HomeController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE, Constant.GUEST_ROLE };
        }

        public IActionResult Index()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            //var bestSellingCarList = _context.OrderDetails
            //    .GroupBy(o => o.CarId)
            //    .Select(group => new
            //    {
            //        CarId = group.Key,
            //        OrderDetailCount = group.Count() // Đếm số lượng OrderDetails trong mỗi nhóm
            //    })
            //    .OrderByDescending(x => x.OrderDetailCount)
            //    .Select(x => x.CarId)
            //    .Take(10);
            //// Lấy danh sách thông tin Description, Image, và ExportPrice cho các CarId trong bestSellingCarList
            //var topCars = _context.Cars
            //    .Where(c => bestSellingCarList.Contains(c.CarId))
            //    .Select(c => new
            //    {
            //        c.Image,
            //        c.Description,
            //        c.ExportPrice
            //    })
            //    .ToList();

            List<Car> topCars = _context.Cars.Where(x => x.DeleteFlag == false).ToList();

            ViewBag.topCars = topCars;
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public ActionResult Register(Customer customer)
        {
            using (CarDealersContext context = new CarDealersContext())
                try
                {
                    bool isError = false;

                    // Check if Full Name is null or empty
                    if (string.IsNullOrEmpty(customer.FullName))
                    {
                        TempData["ErrorMessageFullName"] = "Full Name is required.";
                        isError = true;
                    }

                    // Check if Email is null or empty
                    if (string.IsNullOrEmpty(customer.Email))
                    {
                        TempData["ErrorMessageEmail"] = "Email is required.";
                        isError = true;
                    }
                    else if (!Validation.CheckEmail(customer.Email))
                    {
                        TempData["ErrorMessageEmail"] = "Email must be in the correct format";
                        isError = true;
                    }

                    // Check if Phone Number is null or empty
                    if (string.IsNullOrEmpty(customer.PhoneNumber))
                    {
                        TempData["ErrorMessagePhoneNumber"] = "Phone Number is required.";
                        isError = true;
                    }
                    else if (!Validation.CheckPhone(customer.PhoneNumber))
                    {
                        TempData["ErrorMessagePhoneNumber"] = "Please check the phone number format.";
                        isError = true;
                    }

                    // If there is an error, return to the view with the error messages
                    if (isError)
                    {
                        return View("Index", customer);
                    }

                    //if no error
                    customer.Status = 1;
                    customer.CustomerType = 1;
                    customer.DeleteFlag = false;
                    customer.Ads = true;
                    customer.CreatedOn = DateTime.Today;
                    _context.Customers.Add(customer);
                    _context.SaveChanges();
                    TempData["SuccessRegisterMessage"] = "Submited successfully!";
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            return Redirect(Constant.DEFAULT_CUSTOMER_PAGE);
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