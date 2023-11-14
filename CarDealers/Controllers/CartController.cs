using CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarDealers.Controllers
{
	public class CartController : CustomController
	{
		private readonly CarDealersContext _context;

		public CartController(CarDealersContext context)
		{
			_context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE};
        }

		[HttpGet]
		public ActionResult addProduct(int id)
		{
			string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
			if (!customerId.IsNullOrEmpty())
			{
				var orderA = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.CustomerId == int.Parse(customerId) && x.Status == 0);
				var accessory = _context.AutoAccessories.FirstOrDefault(x => x.DeleteFlag == false && x.AccessoryId == id);

				if (orderA == null)
				{
					var order = new Order
					{
						CustomerId = int.Parse(customerId),
						Status = 0,
						CreatedBy = customerId != null ? int.Parse(customerId) : null,
						CreatedOn = DateTime.Now
					};
					_context.Orders.Add(order);
					_context.SaveChanges();

					var newOrder = new OrderAccessoryDetail
					{
						OrderId = order.OrderId,
						AccessoryId = id,
						SellerId = customerId != null ? int.Parse(customerId) : null,
						Quantity = 1,
						TotalPrice = accessory.ExportPrice,
						CreatedBy = customerId != null ? int.Parse(customerId) : null,
						CreatedOn = DateTime.Now
					};
					_context.OrderAccessoryDetails.Add(newOrder);
					_context.SaveChanges();


					var idOrder = order.OrderId;
					return RedirectToAction("ShowCart", new { idOrder });

				}
				else
				{
					var newOrder = new OrderAccessoryDetail
					{
						OrderId = orderA.OrderId,
						AccessoryId = id,
						SellerId = customerId != null ? int.Parse(customerId) : null,
						Quantity = 1,
						TotalPrice = accessory.ExportPrice,
						CreatedBy = customerId != null ? int.Parse(customerId) : null,
						CreatedOn = DateTime.Now
					};
					_context.OrderAccessoryDetails.Add(newOrder);
					_context.SaveChanges();

					var idOrder = orderA.OrderId;
					return RedirectToAction("ShowCart", new { idOrder });
				}


			}
			else
			{
				//var orderA = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.CustomerId == int.Parse(customerId) && x.Status != 0);
				//var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
				//var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
				//var customers = _context.Customers.Where(x => x.DeleteFlag == false && x.CustomerType == 2).ToList();


				//var newOrder = _context.OrderAccessoryDetails
				//        .Include(x => x.Order)
				//        .Include(x => x.Accessory)
				//        .Include(x => x.Coupon)
				//        .Where(x => x.DeleteFlag == false)
				//        .Select(x => new OrderAccessoryDetail
				//        {
				//            OrderId = order.OrderId,
				//            AccessoryId = x.AccessoryId,
				//            CouponId = x.CouponId,
				//            SellerId = customerId != null ? int.Parse(customerId) : null,
				//            Quantity = 1,
				//            TotalPrice = x.Accessory.ExportPrice,
				//            CreatedBy = customerId != null ? int.Parse(customerId) : null,
				//            CreatedOn = DateTime.Now
				//        }).ToList();
				//_context.OrderAccessoryDetails.AddRangeAsync(newOrder);
				//_context.SaveChangesAsync();

				//return View(orderA);
				return View();
			}



		}

		[HttpGet]
		public ActionResult ShowCart(int idOrder)
		{
			var orderViewModel = new ViewOrder();
			var orderA = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.OrderId == idOrder && x.Status == 0);
			orderViewModel = new ViewOrder
			{
				OrderId = orderA.OrderId,
				FullName = orderA.Customer.FullName,
				PhoneNumber = orderA.Customer.PhoneNumber,
				CreatedOn = orderA.CreatedOn,

			};

			orderViewModel.OrderAccessories = _context.OrderAccessoryDetails.Where(x => x.OrderId == idOrder && x.DeleteFlag == false)
				.Include(x => x.Coupon)
				.Include(x => x.Accessory)
				.ToList();
			foreach (var item in orderViewModel.OrderAccessories)
			{
				orderViewModel.totalPrice += item.TotalPrice.Value;
			}
			ViewBag.FooterText = GetDefaultFooterText();
			ViewBag.Menu = GetDefaultMenu();
			return View(orderViewModel);


		}

		[HttpPost]
		public ActionResult ShowCartMenu()
		{
			string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
			var orderViewModel = new ViewOrder();
			var orderA = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.CustomerId == int.Parse(customerId) && x.Status == 0);
			if(orderA != null)
			{
				orderViewModel = new ViewOrder
				{
					OrderId = orderA.OrderId,
					FullName = orderA.Customer.FullName,
					PhoneNumber = orderA.Customer.PhoneNumber,
					CreatedOn = orderA.CreatedOn,

				};

				orderViewModel.OrderAccessories = _context.OrderAccessoryDetails.Where(x => x.OrderId == orderA.OrderId && x.DeleteFlag == false)
					.Include(x => x.Coupon)
					.Include(x => x.Accessory)
					.ToList();
				foreach (var item in orderViewModel.OrderAccessories)
				{
					orderViewModel.totalPrice += item.TotalPrice.Value;
				}
				ViewBag.FooterText = GetDefaultFooterText();
				ViewBag.Menu = GetDefaultMenu();
				return View(orderViewModel);
			}
			else
			{
				orderViewModel = new ViewOrder();
				return View(orderViewModel);
			}
		}

		[HttpPost]
		public ActionResult updateCart(ViewOrder update)
		{
			var orderAccessoriesDetails = _context.OrderAccessoryDetails
				.Include(x => x.Order.Customer)
				.Include(x => x.Order)
				.Include(x => x.Accessory)
				.Where(x => x.OrderId == update.OrderId && x.DeleteFlag == false).ToList();
			if (orderAccessoriesDetails == null)
			{
				return RedirectToAction("RecordNotFound");
			}


			foreach (var item in orderAccessoriesDetails)
			{
				item.Quantity = update.OrderAccessories.FirstOrDefault(x => x.OrderAccessoryId == item.OrderAccessoryId).Quantity;
				item.TotalPrice = item.Quantity * item.Accessory.ExportPrice;
			}

			_context.OrderAccessoryDetails.UpdateRange(orderAccessoriesDetails);
			_context.SaveChangesAsync();
			var idOrder = update.OrderId;

			return RedirectToAction("ShowCart", new { idOrder });

		}

		[HttpGet]
		public IActionResult CreateOrderAccessoriesForCustomer()
		{
			var viewModel = new CreateOrderAccessoriesForCustomerViewModel();
			var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
			var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
			var customers = _context.Customers.Where(x => x.DeleteFlag == false && x.CustomerType == 2).ToList();

			viewModel.AccessoryList = accessories.Select(accessorie => new SelectListItem
			{
				Value = accessorie.AccessoryId.ToString(),
				Text = accessorie.AccessoryName
			});
			viewModel.CustomerList = customers.Select(customer => new SelectListItem
			{
				Value = customer.CustomerId.ToString(),
				Text = customer.FullName
			});
			viewModel.CouponList = coupons.Select(coupon => new SelectListItem
			{
				Value = coupon.CouponId.ToString(),
				Text = coupon.Name
			});

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateOrderAccessoriesForCustomer(CreateOrderAccessoriesForCustomerViewModel orderA)
		{
			string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

			var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
			var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
			var customers = _context.Customers.Where(x => x.DeleteFlag == false && x.CustomerType == 2).ToList();

			orderA.AccessoryList = accessories.Select(accessorie => new SelectListItem
			{
				Value = accessorie.AccessoryId.ToString(),
				Text = accessorie.AccessoryName
			});
			orderA.CustomerList = customers.Select(customer => new SelectListItem
			{
				Value = customer.CustomerId.ToString(),
				Text = customer.FullName
			});
			orderA.CouponList = coupons.Select(coupon => new SelectListItem
			{
				Value = coupon.CouponId.ToString(),
				Text = coupon.Name
			});

			if (ModelState.IsValid)
			{

				var order = new Order
				{
					CustomerId = orderA.CustomerId,
					Status = 0,
					CreatedBy = customerId != null ? int.Parse(customerId) : null,
					CreatedOn = DateTime.Now
				};
				await _context.Orders.AddAsync(order);
				await _context.SaveChangesAsync();

				var select = accessories.Where(x => orderA.SelectedAccessories.Contains(x.AccessoryId));
				var orderAccessories = _context.OrderAccessoryDetails.Where(x => x.DeleteFlag == false).ToList();
				var newOrder = select.Select(x => new OrderAccessoryDetail
				{
					OrderId = order.OrderId,
					AccessoryId = x.AccessoryId,
					CouponId = orderA.CouponId,
					SellerId = customerId != null ? int.Parse(customerId) : null,
					Quantity = 1,
					TotalPrice = x.ExportPrice,

					CreatedBy = customerId != null ? int.Parse(customerId) : null,
					CreatedOn = DateTime.Now
				}).ToList();

				await _context.OrderAccessoryDetails.AddRangeAsync(newOrder);
				await _context.SaveChangesAsync();

				return RedirectToAction("ListOrderAccessories");
			}
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

	}
}
