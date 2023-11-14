using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Drawing.Printing;
using CarDealers.Models.CarPageModel;
using CarDealers.Areas.Admin.Models.OrderCarViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace CarDealers.Controllers
{
    public class CarPageController : CustomController
    {

        private readonly CarDealersContext _context;

        public CarPageController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE, Constant.GUEST_ROLE };
        }

        public IActionResult ListCar(int? page, int? pageSize, string Keyword, string brandFilter, string carTypeFilter, string engineFilter, string fuelFilter)
        {
            int defaultPageSize = 10; // Set a default number of records per page
            int pageNumber = page ?? 1;
            int recordsPerPage = pageSize ?? defaultPageSize;
            var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();
            var brandSelectList = brands.Select(brand => new SelectListItem
            {
                Value = brand.BrandId.ToString(),
                Text = brand.BrandName,
                Selected = brand.BrandId.ToString().Equals(brandFilter)
            }).ToList();
            ViewBag.BrandOptions = brandSelectList;
            var cartypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
            var carTypeSelectList = cartypes.Select(cartype => new SelectListItem
            {
                Value = cartype.CarTypeId.ToString(),
                Text = cartype.TypeName,
                Selected = cartype.CarTypeId.ToString().Equals(carTypeFilter)
            }).ToList();
            ViewBag.CarTypeOptions = carTypeSelectList;
            var enginetypes = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
            var engineTypeSelectList = enginetypes.Select(enginetype => new SelectListItem
            {
                Value = enginetype.EngineTypeId.ToString(),
                Text = enginetype.EngineTypeName,
                Selected = enginetype.EngineTypeId.ToString().Equals(engineFilter)
            }).ToList();
            ViewBag.EngineType = engineTypeSelectList;
            var fueltypes = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
            var fuelTypeSelectList = fueltypes.Select(fueltype => new SelectListItem
            {
                Value = fueltype.FuelTypeId.ToString(),
                Text = fueltype.FuelTypeName,
                Selected = fueltype.FuelTypeId.ToString().Equals(fuelFilter)
            }).ToList();
            ViewBag.FuelType = fuelTypeSelectList;
            ViewBag.Keyword = Keyword;
            //fetch data from database table orders
            var query1 = _context.Cars.Where(x => x.DeleteFlag == false)
                                        .Include(x => x.Brand)
                                        .Include(x => x.CarType)
                                        .Include(x => x.EngineType)
                                        .Include(x => x.FuelType)
                                        .OrderBy(p => p.CarId).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {

                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.Model.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.Model.ToLower()) || keywords.Any(e => u.Model.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            if (!string.IsNullOrEmpty(brandFilter))
            {
                query = query.Where(u => u.BrandId == int.Parse(brandFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(engineFilter))
            {
                query = query.Where(u => u.EngineTypeId == int.Parse(engineFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(fuelFilter))
            {
                query = query.Where(u => u.FuelTypeId == int.Parse(fuelFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(carTypeFilter))
            {
                query = query.Where(u => u.CarTypeId == int.Parse(carTypeFilter.Trim().ToLower()));
            }

            var cars = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();
            var viewModels = cars.Select(x => new CarViewModel
            {
                CarId = x.CarId,
                CarName = x.Model,
                ExportPrice = x.ExportPrice?.ToString("N0").Replace(",", "."),
                Image = x.Image
            }).ToList();
            ViewBag.Brand = brandFilter;
            ViewBag.CarType = carTypeFilter;
            ViewBag.Engine = engineFilter;
            ViewBag.Fuel = fuelFilter;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View("Index", viewModels);
        }

        [HttpGet]
        public IActionResult CreateOrder(int id)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

            var car = _context.Cars.Where(x => x.CarId == id).FirstOrDefault();
            if (car == null)
            {
                return RedirectToAction("RecordNotFound");
            }
            var model = new CreateOrderViewModel();
            model.CarId = id;
            model.Price = car.ExportPrice?.ToString("N0").Replace(",", ".");
            model.DepositePrice = car.DepositPrice?.ToString("N0").Replace(",", ".");
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            ViewBag.Image = car.Image;
            ViewBag.CustomerId = customerId;
            if(customerId != null)
            {
                var customer = _context.Customers.Where(x => x.CustomerId == int.Parse(customerId)).FirstOrDefault();
                model.FullName = customer.FullName;
                model.PhoneNumber = customer.PhoneNumber;
                model.Email = customer.Email;
                model.Quantity = "1";
            }
            model.Quantity = "1";
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrder(CreateOrderViewModel model)
        {
            var customerAll = _context.Customers.ToList();
            var car = _context.Cars.Where(x => x.CarId == model.CarId).FirstOrDefault();
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            bool fail = false;
            //name
            if (customerId == null)
            {
                if (model.FullName.IsNullOrEmpty())
                {
                    ModelState.AddModelError("FullName", "FullName cannot empty.");
                    fail = true;
                }
                //email
                if (model.Email.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Email", "Email cannot empty.");
                    fail = true;
                }
                if (customerAll.Any(c => c.Email.Equals(model.Email)))
                {
                    ModelState.AddModelError("Email", "This email currently been held by other customer, please chose other email.");
                    fail = true;
                }
                //phone number
                if (model.PhoneNumber.IsNullOrEmpty())
                {
                    ModelState.AddModelError("PhoneNumber", "Phone number cannot empty.");
                    fail = true;
                }
                //phone number
                if (model.Quantity.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Quantity", "Quantity cannot empty.");
                    fail = true;
                }
                //check if return or not
                if (fail == true)
                {
                    ViewBag.Image = car.Image;
                    return View(model);
                }
            }
            if (ModelState.IsValid)
            {
                var customer = new Customer();
                if (customerId == null)
                {
                    customer.Email = model.Email;
                    customer.PhoneNumber = model.PhoneNumber;
                    customer.FullName = model.FullName;
                    customer.CustomerType = 1;
                    customer.CreatedBy = customerId != null ? int.Parse(customerId) : null;
                    customer.CreatedOn = DateTime.Now;

                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();
                }

                var order = new Order
                {
                    CustomerId = customerId == null ? customer.CustomerId : int.Parse(customerId),
                    OrderDate = DateTime.Now,
                    Status = 0,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                int colorId = _context.ColorCarRefers.Where(x => x.CarId == model.CarId).FirstOrDefault().ColorId;
                var OrderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    CarId = model.CarId,
                    ColorId = colorId,
                    CouponId = null,
                    SellerId = null,
                    Quantity = int.Parse(model.Quantity),
                    TotalPrice = int.Parse(model.Quantity) * car.ExportPrice,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.OrderDetails.AddAsync(OrderDetail);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            ViewBag.Image = car.Image;
            return View(model);
        }
        [HttpGet]
        public ActionResult CarDetails(int id)
        {
            if (ModelState.IsValid)
            {
                var cars = _context.Cars.Where(x => x.CarId == id && x.DeleteFlag == false).Include(x => x.Brand)
                                        .Include(x => x.CarType)
                                        .Include(x => x.EngineType)
                                        .Include(x => x.FuelType)
                                       .Include(x => x.ImageCars)
            .OrderBy(p => p.CarId).FirstOrDefault();


                if (cars == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return View(cars);
            }
            return RedirectToAction("RecordNotFound");
        }


        public async Task<ActionResult> GetAllImages(int carId)
        {

            var images = await _context.ImageCars
                .Where(x => x.CarId == carId && x.DeleteFlag == false)
                .ToListAsync();
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View(images);
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
