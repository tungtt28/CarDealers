using Microsoft.AspNetCore.Mvc;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.EntityFrameworkCore;
using CarDealers.Models.AccessoriesPageViewModel;
using Newtonsoft.Json;

namespace CarDealers.Controllers
{
    public class AccessoriesPageController : CustomController
    {
        CarDealersContext dbContext = new CarDealersContext();

        public AccessoriesPageController(CarDealersContext dbContext)
        {
            this.dbContext = dbContext;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE, Constant.GUEST_ROLE };
        }

        public IActionResult Accessories(int? page)
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            int pageSize = 6; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1

            //fetch data from database table orders
            var accessoriesList = dbContext.AutoAccessories.Where(x => x.DeleteFlag == false)
            .OrderBy(p => p.AccessoryId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var accessoriesSelected= accessoriesList.Select(x => new ListAccessoriesPageViewModel
            {
                AccessoryId = x.AccessoryId,
                Quantity = x.Quantity.ToString("N0"),
                AccessoryName = x.AccessoryName,
                ImportPrice = x.ImportPrice?.ToString("N0"),
                ExportPrice = x.ExportPrice?.ToString("N0").Replace(",","."),
                Description = x.Description,
                Image = x.Image,
                Status = x.Status
            }).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = dbContext.AutoAccessories.Count();
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View(accessoriesSelected);
        }

        [HttpGet]
        public ActionResult ViewDetails(int id)
        {
            if (ModelState.IsValid)
            {
                var accessories = dbContext.AutoAccessories.Where(x => x.AccessoryId == id && x.DeleteFlag == false)
            .OrderBy(p => p.AccessoryId).FirstOrDefault();

                if (accessories == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                var TopAccessories = GetTopAccessories();
                ViewBag.TopAccessories = TopAccessories;
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return View(accessories);
            }
            return RedirectToAction("RecordNotFound");
        }

        public IActionResult SearchAccessories(int? page, string Keyword)
        {
            int pageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1

            //fetch data from database table orders
            var query1 = dbContext.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
            var query2 = query1.Select(x => new ListAccessoriesPageViewModel
            {
                AccessoryId = x.AccessoryId,
                Quantity = x.Quantity.ToString("N0"),
                AccessoryName = x.AccessoryName,
                ImportPrice = x.ImportPrice?.ToString("N0"),
                ExportPrice = x.ExportPrice?.ToString("N0").Replace(",", "."),
                Description = x.Description,
                Image = x.Image,
                Status = x.Status
            }).ToList();
            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query2 = query2.Where(u => u.AccessoryName.ToLower().Contains(Keyword.Trim().ToLower()) ||
                Keyword.Trim().ToLower().Contains(u.AccessoryName.ToLower()) ||
                keywords.Any(e => u.AccessoryName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query2.AsQueryable();
            var titles = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = dbContext.AutoAccessories.Where(x => x.DeleteFlag == false).Count();
            ViewBag.Keyword = Keyword;
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View("Accessories", titles);
        }
        public List<AutoAccessory> GetTopAccessories()
        {
            //var bestAccessoriesList = dbContext.OrderAccessoryDetails
            //    .GroupBy(o => o.AccessoryId)
            //    .Select(group => new
            //    {
            //        AccessoryId = group.Key,
            //        OrderAccessoryDetailsCount = group.Count() // Đếm số lượng OrderDetails trong mỗi nhóm
            //    })
            //    .OrderByDescending(x => x.OrderAccessoryDetailsCount)
            //    .Select(x => x.AccessoryId)
            //    .Take(3);
            //// Lấy danh sách thông tin Description, Image, và ExportPrice cho các CarId trong bestSellingCarList
            //var topAccessories = dbContext.AutoAccessories
            //    .Where(c => bestAccessoriesList.Contains(c.AccessoryId))
            //    .Select(c => new
            //    {
            //        c.Image,
            //        c.AccessoryName,
            //    })
            //    .ToList();

            var latestAccessories = dbContext.AutoAccessories.OrderByDescending(n => n.AccessoryId).Take(3).ToList();
            return latestAccessories;
        }

        protected List<News> GetDefaultFooterText()
        {
            var footers = dbContext.News.Include(e => e.NewsType).Where(e => e.DeleteFlag == false && e.NewsType.NewsTypeName.Equals(Constant.FOOTER)).OrderBy(e => e.Order).ToList();
            return footers;
        }

        protected List<News> GetDefaultMenu()
        {
            var menus = dbContext.News.Include(e => e.NewsType).Where(e => e.DeleteFlag == false && e.NewsType.NewsTypeName.Equals(Constant.MENU)).OrderBy(e => e.Order).ToList();
            return menus;
        }

        // Key lưu chuỗi json của Cart
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

        /// Thêm sản phẩm vào cart
        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = dbContext.AutoAccessories
                .Where(p => p.AccessoryId == productid)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.accessory.AccessoryId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                //  Thêm mới
                cart.Add(new CartItem() { quantity = 1, accessory = product });
            }

            // Lưu cart vào Session
            SaveCartSession(cart);
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }

        /// Cập nhật
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.accessory.AccessoryId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity = quantity;
            }
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.accessory.AccessoryId == productid);
            if (cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }
    }
}
