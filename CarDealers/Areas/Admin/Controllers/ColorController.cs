using CarDealers.Areas.Admin.Models.BookingServiceModel;
using CarDealers.Areas.Admin.Models.CustomerViewModel;
using CarDealers.Entity;
using CarDealers.Models;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class ColorController : CustomController
    {
        private readonly CarDealersContext _context;

        public ColorController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListColor(int? page, int? pageSize)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 10; // Set a default number of records per page
            int pageNumber = page ?? 1;
            int recordsPerPage = pageSize ?? defaultPageSize;

            var colors = _context.Colors.Where(x => x.DeleteFlag == false)
            .OrderBy(p => p.ColorId)
            .Skip((pageNumber - 1) * recordsPerPage)
            .Take(recordsPerPage)
            .ToList();

            int totalRecords = _context.Colors.Where(x => x.DeleteFlag == false).Count();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = totalRecords;

            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed

            return View(colors);
        }

        public IActionResult SearchColor(int? page, int? pageSize, string Keyword)
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            int defaultPageSize = 2; // Set a default number of records per page
            int pageNumber = page ?? 1;
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.Colors.Where(x => x.DeleteFlag == false).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.ColorName.ToLower().Contains(Keyword.Trim().ToLower()) 
                || Keyword.Trim().ToLower().Contains(u.ColorName.ToLower()) 
                || keywords.Any(e => u.ColorName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var colors = query.Skip((pageNumber - 1) * recordsPerPage)
            .Take(recordsPerPage)
            .ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = _context.Colors.Where(x => x.DeleteFlag == false).Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListColor", colors);
        }
        public ActionResult CreateColor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateColor(Entity.Color model)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            if (ModelState.IsValid)
            {
                var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
                if (colors.Any(x => x.ColorName.Equals(model.ColorName)))
                {
                    ModelState.AddModelError("ColorName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                var Color = new Entity.Color
                { 
                    ColorName = model.ColorName,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                _context.Colors.Add(Color);
                _context.SaveChanges();
            }
            return RedirectToAction("ListColor");
        }

        [HttpGet]
        public IActionResult DeleteColor(int id)
        {
            if (ModelState.IsValid)
            {
                var color = _context.Colors.Where(x => x.ColorId == id).FirstOrDefault();
                color.DeleteFlag = true;
                _context.Colors.Update(color);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListColor");
        }

        [HttpGet]
        public ActionResult UpdateColor(int id)
        {
            if (ModelState.IsValid)
            {
                var color = _context.Colors.Where(x => x.ColorId == id && x.DeleteFlag == false).FirstOrDefault();

                if (color == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(color);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        public ActionResult UpdateColor(Entity.Color model)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            if (ModelState.IsValid)
            {
                var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
                var color = colors.Where(x => x.ColorId == model.ColorId).FirstOrDefault();

                if (color == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                if (colors.Any(x => x.ColorName.Equals(model.ColorName) && x.ColorId != model.ColorId))
                {
                    ModelState.AddModelError("ColorName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                color.ColorName = model.ColorName;
                color.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                color.ModifiedOn = DateTime.Now;

                _context.Colors.Update(color);
                _context.SaveChanges();
            }
            return RedirectToAction("ListColor");
        }

        [HttpPost]
        public IActionResult DeleteListColor(List<int> selectedIds)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var color = _context.Colors.Find(id);
                    if (color != null)
                    {
                        color.DeleteFlag = true;
                        color.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        color.ModifiedOn = DateTime.Now;

                        _context.Colors.Update(color);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListColor");
        }

    }
}
