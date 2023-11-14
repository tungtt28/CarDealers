
using Azure.Core;
using CarDealers.Entity;
using CarDealers.Models;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Reflection.Emit;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class CouponsController : CustomController
    {
        //create dbContext variable
        private readonly CarDealersContext _context;

        public CouponsController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public ActionResult CreateCoupons()
        {
            return View();
        }


        public IActionResult ListCoupons(int? page, string Keyword, int? pageSize)
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.Name.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.Name.ToLower()) || keywords.Any(e => u.Name.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }

            var coupons = query1.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query1.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListCoupons", coupons);
        }



        [HttpPost]
        public IActionResult CreateCoupons(Coupon model)
        {
            var coupon = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
            if (coupon.Any(x => x.Name.Equals(model.Name)))
            {
                ModelState.AddModelError("CouponsName", "The name already exists. Please choose a different name.");
                return View(model);
            }

            if (model.DateStart > model.DateEnd)
            {
                ModelState.AddModelError("DateEnd", "The end date must be greater than the creation date !!");
                return View(model);
            }

            DateTime currentDate = DateTime.Now;
            bool status;
            if (currentDate > model.DateEnd || model.UsesTotal <= 0)
            {
                status = false;
            }else
            {
                status = true;
            }




            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var coupons = new Coupon
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description,
                DateStart = model.DateStart,
                DateEnd = model.DateEnd,
                UsesTotal = model.UsesTotal,
                DateAdded = model.DateAdded,
                PercentDiscount = model.PercentDiscount,
                Status = status,
                CreatedBy = customerId != null ? int.Parse(customerId) : null,
                CreatedOn = DateTime.Now,
            };
            _context.Coupons.Add(coupons);
            _context.SaveChanges();
            return RedirectToAction("ListCoupons");
        }

        [HttpGet]
        public IActionResult DeleteCoupons(int id)
        {
            if (ModelState.IsValid)
            {
                var coupons = _context.Coupons.Where(x => x.CouponId == id).FirstOrDefault();
                coupons.DeleteFlag = true;
                _context.Coupons.Update(coupons);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCoupons");
        }

        [HttpGet]
        public ActionResult UpdateCoupons(int id)
        {
            if (ModelState.IsValid)
            {

                var coupons = _context.Coupons.Where(x => x.CouponId == id && x.DeleteFlag == false).FirstOrDefault();

                if (coupons == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(coupons);
            }
            return RedirectToAction("RecordNotFound");
        }
        [HttpPost]
        public ActionResult UpdateCoupons(Coupon model)
        {
            if (ModelState.IsValid)
            {
                var coupons = _context.Coupons.Where(x => x.DeleteFlag == false).ToList();
                var coupon = coupons.Where(x => x.CouponId == model.CouponId).FirstOrDefault();

                if (coupon == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                if (coupons.Any(x => x.Name.Equals(model.Name) && x.CouponId != model.CouponId))
                {
                    ModelState.AddModelError("CouponsName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                if (model.DateStart > model.DateEnd)
                {
                    ModelState.AddModelError("DateEnd", "The end date must be greater than the creation date !!");
                    return View(model);
                }

                DateTime currentDate = DateTime.Now;
                bool status;
                if (currentDate > model.DateEnd || model.UsesTotal <= 0)
                {
                    status = false;
                    
                }
                else
                {
                    status = true;
                    
                }


                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                coupon.Name = model.Name;
                coupon.Code = model.Code;
                coupon.Description = model.Description;
                coupon.DateStart = model.DateStart;
                coupon.DateEnd = model.DateEnd;
                coupon.UsesTotal = model.UsesTotal;
                coupon.DateAdded = model.DateAdded;
                coupon.PercentDiscount = model.PercentDiscount;
                coupon.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                coupon.ModifiedOn = DateTime.Now;
                coupon.Status = status;

                _context.Coupons.Update(coupon);
                _context.SaveChanges();
            }
            return RedirectToAction("ListCoupons");
        }

        [HttpPost]
        public IActionResult DeleteListCoupons(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var coupons = _context.Coupons.Find(id);
                    if (coupons != null)
                    {
                        coupons.DeleteFlag = true;
                        _context.Coupons.Update(coupons);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCoupons");
        }


    }

}


