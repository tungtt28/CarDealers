using CarDealers.Areas.Admin.Models.OrderAccessoriesViewModel;
using CarDealers.Areas.Admin.Models.OrderCarViewModel;
using CarDealers.Controllers;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using static CarDealers.Areas.Admin.Controllers.SendEmailController;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using IronPdf.Extensions.Mvc.Core;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Grpc.Core;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class OrderCarController : CustomController
    {
        private readonly CarDealersContext _context;

        public OrderCarController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListOrderCar()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            var orders = _context.Orders.Where(o => o.DeleteFlag == false).ToList();
            var orderSelectList = orders.Select(order => new SelectListItem
            {
                Value = order.OrderId.ToString(),
                Text = order.OrderId.ToString()
            }).ToList();
            ViewBag.OrderOptions = orderSelectList;

            var cars = _context.Cars.Where(x => x.DeleteFlag == false).ToList();
            var carTypeSelectList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model
            }).ToList();
            ViewBag.CarOptions = carTypeSelectList;

            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
            var colorTypeSelectList = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName
            }).ToList();
            ViewBag.ColorOptions = colorTypeSelectList;

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
            var statusList = new UpdateOrderCarViewModel().StatusList;
            var ordercars = _context.OrderDetails.Where(x => x.DeleteFlag == false)
                .Include(x => x.Order.Customer)
                .Include(x => x.Order)
                .Include(x => x.Coupon)
                .Include(x => x.Car)
                .Include(x => x.Color)
                .ToList();

            var ordernew = ordercars?.Select(x => new ListOrderCarViewModel
            {
                OrderId = x.OrderId,
                FullName = x.Order.Customer.FullName,
                Email = x.Order.Customer.Email,
                PhoneNumber = x.Order.Customer.PhoneNumber,
                Coupon = x.Coupon?.Name,
                SellerName = x.SellerId.HasValue ? x.Seller?.FullName : null,
                Status = statusList.FirstOrDefault(s => s.Value.Equals(x.Order.Status.ToString())).Text,
            }).ToList();

            return View(ordernew);
        }

        [HttpGet]
        public IActionResult CreateOrderCarGuest()
        {
            var viewModel = new CreateOrderCarGuestViewModel();
            var orders = _context.Orders.Where(o => o.DeleteFlag == false).ToList();
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).ToList();
            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            var roleEmployee = _context.UserRoles.Where(r => r.RoleName.ToLower().Equals("employee")).FirstOrDefault();
            int roleEmployeeId = roleEmployee.UserRoleId;
            var sellers = _context.Users.Where(x => x.DeleteFlag == false && x.UserRoleId == roleEmployeeId).ToList();

            viewModel.OrderList = orders.Select(order => new SelectListItem
            {
                Value = order.OrderId.ToString(),
                Text = order.OrderId.ToString()
            });
            viewModel.CarList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model
            });
            viewModel.ColorList = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName
            });
            viewModel.CouponList = coupons.Select(coupon => new SelectListItem
            {
                Value = coupon.CouponId.ToString(),
                Text = coupon.Name
            });
            viewModel.SellerList = sellers.Select(seller => new SelectListItem
            {
                Value = seller.UserId.ToString(),
                Text = seller.Username
            });

            viewModel.Quantity = 1;
            return View(viewModel);
        }
        [HttpGet]
        public IActionResult CreateOrderCarCustomer()
        {
            var viewModel = new CreateOrderCarCustomerViewModel();
            var orders = _context.Orders.Where(o => o.DeleteFlag == false).ToList();
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).ToList();
            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            var roleEmployee = _context.UserRoles.Where(r => r.RoleName.ToLower().Equals("employee")).FirstOrDefault();
            int roleEmployeeId = roleEmployee.UserRoleId;
            var sellers = _context.Users.Where(x => x.DeleteFlag == false && x.UserRoleId == roleEmployeeId).ToList();
            var customers = _context.Customers.Where(x => x.DeleteFlag == false && x.CustomerType == 2).ToList();

            viewModel.OrderList = orders.Select(order => new SelectListItem
            {
                Value = order.OrderId.ToString(),
                Text = order.OrderId.ToString()
            });
            viewModel.CarList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model
            });
            viewModel.ColorList = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName
            });
            viewModel.CouponList = coupons.Select(coupon => new SelectListItem
            {
                Value = coupon.CouponId.ToString(),
                Text = coupon.Name
            });
            viewModel.SellerList = sellers.Select(seller => new SelectListItem
            {
                Value = seller.UserId.ToString(),
                Text = seller.Username
            });
            viewModel.CustomerList = customers.Select(customers => new SelectListItem
            {
                Value = customers.CustomerId.ToString(),
                Text = customers.FullName
            });
            viewModel.Quantity = 1;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrderCarGuest(CreateOrderCarGuestViewModel model)
        {
            var orders = _context.Orders.Where(o => o.DeleteFlag == false).ToList();
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).ToList();
            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            var roleEmployee = _context.UserRoles.Where(r => r.RoleName.ToLower().Equals("employee")).FirstOrDefault();
            int roleEmployeeId = roleEmployee.UserRoleId;
            var sellers = _context.Users.Where(x => x.DeleteFlag == false && x.UserRoleId == roleEmployeeId).ToList();
            var customerAll = _context.Customers.ToList();
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

            model.OrderList = orders.Select(order => new SelectListItem
            {
                Value = order.OrderId.ToString(),
                Text = order.OrderId.ToString()
            });

            model.CarList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model
            });

            model.ColorList = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName
            });
            model.CouponList = coupons.Select(coupon => new SelectListItem
            {
                Value = coupon.CouponId.ToString(),
                Text = coupon.Name
            });
            model.SellerList = sellers.Select(seller => new SelectListItem
            {
                Value = seller.UserId.ToString(),
                Text = seller.Username
            });
            bool fail = false;
            //car
            if (model.CarList.IsNullOrEmpty())
            {
                ModelState.AddModelError("CarId", "Car cannot empty.");
                fail = true;
            }
            //color
            if (model.ColorList.IsNullOrEmpty())
            {
                ModelState.AddModelError("ColorId", "Color cannot empty.");
                fail = true;
            }
            //seller
            if (model.SellerList.IsNullOrEmpty())
            {
                ModelState.AddModelError("SellerId", "SellerId cannot empty.");
                fail = true;
            }
            //name
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
            //check if return or not
            if (fail == true)
            {
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var orderCars = _context.OrderDetails.Where(x => x.DeleteFlag == false).ToList();

                var customer = new Customer
                {
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    FullName = model.FullName,
                    CustomerType = 1,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();

                var newOrder = new Order
                {
                    CustomerId = customer.CustomerId,
                    OrderDate = DateTime.Now,
                    Status = model.Status,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                var newOrderCar = new OrderDetail
                {
                    OrderId = newOrder.OrderId,
                    CarId = model.CarId,
                    ColorId = model.ColorId,
                    CouponId = model.CouponId,
                    SellerId = model.SellerId,
                    Quantity = model.Quantity,
                    TotalPrice = model.Quantity * _context.Cars.Where(x => x.CarId == model.CarId).FirstOrDefault()?.ExportPrice,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.OrderDetails.AddAsync(newOrderCar);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListOrderCar");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrderCarCustomer(CreateOrderCarCustomerViewModel model)
        {
            var orders = _context.Orders.Where(o => o.DeleteFlag == false).ToList();
            var cars = _context.Cars.Where(x => x.DeleteFlag == false).ToList();
            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
            var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            var roleEmployee = _context.UserRoles.Where(r => r.RoleName.ToLower().Equals("employee")).FirstOrDefault();
            int roleEmployeeId = roleEmployee.UserRoleId;
            var sellers = _context.Users.Where(x => x.DeleteFlag == false && x.UserRoleId == roleEmployeeId).ToList();
            var customers = _context.Customers.Where(x => x.DeleteFlag == false && x.CustomerType == 2).ToList();
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

            model.OrderList = orders.Select(order => new SelectListItem
            {
                Value = order.OrderId.ToString(),
                Text = order.OrderId.ToString()
            });

            model.CarList = cars.Select(car => new SelectListItem
            {
                Value = car.CarId.ToString(),
                Text = car.Model
            });

            model.ColorList = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName
            });

            model.CouponList = coupons.Select(coupon => new SelectListItem
            {
                Value = coupon.CouponId.ToString(),
                Text = coupon.Name
            });
            model.SellerList = sellers.Select(seller => new SelectListItem
            {
                Value = seller.UserId.ToString(),
                Text = seller.Username
            });
            model.CustomerList = customers.Select(customers => new SelectListItem
            {
                Value = customers.CustomerId.ToString(),
                Text = customers.FullName
            });
            bool fail = false;
            //car
            if (model.CarList.IsNullOrEmpty())
            {
                ModelState.AddModelError("CarId", "Car cannot empty.");
                fail = true;
            }
            //color
            if (model.ColorList.IsNullOrEmpty())
            {
                ModelState.AddModelError("ColorId", "Color cannot empty.");
                fail = true;
            }
            //seller
            if (model.SellerList.IsNullOrEmpty())
            {
                ModelState.AddModelError("SellerId", "SellerId cannot empty.");
                fail = true;
            }
            //customer
            if (model.CarList.IsNullOrEmpty())
            {
                ModelState.AddModelError("CustomerId", "Customer cannot empty.");
                fail = true;
            }
            if (orders.Any(o => o.CustomerId == model.CustomerId && o.Status != 5))//check if with same customer, have or not unfinished order
            {
                ModelState.AddModelError("CustomerId", "The Customer have uncomplete order.");
                fail = true;
            }
            //check if return or not
            if (fail == true)
            {
                return View(model);

            }

            if (ModelState.IsValid)
            {
                var orderCars = _context.OrderDetails.Where(x => x.DeleteFlag == false).ToList();

                var newOrder = new Order
                {
                    CustomerId = model.CustomerId,
                    OrderDate = DateTime.Now,
                    Status = model.Status,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.Orders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                var newOrderCar = new OrderDetail
                {
                    OrderId = newOrder.OrderId,
                    CarId = model.CarId,
                    ColorId = model.ColorId,
                    CouponId = model.CouponId,
                    SellerId = model.SellerId,
                    Quantity = model.Quantity,
                    TotalPrice = model.Quantity * _context.Cars.Where(x => x.CarId == model.CarId).FirstOrDefault()?.ExportPrice,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.OrderDetails.AddAsync(newOrderCar);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListOrderCar");
            }
            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> UpdateOrderCar(int id)
        {
            if (ModelState.IsValid)
            {
                var order = _context.Orders.Where(x => x.OrderId == id && x.DeleteFlag == false).FirstOrDefault();
                var orderDetail = _context.OrderDetails.Where(x => x.OrderId == order.OrderId && x.DeleteFlag == false).FirstOrDefault();
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

                var orders = _context.Orders.ToList();
                var car = _context.Cars.Where(x => x.CarId == orderDetail.CarId).FirstOrDefault();
                var color = _context.Colors.Where(x => x.ColorId == orderDetail.ColorId).FirstOrDefault();
                var coupon = _context.Coupons.Where(x => x.CouponId == orderDetail.CouponId).FirstOrDefault();
                var roleEmployee = _context.UserRoles.Where(r => r.RoleName.ToLower().Equals("employee")).FirstOrDefault();
                int roleEmployeeId = roleEmployee.UserRoleId;
                var seller = _context.Users.Where(x => x.UserId == orderDetail.SellerId).FirstOrDefault();
                var customer = _context.Customers.Where(x => x.CustomerId == order.CustomerId).FirstOrDefault();

                var statusList = new UpdateOrderCarViewModel().StatusList;

                var updateOrderCarViewModel = new UpdateOrderCarViewModel
                {
                    OrderId = orderDetail.OrderId,
                    OrderDetailId = orderDetail.OrderDetailId,
                    CarModel = car?.Model + " - ID(" + orderDetail.CarId + ")",
                    Color = color?.ColorName + " - ID(" + color?.ColorId + ")",
                    Coupon = coupon?.Name != null ? coupon?.Name + " - ID(" + coupon?.CouponId + ")" : "No coupon",
                    SellerName = seller?.FullName + " - ID(" + seller?.UserId + ")",
                    CustomerName = customer?.FullName + " - ID(" + customer?.CustomerId + ")",
                    Quantity = orderDetail.Quantity,
                    TotalPrice = orderDetail.TotalPrice,
                    Status = order.Status,
                };
                if (seller?.FullName == null)
                {
                    var sellers = _context.Users.Where(x => x.DeleteFlag == false && x.UserRoleId == roleEmployeeId).ToList();
                    updateOrderCarViewModel.SellerList = sellers.Select(x => new SelectListItem
                    {
                        Value = x.UserId.ToString(),
                        Text = x.Username,
                        Selected = customerId.Equals(x.UserId.ToString())
                    });
                }
                updateOrderCarViewModel.StatusList = updateOrderCarViewModel.StatusList.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text,
                    Selected = order.Status.ToString().Equals(x.Value)
                });
                ViewBag.seller = seller?.FullName;
                return View(updateOrderCarViewModel);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrderCar(UpdateOrderCarViewModel model)
        {
            var order = _context.Orders.Where(x => x.OrderId == model.OrderId).FirstOrDefault();
            var orderDetail = _context.OrderDetails.Where(x => x.OrderId == model.OrderId).FirstOrDefault();
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

            if (model.SellerId != null)
            {
                orderDetail.SellerId = model.SellerId;
                orderDetail.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                orderDetail.ModifiedOn = DateTime.Now;
                _context.OrderDetails.Update(orderDetail);
            }

            if (ModelState.IsValid)
            {
                order.Status = model.Status;
                order.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                order.ModifiedOn = DateTime.Now;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                return RedirectToAction("ListOrderCar");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteListOrderCar(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                // Tiến hành xóa các bản ghi có ID nằm trong danh sách selectedIds
                foreach (var id in selectedIds)
                {
                    var order = _context.Orders.Find(id);
                    var orderDetail = _context.OrderDetails.Where(x => x.OrderId == order.OrderId).FirstOrDefault();
                    if (orderDetail != null)
                    {
                        orderDetail.DeleteFlag = true;
                        orderDetail.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        orderDetail.ModifiedOn = DateTime.Now;
                        _context.OrderDetails.Update(orderDetail);

                        order.DeleteFlag = true;
                        order.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        order.ModifiedOn = DateTime.Now;
                        _context.Orders.Update(order);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListOrderCar");
        }

        public ActionResult ViewOrder(int id)
        {
            var order = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.OrderId == id);
            if (order == null)
            {
                return RedirectToAction("RecordNotFound");
            }
            var orderDetail = _context.OrderDetails.Where(x => x.OrderId == order.OrderId).FirstOrDefault();
            var car = _context.Cars.Where(x => x.CarId == orderDetail.CarId).FirstOrDefault();
            var couponDiscount = _context.Coupons.Where(x => x.CouponId == orderDetail.CouponId).FirstOrDefault()?.PercentDiscount;
            if (couponDiscount == null) { couponDiscount = 0; }
            var totalPriceAfterDiscount = orderDetail.TotalPrice - (orderDetail.TotalPrice * decimal.Parse((couponDiscount / 100).ToString()));
            var depositeAfterDiscount = totalPriceAfterDiscount * decimal.Parse((0.15).ToString());
            var totalPrice = totalPriceAfterDiscount - depositeAfterDiscount;
            var orderViewModel = new ViewOrderCarViewModel
            {
                OrderId = order.OrderId,
                FullName = order.Customer.FullName,
                PhoneNumber = order.Customer.PhoneNumber,
                Email = order.Customer.Email,
                Quantity = orderDetail.Quantity.ToString(),
                Price = car.ExportPrice?.ToString("N0").Replace(",", "."),
                Discount = couponDiscount + "%",
                TotalPrice = totalPrice?.ToString("N0").Replace(",", "."),
                CreatedOn = order.CreatedOn,
                Deposite = depositeAfterDiscount == null ? "" : depositeAfterDiscount?.ToString("N0").Replace(",", "."),
                Status = order.Status,
            };

            return View(orderViewModel);
        }
        public IActionResult SavePDF(int id)
        {
            var order = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.OrderId == id);
            var orderDetail = _context.OrderDetails.Where(x => x.OrderId == order.OrderId).FirstOrDefault();
            var car = _context.Cars.Where(x => x.CarId == orderDetail.CarId).FirstOrDefault();
            var couponDiscount = _context.Coupons.Where(x => x.CouponId == orderDetail.CouponId).FirstOrDefault()?.PercentDiscount;
            if (couponDiscount == null) { couponDiscount = 0; }
            var totalPriceAfterDiscount = orderDetail.TotalPrice - (orderDetail.TotalPrice * decimal.Parse((couponDiscount / 100).ToString()));
            var depositeAfterDiscount = totalPriceAfterDiscount * decimal.Parse((0.15).ToString());
            var totalPrice = totalPriceAfterDiscount - depositeAfterDiscount;
            var model = new ViewOrderCarViewModel
            {
                OrderId = order.OrderId,
                FullName = order.Customer.FullName,
                PhoneNumber = order.Customer.PhoneNumber,
                Email = order.Customer.Email,
                Quantity = orderDetail.Quantity.ToString(),
                Price = car.ExportPrice?.ToString("N0").Replace(",", "."),
                Discount = couponDiscount + "%",
                TotalPrice = totalPrice?.ToString("N0").Replace(",", "."),
                CreatedOn = order.CreatedOn,
                Deposite = depositeAfterDiscount == null ? "" : depositeAfterDiscount?.ToString("N0").Replace(",", "."),
                Status = order.Status,
            };
            var renderer = new ChromePdfRenderer();

            var fileName = "OrderCar" + order.OrderId + ".pdf";

            if (order.Status == 0 || order.Status == 1)
            {
                var pdf = renderer.RenderHtmlAsPdf("\r\n<div class=\"content-wrap\">\r\n    <div class=\"col-lg-12\">\r\n        <div class=\"card\">\r\n            <div class=\"table-responsive\">\r\n                <div class=\"table-wrapper\">\r\n                    <div style=\"display: flex; align-items:center;justify-content:space-between\">\r\n                        <div>\r\n                            <h1>CAR DEALERS</h1>\r\n                        </div>\r\n                        <div>\r\n                            <p style=\"margin-bottom:unset\">Đc: CAR DEALERS</p>\r\n                            <p style=\"margin-bottom:unset\">ĐT: 0338770668</p>\r\n                        </div>\r\n                    </div>\r\n                    <div style=\"margin-left\">                            " +
                    "<h2>ORDER: # " + model.OrderId + "</h2>\r\n                    </div>\r\n                    <div style=\"margin-right\">\r\n                        \r\n                            " +
                    "<p>Ngày: " + model.CreatedOn + "</p>\r\n                    </div>\r\n                    <h2>THÔNG TIN KHÁCH HÀNG</h2>\r\n                    " + "<p>Họ và tên : " + model.FullName + " </p>\r\n                    " +
                    "<p>Điện thoại : " + model.PhoneNumber + "</p>\r\n                    <h2>THÔNG TIN ĐẶT CỌC ĐƠN HÀNG</h2>\r\n                    <table border=\"1\">\r\n                        <thead>\r\n                            <tr>\r\n                                <th>Tên xe</th>\r\n                                <th>Đơn giá tính theo VNĐ</th>\r\n                                <th>Số lượng</th>\r\n                                <th>Giảm giá</th>\r\n                                <th>Đặt cọc 15%</th>\r\n                            </tr>\r\n                        </thead>\r\n                        <tbody>\r\n                            <tr>\r\n                                " +
                    "<td>" + model.FullName + "</td>\r\n                                " +
                    "<td>" + model.Price + " VND</td>\r\n                                " +
                    "<td>" + model.Quantity + "</td>\r\n                                " +
                    "<td>" + model.Discount + "</td>\r\n                                " +
                    "<td>" + model.Deposite + " VND</td>\r\n                            </tr>\r\n                        </tbody>\r\n                    </table>\r\n                    " +
                    "<p style=\"margin-top:6px;float:unset\">Thành tiền: " + model.Deposite + " VND</p>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>");
                pdf.SaveAs(fileName);
            }
            else
            {
                var pdf = renderer.RenderHtmlAsPdf("\r\n<div class=\"content-wrap\">\r\n    <div class=\"col-lg-12\">\r\n        <div class=\"card\">\r\n            <div class=\"table-responsive\">\r\n                <div class=\"table-wrapper\">\r\n                    <div style=\"display: flex; align-items:center;justify-content:space-between\">\r\n                        <div>\r\n                            <h1>CAR DEALERS</h1>\r\n                        </div>\r\n                        <div>\r\n                            <p style=\"margin-bottom:unset\">Đc: CAR DEALERS</p>\r\n                            <p style=\"margin-bottom:unset\">ĐT: 0338770668</p>\r\n                        </div>\r\n                    </div>\r\n                    <div style=\"margin-left\">                            " +
                    "<h2>ORDER: # " + model.OrderId + "</h2>\r\n                    </div>\r\n                    <div style=\"margin-right\">\r\n                        \r\n                            " +
                    "<p>Ngày: " + model.CreatedOn + "</p>\r\n                    </div>\r\n                    <h2>THÔNG TIN KHÁCH HÀNG</h2>\r\n                    " + "<p>Họ và tên : " + model.FullName + " </p>\r\n                    " +
                    "<p>Điện thoại : " + model.PhoneNumber + "</p>\r\n                    <h2>THÔNG TIN ĐƠN HÀNG</h2>\r\n                    <table border=\"1\">\r\n                        <thead>\r\n                            <tr>\r\n                                <th>Tên xe</th>\r\n                                <th>Đơn giá tính theo VNĐ</th>\r\n                                <th>Số lượng</th>\r\n                                <th>Giảm giá</th>\r\n                                <th>Đặt cọc 15%</th>\r\n                                <th>Thành tiền</th>\r\n                            </tr>\r\n                        </thead>\r\n                        <tbody>\r\n                            <tr>\r\n                                " +
                    "<td>" + model.FullName + "</td>\r\n                                " +
                    "<td>" + model.Price + "</td>\r\n                                " +
                    "<td>" + model.Quantity + "</td>\r\n                                " +
                    "<td>" + model.Discount + "</td>\r\n                                " +
                    "<td>" + model.Deposite + "</td>\r\n                                " +
                    "<td>" + model.TotalPrice + " VND</td>\r\n                            </tr>\r\n                        </tbody>\r\n                    </table>\r\n                    " +
                    "<p style=\"margin-top:6px;float:unset\">Thành tiền: " + model.TotalPrice + " VND</p>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>");
                pdf.SaveAs(fileName);
            }

            return RedirectToAction("ViewOrder", new { id = id });
        }
        public IActionResult SendEmail(String email, int id)
        {
            var order = _context.Orders.Include(x => x.Customer).FirstOrDefault(x => x.OrderId == id);
            var orderDetail = _context.OrderDetails.Where(x => x.OrderId == order.OrderId).FirstOrDefault();
            var car = _context.Cars.Where(x => x.CarId == orderDetail.CarId).FirstOrDefault();
            var couponDiscount = _context.Coupons.Where(x => x.CouponId == orderDetail.CouponId).FirstOrDefault()?.PercentDiscount;
            if (couponDiscount == null) { couponDiscount = 0; }
            var totalPriceAfterDiscount = orderDetail.TotalPrice - (orderDetail.TotalPrice * decimal.Parse((couponDiscount / 100).ToString()));
            var depositeAfterDiscount = totalPriceAfterDiscount * decimal.Parse((0.15).ToString());
            var totalPrice = totalPriceAfterDiscount - depositeAfterDiscount;
            var model = new ViewOrderCarViewModel
            {
                OrderId = order.OrderId,
                FullName = order.Customer.FullName,
                PhoneNumber = order.Customer.PhoneNumber,
                Email = order.Customer.Email,
                Quantity = orderDetail.Quantity.ToString(),
                Price = car.ExportPrice?.ToString("N0").Replace(",", "."),
                Discount = couponDiscount + "%",
                TotalPrice = totalPrice?.ToString("N0").Replace(",", "."),
                CreatedOn = order.CreatedOn,
                Deposite = depositeAfterDiscount == null ? "" : depositeAfterDiscount?.ToString("N0").Replace(",", "."),
                Status = order.Status,
            };
            var renderer = new ChromePdfRenderer();

            var fileName = "OrderCar" + order.OrderId + ".pdf";

            if (order.Status == 0 || order.Status == 1)
            {
                var pdf = renderer.RenderHtmlAsPdf("\r\n<div class=\"content-wrap\">\r\n    <div class=\"col-lg-12\">\r\n        <div class=\"card\">\r\n            <div class=\"table-responsive\">\r\n                <div class=\"table-wrapper\">\r\n                    <div style=\"display: flex; align-items:center;justify-content:space-between\">\r\n                        <div>\r\n                            <h1>CAR DEALERS</h1>\r\n                        </div>\r\n                        <div>\r\n                            <p style=\"margin-bottom:unset\">Đc: CAR DEALERS</p>\r\n                            <p style=\"margin-bottom:unset\">ĐT: 0338770668</p>\r\n                        </div>\r\n                    </div>\r\n                    <div style=\"margin-left\">                            " +
                    "<h2>ORDER: # " + model.OrderId + "</h2>\r\n                    </div>\r\n                    <div style=\"margin-right\">\r\n                        \r\n                            " +
                    "<p>Ngày: " + model.CreatedOn + "</p>\r\n                    </div>\r\n                    <h2>THÔNG TIN KHÁCH HÀNG</h2>\r\n                    " + "<p>Họ và tên : " + model.FullName + " </p>\r\n                    " +
                    "<p>Điện thoại : " + model.PhoneNumber + "</p>\r\n                    <h2>THÔNG TIN ĐẶT CỌC ĐƠN HÀNG</h2>\r\n                    <table border=\"1\">\r\n                        <thead>\r\n                            <tr>\r\n                                <th>Tên xe</th>\r\n                                <th>Đơn giá tính theo VNĐ</th>\r\n                                <th>Số lượng</th>\r\n                                <th>Giảm giá</th>\r\n                                <th>Đặt cọc 15%</th>\r\n                            </tr>\r\n                        </thead>\r\n                        <tbody>\r\n                            <tr>\r\n                                " +
                    "<td>" + model.FullName + "</td>\r\n                                " +
                    "<td>" + model.Price + " VND</td>\r\n                                " +
                    "<td>" + model.Quantity + "</td>\r\n                                " +
                    "<td>" + model.Discount + "</td>\r\n                                " +
                    "<td>" + model.Deposite + " VND</td>\r\n                            </tr>\r\n                        </tbody>\r\n                    </table>\r\n                    " +
                    "<p style=\"margin-top:6px;float:unset\">Thành tiền: " + model.Deposite + " VND</p>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>");
                pdf.SaveAs(fileName);
            }
            else
            {
                var pdf = renderer.RenderHtmlAsPdf("\r\n<div class=\"content-wrap\">\r\n    <div class=\"col-lg-12\">\r\n        <div class=\"card\">\r\n            <div class=\"table-responsive\">\r\n                <div class=\"table-wrapper\">\r\n                    <div style=\"display: flex; align-items:center;justify-content:space-between\">\r\n                        <div>\r\n                            <h1>CAR DEALERS</h1>\r\n                        </div>\r\n                        <div>\r\n                            <p style=\"margin-bottom:unset\">Đc: CAR DEALERS</p>\r\n                            <p style=\"margin-bottom:unset\">ĐT: 0338770668</p>\r\n                        </div>\r\n                    </div>\r\n                    <div style=\"margin-left\">                            " +
                    "<h2>ORDER: # " + model.OrderId + "</h2>\r\n                    </div>\r\n                    <div style=\"margin-right\">\r\n                        \r\n                            " +
                    "<p>Ngày: " + model.CreatedOn + "</p>\r\n                    </div>\r\n                    <h2>THÔNG TIN KHÁCH HÀNG</h2>\r\n                    " + "<p>Họ và tên : " + model.FullName + " </p>\r\n                    " +
                    "<p>Điện thoại : " + model.PhoneNumber + "</p>\r\n                    <h2>THÔNG TIN ĐƠN HÀNG</h2>\r\n                    <table border=\"1\">\r\n                        <thead>\r\n                            <tr>\r\n                                <th>Tên xe</th>\r\n                                <th>Đơn giá tính theo VNĐ</th>\r\n                                <th>Số lượng</th>\r\n                                <th>Giảm giá</th>\r\n                                <th>Đặt cọc 15%</th>\r\n                                <th>Thành tiền</th>\r\n                            </tr>\r\n                        </thead>\r\n                        <tbody>\r\n                            <tr>\r\n                                " +
                    "<td>" + model.FullName + "</td>\r\n                                " +
                    "<td>" + model.Price + "</td>\r\n                                " +
                    "<td>" + model.Quantity + "</td>\r\n                                " +
                    "<td>" + model.Discount + "</td>\r\n                                " +
                    "<td>" + model.Deposite + "</td>\r\n                                " +
                    "<td>" + model.TotalPrice + " VND</td>\r\n                            </tr>\r\n                        </tbody>\r\n                    </table>\r\n                    " +
                    "<p style=\"margin-top:6px;float:unset\">Thành tiền: " + model.TotalPrice + " VND</p>\r\n                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>");
                pdf.SaveAs(fileName);
            }


            var attachmentPath = fileName; // Replace with the actual path to your PDF file
            Attachment attachment = new Attachment(attachmentPath, System.Net.Mime.MediaTypeNames.Application.Pdf);
            //send email
            var customerEmail = new EmailModel()
            {
                To = email,
                Body = "Order of your car",
                Subject = "Order of your car"
            };

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("quochuycode0209@gmail.com", "mfus xank wtah rywh"),
                    EnableSsl = true,
                };

                using (var dbContext = new CarDealersContext())
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("quochuycode0209@gmail.com"),
                        Subject = customerEmail.Subject,
                        Body = customerEmail.Body,
                    };
                    mailMessage.To.Add(email);

                    mailMessage.Attachments.Add(attachment);//pdf order attachment

                    smtpClient.Send(mailMessage);
                }

                ViewBag.Message = "Email sent successfully";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to send email: {ex.Message}";
            }

            return RedirectToAction("ViewOrder", new { id = id });
        }


    }
}
