using CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class OrderAccessoriesController : CustomController
    {
        private readonly CarDealersContext _context;

        public OrderAccessoriesController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListOrderAccessories()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            var orders = _context.Orders.ToList();
            var orderSelectList = orders.Select(order => new SelectListItem
            {
                Value = order.OrderId.ToString(),
                Text = order.OrderId.ToString()
            }).ToList();
            ViewBag.OrderOptions = orderSelectList;

            var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
            var accessoryTypeSelectList = accessories.Select(accessorie => new SelectListItem
            {
                Value = accessorie.AccessoryId.ToString(),
                Text = accessorie.AccessoryName
            }).ToList();
            ViewBag.AccessoryOptions = accessoryTypeSelectList;

            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            var couponTypeSelectList = coupons.Select(coupon => new SelectListItem
            {
                Value = coupon.CouponId.ToString(),
                Text = coupon.Name
            }).ToList();
            ViewBag.CouponOptions = couponTypeSelectList;

            var sellers = _context.Users.Where(x => x.DeleteFlag == false).ToList();
            var sellerTypeSelectList = sellers.Select(seller => new SelectListItem
            {
                Value = seller.UserId.ToString(),
                Text = seller.Username
            }).ToList();
            ViewBag.SellerOptions = sellerTypeSelectList;

            var statusList = new UpdateOrderAccessoriesViewModel().StatusList;
            var x = _context.OrderAccessoryDetails.Where(x => x.DeleteFlag == false).ToList();
            var orderaccessories = _context.OrderAccessoryDetails.Where(x => x.DeleteFlag == false)
                .Include(x => x.Order.Customer)
                .Include(x => x.Coupon)
                .Include(x => x.Seller)
                .GroupBy(item => item.OrderId)
                .Select(group => group.First())
                .ToList();
            var ordernew = orderaccessories?.Select(x => new ListOrderAccessoriesViewModel
            {
                OrderId = x.OrderId,
                FullName = x.Order.Customer.FullName,
                Email = x.Order.Customer.Email,
                PhoneNumber = x.Order.Customer.PhoneNumber,
                Coupon = x.Coupon == null ? null : x.Coupon.Name,
                SellerName = x.SellerId.HasValue ? x.Seller.FullName : null,
                Status = statusList.FirstOrDefault(s => s.Value.Equals(x.Order.Status.ToString())).Text,
            }).ToList();

            return View(ordernew);
        }

        [HttpGet]
        public IActionResult CreateOrderAccessories()
        {
            var viewModel = new CreateOrderAccessoriesViewModel();
            var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();

            viewModel.AccessoryList = accessories.Select(accessorie => new SelectListItem
            {
                Value = accessorie.AccessoryId.ToString(),
                Text = accessorie.AccessoryName
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
        public async Task<ActionResult> CreateOrderAccessories(CreateOrderAccessoriesViewModel orderA)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            
            var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            
            orderA.AccessoryList = accessories.Select(accessorie => new SelectListItem
            {
                Value = accessorie.AccessoryId.ToString(),
                Text = accessorie.AccessoryName
            });
            orderA.CouponList = coupons.Select(coupon => new SelectListItem
            {
                Value = coupon.CouponId.ToString(),
                Text = coupon.Name
            });

            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    Email = orderA.Email,
                    PhoneNumber = orderA.PhoneNumber,
                    FullName = orderA.FullName,
                    CustomerType = 1,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();

                var order = new Order
                {
                    CustomerId = customer.CustomerId,
                    Status =  0,
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

        [HttpGet]
        public async Task<ActionResult> UpdateOrderAccessories(int id)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            if (ModelState.IsValid)
            {
                var orderA = _context.OrderAccessoryDetails.Where(x => x.OrderId == id && x.DeleteFlag == false).ToList();

                if (orderA == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                var order = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.OrderId == id);
                var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
                var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
                var sellers = _context.Users.Where(x => x.DeleteFlag == false).ToList();
                var updateOrderAccessoriesViewModel = new UpdateOrderAccessoriesViewModel
                {
                    OrderId = order.OrderId,
                    FullName = order.Customer.FullName,
                    Email = order.Customer.Email,
                    PhoneNumber= order.Customer.PhoneNumber,
                    CouponId = orderA.FirstOrDefault().CouponId,
                    Status = order.Status.Value,
                    SelectedAccessories = orderA.Select(x => x.AccessoryId),

                    AccessoryList = accessories.Select(accessorie => new SelectListItem
                    {
                        Value = accessorie.AccessoryId.ToString(),
                        Text = accessorie.AccessoryName,
                        Selected = orderA.Select(x => x.AccessoryId).Contains(accessorie.AccessoryId),
                    }),
                    CouponList = coupons.Select(coupon => new SelectListItem
                    {
                        Value = coupon.CouponId.ToString(),
                        Text = coupon.Name,
                        Selected = coupon.CouponId == orderA.FirstOrDefault().CouponId,
                    }),
                };

                updateOrderAccessoriesViewModel.StatusList = updateOrderAccessoriesViewModel.StatusList.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text,
                    Selected = order.Status.ToString().Equals(x.Value)
                });
                return View(updateOrderAccessoriesViewModel);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrderAccessories(UpdateOrderAccessoriesViewModel updateCarViewModel)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var orderAccessoryDetails = _context.OrderAccessoryDetails.Where(x => x.OrderId == updateCarViewModel.OrderId && x.DeleteFlag == false).ToList();

            if (orderAccessoryDetails == null)
            {
                return RedirectToAction("RecordNotFound");
            }
            
            var order = _context.Orders.FirstOrDefault(x => x.OrderId == updateCarViewModel.OrderId);
            var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            
            updateCarViewModel.AccessoryList = accessories.Select(accessorie => new SelectListItem
            {
                Value = accessorie.AccessoryId.ToString(),
                Text = accessorie.AccessoryName,
                Selected = updateCarViewModel.SelectedAccessories.Contains(accessorie.AccessoryId),
            });
            updateCarViewModel.CouponList = coupons.Select(coupon => new SelectListItem
            {
                Value = coupon.CouponId.ToString(),
                Text = coupon.Name,
                Selected = coupon.CouponId == updateCarViewModel.CouponId,
            });


            if (ModelState.IsValid)
            {
                var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == order.CustomerId && x.DeleteFlag == false);
                customer.FullName = updateCarViewModel.FullName;
                customer.PhoneNumber = updateCarViewModel.PhoneNumber;
                customer.Email = updateCarViewModel.Email;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();

                order.Status = updateCarViewModel.Status;
                order.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                order.ModifiedOn = DateTime.Now;
                _context.Orders.Update(order);
                _context.OrderAccessoryDetails.RemoveRange(orderAccessoryDetails);
                await _context.SaveChangesAsync();

                var newOrder = accessories.Where(x => updateCarViewModel.SelectedAccessories.Contains(x.AccessoryId)).
                    Select(x => new OrderAccessoryDetail
                    {
                        OrderId = order.OrderId,
                        AccessoryId = x.AccessoryId,
                        CouponId = updateCarViewModel.CouponId,
                        SellerId = customerId != null ? int.Parse(customerId) : null,
                        Quantity = 1,
                        TotalPrice = x.ExportPrice,
                        CreatedBy = customerId != null ? int.Parse(customerId) : null,
                        CreatedOn = DateTime.Now

                    }).ToList();
                _context.OrderAccessoryDetails.AddRangeAsync(newOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListOrderAccessories");
            }
            return View(updateCarViewModel);
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            var orderA = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.OrderId == id);
			if (orderA == null)
			{
				return RedirectToAction("RecordNotFound");
			}
			var orderViewModel = new ViewOrder
            {
                OrderId = orderA.OrderId,
                FullName = orderA.Customer.FullName,
                PhoneNumber = orderA.Customer.PhoneNumber,
                CreatedOn= orderA.CreatedOn,
                
            };
            
            orderViewModel.OrderAccessories = _context.OrderAccessoryDetails.Where(x => x.OrderId == id && x.DeleteFlag == false)
				.Include(x => x.Coupon)
				.Include(x => x.Accessory)
				.ToList();
			foreach (var item in orderViewModel.OrderAccessories)
			{
                orderViewModel.totalPrice += item.TotalPrice.Value;
			}

			
            return View(orderViewModel);
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
        [HttpGet]
		public ActionResult Update(int id)
		{
			var orderA = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.OrderId == id);
			if (orderA == null)
			{
				return RedirectToAction("RecordNotFound");
			}
			var orderViewModel = new ViewOrder
			{
				OrderId = orderA.OrderId,
				FullName = orderA.Customer.FullName,
				PhoneNumber = orderA.Customer.PhoneNumber,
				CreatedOn = orderA.CreatedOn,

			};

			orderViewModel.OrderAccessories = _context.OrderAccessoryDetails.Where(x => x.OrderId == id && x.DeleteFlag == false)
				.Include(x => x.Coupon)
				.Include(x => x.Accessory)
				.ToList();
			foreach (var item in orderViewModel.OrderAccessories)
			{
				orderViewModel.totalPrice += item.TotalPrice.Value;
			}


			return View(orderViewModel);
		}

		[HttpPost]
		public ActionResult Update(ViewOrder update)
		{
			var orderAccessoriesDetails = _context.OrderAccessoryDetails
                .Include(x => x.Order.Customer)
                .Include(x => x.Order)
				.Include(x => x.Accessory)
                .Include(x => x.Coupon)
				.Where(x => x.OrderId == update.OrderId && x.DeleteFlag == false).ToList();
			if (orderAccessoriesDetails == null)
			{
				return RedirectToAction("RecordNotFound");
			}



            foreach (var item in orderAccessoriesDetails)
            {
                item.Quantity = update.OrderAccessories.FirstOrDefault(x => x.OrderAccessoryId == item.OrderAccessoryId).Quantity;
                var percent = (decimal)(item.Coupon == null ? 0 : item.Coupon.PercentDiscount);
				item.TotalPrice = (item.Quantity * item.Accessory.ExportPrice) - (item.Quantity * item.Accessory.ExportPrice * (percent/100));
			}   

			_context.OrderAccessoryDetails.UpdateRange(orderAccessoriesDetails);
			 _context.SaveChangesAsync();
            var id = update.OrderId;

            return RedirectToAction("Detail", new { id }); 
            
		}


	}

}
