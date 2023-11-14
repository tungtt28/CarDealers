
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
    public class NewsTypeController : CustomController
    {
        //create dbContext variable
        private readonly CarDealersContext _context;

        public NewsTypeController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public ActionResult CreateNewsType()
        {
            return View();
        }


        public IActionResult ListNewsType(int? page, string Keyword, int? pageSize)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();
            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.NewsTypeName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.NewsTypeName.ToLower()) || keywords.Any(e => u.NewsTypeName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }

            var newTypes = query1.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query1.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListNewsType", newTypes);
        }



        [HttpPost]
        public IActionResult CreateNewsType(NewsType model)
        {
            var newsType = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();
            if (newsType.Any(x => x.NewsTypeName.Equals(model.NewsTypeName)))
            {
                ModelState.AddModelError("NewsTypeName", "The name already exists. Please choose a different name.");
                return View(model);
            }
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var newsTypes = new NewsType { 
                NewsTypeName = model.NewsTypeName,
                CreatedBy = customerId != null ? int.Parse(customerId) : null,
                CreatedOn = DateTime.Now,
            };
            _context.NewsTypes.Add(newsTypes);
            _context.SaveChanges();

            return RedirectToAction("ListNewsType");
        }

        [HttpGet]
        public IActionResult DeleteNewsType(int id)
        {
            if (ModelState.IsValid)
            {
                var newsType = _context.NewsTypes.Where(x => x.NewsTypeId == id).FirstOrDefault();
                newsType.DeleteFlag = true;
                _context.NewsTypes.Update(newsType);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListNewsType");
        }

        [HttpGet]
        public ActionResult UpdateNewsType(int id)
        {
            if (ModelState.IsValid)
            {
                var newsType = _context.NewsTypes.Where(x => x.NewsTypeId == id && x.DeleteFlag == false).FirstOrDefault();

                if (newsType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(newsType);
            }
            return RedirectToAction("RecordNotFound");
        }
        [HttpPost]
        public ActionResult UpdateNewsType(NewsType model)
        {
            if (ModelState.IsValid)
            {
                var newsTypes = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();
                var newType = newsTypes.Where(x => x.NewsTypeId == model.NewsTypeId).FirstOrDefault();

                if (newType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                if (newsTypes.Any(x => x.NewsTypeName.Equals(model.NewsTypeName) && x.NewsTypeId != model.NewsTypeId))
                {
                    ModelState.AddModelError("NewsTypeName", "The name already exists. Please choose a different name.");
                    return View(model);
                }


                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                newType.NewsTypeName = model.NewsTypeName;
                newType.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                newType.ModifiedOn = DateTime.Now;
                _context.NewsTypes.Update(newType);
                _context.SaveChanges();
            }
            return RedirectToAction("ListNewsType");
        }

        [HttpPost]
        public IActionResult DeleteListNewsType(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var newsType = _context.NewsTypes.Find(id);
                    if (newsType != null)
                    {
                        newsType.DeleteFlag = true;
                        _context.NewsTypes.Update(newsType);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListNewsType");
        }


    }

}

