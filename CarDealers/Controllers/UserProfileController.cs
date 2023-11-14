using CarDealers.Entity;
using CarDealers.Models.BookingServiceModel;
using CarDealers.Util;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Globalization;

namespace CarDealers.Controllers
{
    public class UserProfileController : CustomController
    {
        private readonly CarDealersContext _context;

        public UserProfileController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE };
        }

        [HttpGet]
        public IActionResult Index()
        {
			ViewBag.FooterText = GetDefaultFooterText();
			ViewBag.Menu = GetDefaultMenu();
			if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            int check = Converter.ParseInt(customerId);
            if (check == 0)
            {
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return Redirect(Constant.LOGIN_PAGE);
            }
            else
            {
                var c = _context.Customers.Where(x => x.CustomerId == check).FirstOrDefault();
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return View(c);
            }
        }

        [HttpPost]
        public IActionResult Index(string fullName, string phoneNumber, string address, bool gender, bool ads, DateTime dob, IFormFile image)
        {
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var c = _context.Customers.Where(x => x.CustomerId == Converter.ParseInt(customerId)).FirstOrDefault();
                try
                {
                    if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(address) || dob == default(DateTime))
                    {
                        TempData["ErrorMessage"] = "Please fill in all fields.";
                        return RedirectToAction("Index");
                    }
                    if (!Validation.CheckPhone(phoneNumber))
                    {
                        TempData["ErrorMessagePhoneNumber"] = "Please check phonenumber again";
                    }
                    else
                    {
                        c.FullName = fullName;
                        c.PhoneNumber = phoneNumber;
                        c.Address = address;
                        c.Gender = gender;
                        c.Ads = ads;
                        c.Dob = dob;
                        c.ModifiedOn = DateTime.Now;
                        if (image != null && image.Length > 0)
                        {
                            if (image.Length > 2 * 1024 * 1024)
                            {
                                TempData["ErrorMessageImage"] = "The uploaded image is too large. It should be 2MB or less.";
                                ViewBag.FooterText = GetDefaultFooterText();
                                ViewBag.Menu = GetDefaultMenu();
                                return RedirectToAction("Index"); 
                            }

                            // Kiểm tra định dạng tệp ảnh
                            string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                            string fileExtension = Path.GetExtension(image.FileName).ToLower();
                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                TempData["ErrorMessageImage"] = "Only .png, .jpeg, .jpg files are allowed.";
                                ViewBag.FooterText = GetDefaultFooterText();
                                ViewBag.Menu = GetDefaultMenu();
                                return RedirectToAction("Index"); 
                            }

                            // Lưu ảnh và cập nhật đối tượng Customer
                            var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGE_AVATAR_PATH, fileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(fileStream);
                            }

                            c.Image = Path.Combine(Constant.IMAGE_AVATAR_PATH, fileName).Replace('\\', '/'); // Lưu đường dẫn tệp trong cơ sở dữ liệu
                        }
                        _context.SaveChanges();
                        TempData["SuccessMessage"] = "Changes saved successfully!";
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                return Redirect("/UserProfile/Index");
            }
        }


        public IActionResult YourCar()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            try
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var orders = _context.Orders
                    .Where(x => x.CustomerId == Converter.ParseInt(customerId) && x.Status == 1).ToList();


                var orderDetails = _context.OrderDetails.Where(e => e.DeleteFlag == false)
                    .Include(x => x.Car).ThenInclude(x => x.Brand)
                    .Include(x => x.Car).ThenInclude(x => x.FuelType)
                    .Include(x => x.Car).ThenInclude(x => x.EngineType)
                    .Include(x => x.Car).ThenInclude(x => x.CarType)
                    .Include(x => x.Color).ToList();

                var newOrders = orders.Select(e => new Order
                {
                    OrderId = e.OrderId,
                    CustomerId = e.CustomerId,
                    OrderDate = e.OrderDate,
                    Status = e.Status,
                    OrderDetails = orderDetails.Where(x => x.OrderId == e.OrderId).ToList()
                }).ToList();
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return View(newOrders);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult ChangePassword()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View();
        }


        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var customer = _context.Customers.Where(x => x.CustomerId == Converter.ParseInt(customerId)).FirstOrDefault();
                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    TempData["ErrorMessage"] = "Please fill all fields.";
                    ViewBag.FooterText = GetDefaultFooterText();
                    ViewBag.Menu = GetDefaultMenu();
                    return View();
                }

                if (customer.Password != HashPassword.GetMD5(currentPassword))
                {
                    TempData["ErrorMessagePassword"] = "Current password is incorrect.";
                    ViewBag.FooterText = GetDefaultFooterText();
                    ViewBag.Menu = GetDefaultMenu();
                    return View();
                }
              
                if (!Validation.CheckPassword(newPassword))
                {
                    TempData["ErrorMessageNewPassword"] = "Password must have at least 6 character, contains special characters, uppercase letters, lowercase letters and numbers ";
                    ViewBag.FooterText = GetDefaultFooterText();
                    ViewBag.Menu = GetDefaultMenu();
                    return View();
                }

                if (newPassword != confirmPassword)
                {
                    TempData["ErrorMessageConfirmPassword"] = "New passwords do not match.";
                    ViewBag.FooterText = GetDefaultFooterText();
                    ViewBag.Menu = GetDefaultMenu();
                    return View();
                }

                customer.Password = HashPassword.GetMD5(newPassword);
                _context.SaveChanges();
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult YourBooking(int? page)
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            int pageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            //fetch data from database table orders
            var customer = _context.Customers.FirstOrDefault(e => e.CustomerId == int.Parse(customerId));
            var customerParent = _context.Customers.AsEnumerable();
            var bookingServices = _context.BookingServices.Include(e => e.Customer).ThenInclude(e => e.Car).Where(x => x.DeleteFlag == false &&
            (x.CustomerParentId == int.Parse(customerId) || x.CustomerId == int.Parse(customerId) || customerParent.FirstOrDefault(e => e.CustomerId == x.CustomerParentId).PhoneNumber.Equals(customer.PhoneNumber)))
            .Include(e => e.Customer)
            .OrderByDescending(p => p.DateBooking)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();
            var bookingRefers = _context.BookingRefers.Include(e => e.ServiceType).ToList();
            var bookingModel = new ViewBookingCustomer();
            var viewBookingServiceModels = bookingServices.Select(e => new ViewBookingCustomer
            {
                Id = e.BookingId,
                FullName = e.Customer.FullName,
                PhoneNumber = e.Customer.PhoneNumber,
                PlateNumber = e.Customer.PlateNumber,
                Email = e.Customer.Email,
                DateBooking = e.DateBooking.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                Status = bookingModel.StatusList.FirstOrDefault(x => int.Parse(x.Value) == e.Status).Text,
                CarName = e.Customer.Car.Model
            }).ToList();
            if (viewBookingServiceModels.Count() != 0)
            {
                for (int i = 0; i < viewBookingServiceModels.Count(); i++)
                {
                    var serviceTypes = bookingRefers.Where(e => e.BookingId == viewBookingServiceModels[i].Id).ToList();
                    string services = "";
                    if (serviceTypes.Count() != 0)
                    {
                        for (int j = 0; j < serviceTypes.Count(); j++)
                        {
                            if (j != 0)
                            {
                                services = services + ", " + serviceTypes[j].ServiceType.ServiceTypeName;
                            }
                            else
                            {
                                services = services + serviceTypes[j].ServiceType.ServiceTypeName;
                            }
                        }
                    }
                    viewBookingServiceModels[i].Service = services;
                }
            }
            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = bookingServices.Count();
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View(viewBookingServiceModels);
        }

        public IActionResult YourOrder()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
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
