using CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel;
using CarDealers.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarDealers.Util;
using Newtonsoft.Json;

namespace CarDealers.Controllers
{
	public class OrderAccessoriesController : CustomController
	{
		private readonly CarDealersContext _context;

		public OrderAccessoriesController(CarDealersContext context)
		{
			_context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE};
        }

		[HttpGet]
		public IActionResult CreateOrderAccessories()
		{
            var viewModel = new CreateOrderAccessoriesViewModel();
			var a = GetCartItems();
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
			if(customerId != null)
			{
				var cus = _context.Customers.FirstOrDefault(x => x.CustomerId == int.Parse(customerId));
				viewModel.FullName = cus.FullName;
                viewModel.Address = cus.Address;
                viewModel.PhoneNumber = cus.PhoneNumber;
                viewModel.Email = cus.Email;

				ViewBag.CustomerId = customerId;
            }
            viewModel.Details = a.ToList();
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateOrderAccessories(CreateOrderAccessoriesViewModel orderA)
		{
			string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
			var a = GetCartItems();
			var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
			if (ModelState.IsValid)
			{
				var customer = new Customer
				{
					Email = orderA.Email,
					PhoneNumber = orderA.PhoneNumber,
					FullName = orderA.FullName,
					Address = orderA.Address,
					CustomerType = 1,
					CreatedBy = customerId != null ? int.Parse(customerId) : null,
					CreatedOn = DateTime.Now
				};
				await _context.Customers.AddAsync(customer);
				await _context.SaveChangesAsync();

				var order = new Order
				{
					CustomerId = customer.CustomerId,
					Status = 0,
					CreatedBy = customerId != null ? int.Parse(customerId) : null,
					CreatedOn = DateTime.Now
				};
				await _context.Orders.AddAsync(order);
				await _context.SaveChangesAsync();

				
				var orderAccessories = _context.OrderAccessoryDetails.Where(x => x.DeleteFlag == false).ToList();
				var newOrder = a.Select(x => new OrderAccessoryDetail
				{
					OrderId = order.OrderId,
					AccessoryId = x.accessory.AccessoryId,
					SellerId = customerId != null ? int.Parse(customerId) : null,
					Quantity = 1,
					TotalPrice = x.accessory.ExportPrice,
					
					CreatedBy = customerId != null ? int.Parse(customerId) : null,
					CreatedOn = DateTime.Now
				}).ToList();

				await _context.OrderAccessoryDetails.AddRangeAsync(newOrder);
				await _context.SaveChangesAsync();

				return RedirectToAction("Index", "Home");
			}
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View(orderA);
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

		public const string CARTKEY = "cart";

		// Lấy cart từ Session (danh sách CartItem)
		List<CartItem> GetCartItems()
		{
			var session = HttpContext.Session;
			string jsoncart = session.GetString(CARTKEY);
			if (jsoncart != null)
			{
				return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
			}
			return new List<CartItem>();
		}

		// Xóa cart khỏi session
		void ClearCart()
		{
			var session = HttpContext.Session;
			session.Remove(CARTKEY);
		}

		// Lưu Cart (Danh sách CartItem) vào session
		void SaveCartSession(List<CartItem> ls)
		{
			var session = HttpContext.Session;
			string jsoncart = JsonConvert.SerializeObject(ls);
			session.SetString(CARTKEY, jsoncart);
		}


	}
}
