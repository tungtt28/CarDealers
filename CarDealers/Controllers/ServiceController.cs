using CarDealers.Entity;
using CarDealers.Models.BookingServiceModel;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarDealers.Controllers
{
    public class ServiceController : CustomController
    {
        private readonly CarDealersContext _context;

        public ServiceController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE, Constant.GUEST_ROLE };
        }
        public IActionResult ServicePage()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).OrderBy(e => e.Model).ToList();
            var carSelectList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model
            }).ToList();
            ViewBag.CarOptions = carSelectList;
            var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).OrderBy(e => e.ServiceTypeName).ToList();
            var serviceTypeSelectList = serviceTypes.Select(serviceType => new SelectListItem
            {
                Value = serviceType.ServiceTypeId.ToString(),
                Text = serviceType.ServiceTypeName
            }).ToList();
            ViewBag.ServiceTypeOptions = serviceTypeSelectList;
            var viewModel = new CreateGuestBookingServiceViewModel();
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBookingService(CreateGuestBookingServiceViewModel bookingService)
        {
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).OrderBy(e => e.Model).ToList();
            var carSelectList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model,
                Selected = bookingService.CarId == car.CarId
            }).ToList();
            ViewBag.CarOptions = carSelectList;
            var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).OrderBy(e => e.ServiceTypeName).ToList();
            var serviceTypeSelectList = serviceTypes.Select(serviceType => new SelectListItem
            {
                Value = serviceType.ServiceTypeId.ToString(),
                Text = serviceType.ServiceTypeName,
                Selected = bookingService.SelectedServiceIds == null ? false : bookingService.SelectedServiceIds.Contains(serviceType.ServiceTypeId)
            }).ToList();
            ViewBag.ServiceTypeOptions = serviceTypeSelectList;
            if (serviceTypes == null || serviceTypes.Count == 0 || bookingService.SelectedServiceIds == null)
            {
                ModelState.AddModelError("SelectedServiceIds", "Service Require");
                return View("ServicePage",bookingService);
            }
            if (!Validation.CheckEmail(bookingService.Email))
            {
                ModelState.AddModelError("Email", "Email is valid");
                return View("ServicePage", bookingService);
            }
            if (!Validation.CheckPhone(bookingService.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "PhoneNumber is valid");
                return View("ServicePage", bookingService);
            }
            if (!Validation.CheckFullName(bookingService.FullName))
            {
                ModelState.AddModelError("FullName", "FullName is valid");
                return View("ServicePage", bookingService);
            }
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    CarId = bookingService.CarId,
                    Email = bookingService.Email.Trim(),
                    PhoneNumber = bookingService.PhoneNumber.Trim(),
                    FullName = bookingService.FullName.ReplaceSpace().Trim().CapitalizeFullName(),
                    CustomerType = 1,
                    Kilometerage = int.Parse(bookingService.Kilometerage.Replace(",", "")),
                    PlateNumber = bookingService.PlateNumber
                };
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                // Save the BookingService details to the database
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var newBookingService = new BookingService
                {
                    CustomerId = customer.CustomerId,
                    CustomerParentId = customerId != null ? int.Parse(customerId) : null,
                    Note = bookingService.Note,
                    DateBooking = DateTime.ParseExact(bookingService.DateBooking, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                    Status = 0,
                };
                await _context.BookingServices.AddAsync(newBookingService);
                await _context.SaveChangesAsync();
                var bookingRefers = bookingService.SelectedServiceIds.Select(e => new BookingRefer
                {
                    BookingId = newBookingService.BookingId,
                    ServiceTypeId = e
                }).ToList();
                await _context.BookingRefers.AddRangeAsync(bookingRefers);
                await _context.SaveChangesAsync();
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View("ServicePage", bookingService);
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
