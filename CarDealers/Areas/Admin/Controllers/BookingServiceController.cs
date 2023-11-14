using CarDealers.Areas.Admin.Models.BookingServiceModel;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class BookingServiceController : CustomController
    {
        private readonly CarDealersContext _context;

        public BookingServiceController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        [HttpGet]
        public IActionResult CreateBookingService()
        {
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).OrderBy(e => e.Model).ToList();
            var carSelectList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model,
            }).ToList();
            ViewBag.CarOptions = carSelectList;
            var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).OrderBy(e => e.ServiceTypeName).ToList();
            var serviceTypeSelectList = serviceTypes.Select(serviceType => new SelectListItem
            {
                Value = serviceType.ServiceTypeId.ToString(),
                Text = serviceType.ServiceTypeName
            }).ToList();
            ViewBag.ServiceTypeOptions = serviceTypeSelectList;
            var viewModel = new CreateBookingServiceViewModel();
            return View(viewModel);
        }

        public IActionResult ListBookingService(int? page, string Keyword, string serviceTypeFilter, string statusFilter, int? pageSize)
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;
            var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).ToList();
            var serviceTypeSelectList = serviceTypes.Select(serviceType => new SelectListItem
            {
                Value = serviceType.ServiceTypeId.ToString(),
                Text = serviceType.ServiceTypeName,
                Selected = serviceType.ServiceTypeId.ToString().Equals(serviceTypeFilter)
            }).ToList();
            ViewBag.ServiceTypeOptions = serviceTypeSelectList;

            var statusList = new UpdateBookingServiceViewModel();
            ViewBag.StatusOptions = statusList.StatusList.Select(e => new SelectListItem
            {
                Value = e.Value,
                Text = e.Text,
                Selected = e.Value.Equals(statusFilter)
            });
            //fetch data from database table orders
            var query1 = _context.BookingServices.Where(x => x.DeleteFlag == false).Include(e => e.Customer).OrderByDescending(e => e.DateBooking).ToList();
            var BookingRefers = _context.BookingRefers.ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.Customer.FullName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.Customer.FullName.ToLower()) || keywords.Any(e => u.Customer.FullName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            if (!string.IsNullOrEmpty(serviceTypeFilter))
            {
                query = query.Where(u => BookingRefers.Where(e => e.BookingId == u.BookingId).Any(e => e.ServiceTypeId == int.Parse(serviceTypeFilter)));
            }
            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(u => u.Status == int.Parse(statusFilter.Trim().ToLower()));
            }
            var bookingServices = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();
            var bookings = bookingServices.Select(e => new ListBookingServiceViewModel
            {
                BookingId = e.BookingId,
                FullName = e.Customer.FullName,
                DateBooking = e.DateBooking.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                Email = e.Customer.Email,
                PhoneNumber = e.Customer.PhoneNumber,
                PlateNumber = e.Customer.PlateNumber,
                Status = statusList.StatusList.FirstOrDefault(s => s.Value.Equals(e.Status.ToString())).Text,
            }).ToList();
            // Pass the records and page information to the view
            ViewBag.ServiceType = serviceTypeFilter;
            ViewBag.Status = statusFilter;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListBookingService", bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBookingService(CreateBookingServiceViewModel bookingService)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
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
                return View(bookingService);
            }
            if (!Validation.CheckEmail(bookingService.Email))
            {
                ModelState.AddModelError("Email", "Email is valid");
                return View(bookingService);
            }
            if (!Validation.CheckPhone(bookingService.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "PhoneNumber is valid");
                return View(bookingService);
            }
            if (!Validation.CheckFullName(bookingService.FullName))
            {
                ModelState.AddModelError("FullName", "FullName is valid");
                return View(bookingService);
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
                    PlateNumber = bookingService.PlateNumber,
                    Ads = true,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                // Save the BookingService details to the database
                var newBookingService = new BookingService
                {
                    CustomerId = customer.CustomerId,
                    Note = bookingService.Note,
                    DateBooking = DateTime.ParseExact(bookingService.DateBooking, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                    Status = 0,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
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
                return RedirectToAction("ListBookingService");
            }
            return View(bookingService);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateBookingService(int id)
        {
            if (ModelState.IsValid)
            {
                var bookingService = _context.BookingServices.Where(x => x.BookingId == id && x.DeleteFlag == false).Include(e => e.Customer).FirstOrDefault();
                var bookingRefers = _context.BookingRefers.Where(x => x.BookingId == bookingService.BookingId).Select(e => e.ServiceTypeId).ToList();
                var cars = _context.Cars.Where(x => x.DeleteFlag == false).OrderBy(e => e.Model).ToList();
                if (bookingService == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                UpdateBookingServiceViewModel booking = new UpdateBookingServiceViewModel
                {
                    BookingId = id,
                    CustomerId = bookingService.CustomerId,
                    CarId = bookingService.Customer.CarId,
                    FullName = bookingService.Customer.FullName,
                    Email = bookingService.Customer.Email,
                    PhoneNumber = bookingService.Customer.PhoneNumber,
                    Kilometerage = bookingService.Customer.Kilometerage?.ToString("N0"),
                    Note = bookingService.Note,
                    PlateNumber = bookingService.Customer.PlateNumber,
                    DateBooking = bookingService.DateBooking.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture),
                    SelectedServiceIds = bookingRefers,
                    Status = bookingService.Status
                };
                var carSelectList = cars.Select(car => new SelectListItem
                {
                    Value = car.CarId.ToString(),
                    Text = car.Model,
                    Selected = bookingService.Customer.CarId == car.CarId
                }).ToList();
                ViewBag.CarOptions = carSelectList;
                var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).OrderBy(e => e.ServiceTypeName).ToList();
                var serviceTypeSelectList = serviceTypes.Select(serviceType => new SelectListItem
                {
                    Value = serviceType.ServiceTypeId.ToString(),
                    Text = serviceType.ServiceTypeName,
                    Selected = bookingRefers.Contains(serviceType.ServiceTypeId)
                }).ToList();
                ViewBag.ServiceTypeOptions = serviceTypeSelectList;
                booking.StatusList = booking.StatusList.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text,
                    Selected = bookingService.Status.ToString().Equals(x.Value)
                }).ToList();
                return View(booking);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateBookingService(UpdateBookingServiceViewModel model)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var bookingService = _context.BookingServices.Where(x => x.BookingId == model.BookingId && x.DeleteFlag == false).Include(e => e.Customer).FirstOrDefault();
            var bookingRefer = _context.BookingRefers.Where(x => x.BookingId == model.BookingId).ToList();
            var bookingRefers = bookingRefer.Select(e => e.ServiceTypeId).ToList();
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).OrderBy(e => e.Model).ToList();
            var carSelectList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model,
                Selected = bookingService.Customer.CarId == car.CarId
            }).ToList();
            ViewBag.CarOptions = carSelectList;
            var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).OrderBy(e => e.ServiceTypeName).ToList();
            var serviceTypeSelectList = serviceTypes.Select(serviceType => new SelectListItem
            {
                Value = serviceType.ServiceTypeId.ToString(),
                Text = serviceType.ServiceTypeName,
                Selected = bookingRefers.Contains(serviceType.ServiceTypeId)
            }).ToList();
            ViewBag.ServiceTypeOptions = serviceTypeSelectList;
            if (model.SelectedServiceIds == null || model.SelectedServiceIds.Count() == 0)
            {
                ModelState.AddModelError("SelectedServiceIds", "Service Require");
                return View(model);
            }
            if (!Validation.CheckEmail(model.Email))
            {
                ModelState.AddModelError("Email", "Email is valid");
                return View(model);
            }
            if (!Validation.CheckPhone(model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "PhoneNumber is valid");
                return View(model);
            }
            if (!Validation.CheckFullName(model.FullName))
            {
                ModelState.AddModelError("FullName", "FullName is valid");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == bookingService.CustomerId && x.DeleteFlag == false);
                customer.CarId = model.CarId;
                customer.Email = model.Email.Trim();
                customer.PhoneNumber = model.PhoneNumber.Trim();
                customer.FullName = model.FullName.ReplaceSpace().Trim().CapitalizeFullName();
                customer.CustomerType = 1;
                customer.Kilometerage = int.Parse(model.Kilometerage.Replace(",", ""));
                customer.PlateNumber = model.PlateNumber;
                customer.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                customer.ModifiedOn = DateTime.Now;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                // Save the BookingService details to the database
                bookingService.Note = model.Note;
                bookingService.DateBooking = DateTime.ParseExact(model.DateBooking, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                bookingService.Status = model.Status.Value;
                bookingService.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                bookingService.ModifiedOn = DateTime.Now;
                _context.BookingServices.Update(bookingService);
                _context.BookingRefers.RemoveRange(bookingRefer);
                await _context.SaveChangesAsync();
                var newBookingRefers = model.SelectedServiceIds.Select(e => new BookingRefer
                {
                    BookingId = bookingService.BookingId,
                    ServiceTypeId = e
                }).ToList();
                await _context.BookingRefers.AddRangeAsync(newBookingRefers);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListBookingService");
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult DeleteListBookingService(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                // Tiến hành xóa các bản ghi có ID nằm trong danh sách selectedIds
                foreach (var id in selectedIds)
                {
                    var bookingServices = _context.BookingServices.Find(id);
                    if (bookingServices != null)
                    {
                        bookingServices.DeleteFlag = true;
                        bookingServices.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        bookingServices.ModifiedOn = DateTime.Now;
                        _context.BookingServices.Update(bookingServices);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListBookingService");
        }
    }
}
