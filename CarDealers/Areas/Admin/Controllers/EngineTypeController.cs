
using Azure.Core;
using CarDealers.Entity;
using CarDealers.Models;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Reflection.Emit;
using System.Threading;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class EngineTypeController : CustomController
    {
        //create dbContext variable
        private readonly CarDealersContext _context;

        public EngineTypeController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListEngineType(int? page, int? pageSize, string Keyword)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.EngineTypeName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.EngineTypeName.ToLower()) || keywords.Any(e => u.EngineTypeName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }

            var query = query1.AsQueryable();
            var engineTypes = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListEngineType", engineTypes);
        }

        public ActionResult CreateEngineType()
        {
            return View();
        }



        [HttpPost]
        public IActionResult CreateEngineType(EngineType model)
        {
            var engineType = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
            if (engineType.Any(x => x.EngineTypeName.Equals(model.EngineTypeName)))
            {
                ModelState.AddModelError("EngineTypeName", "The name already exists. Please choose a different name.");
                return View(model);
            }


            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);


                var ennegitypename = new EngineType 
            { 
                EngineTypeName = model.EngineTypeName, 
                Description =model.Description,
                CreatedBy = customerId != null ? int.Parse(customerId) : null,
                CreatedOn = DateTime.Now,
             };
            _context.EngineTypes.Add(ennegitypename);
            _context.SaveChanges();

            return RedirectToAction("ListEngineType");
        }

        [HttpGet]
        public IActionResult DeleteEngineType(int id)
        {
            if (ModelState.IsValid)
            {
                var engineType = _context.EngineTypes.Where(x => x.EngineTypeId == id).FirstOrDefault();
                engineType.DeleteFlag = true;
                _context.EngineTypes.Update(engineType);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListEngineType");
        }

        [HttpGet]
        public ActionResult UpdateEngineType(int id)
        {
            if (ModelState.IsValid)
            {
                var engineType = _context.EngineTypes.Where(x => x.EngineTypeId == id && x.DeleteFlag == false).FirstOrDefault();

                if (engineType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(engineType);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        public ActionResult UpdateEngineType(EngineType model)
        {
            if (ModelState.IsValid)
            {
                var engineTypes = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
                var engineType = engineTypes.Where(x => x.EngineTypeId == model.EngineTypeId).FirstOrDefault();

                if (engineType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                if (engineTypes.Any(x => x.EngineTypeName.Equals(model.EngineTypeName) && x.EngineTypeId != model.EngineTypeId))
                {
                    ModelState.AddModelError("EngineTypeName", "The name already exists. Please choose a different name.");
                    return View(model);
                }
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);



                engineType.EngineTypeName = model.EngineTypeName;
                engineType.Description = model.Description;
                engineType.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                engineType.ModifiedOn = DateTime.Now;
                _context.EngineTypes.Update(engineType);
                _context.SaveChanges();
            }
            return RedirectToAction("ListEngineType");
        }

        [HttpPost]
        public IActionResult DeleteListEngineType(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                ModelState.AddModelError(string.Empty, "");
                return RedirectToAction("ListEngineType");
            }

            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var engineType = _context.EngineTypes.Find(id);
                    if (engineType != null)
                    {
                        engineType.DeleteFlag = true;
                        _context.EngineTypes.Update(engineType);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListEngineType");
        }


    }

    }

