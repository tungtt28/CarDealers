using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class CarTypeController : CustomController
    {
        //create dbContext variable
        private readonly CarDealersContext _context;

        public CarTypeController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        [HttpGet]
        public IActionResult CreateCarType()
        {
            return View();
        }

        public IActionResult ListCarType(int? page, int? pageSize, string Keyword)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set a default number of records per page
            int pageNumber = page ?? 1;
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.TypeName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.TypeName.ToLower()) || keywords.Any(e => u.TypeName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var cartypes = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.PageSizeList = new List<int> { 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListCarType", cartypes);
        }

        [HttpPost]
        public IActionResult CreateCarType(CarType model)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var CarTypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
                if (CarTypes.Any(x => x.TypeName.Equals(model.TypeName)))
                {
                    ModelState.AddModelError("TypeName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                var CarType = new CarType 
                { 
                    TypeName = model.TypeName.ReplaceSpace().Trim(),
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                _context.CarTypes.Add(CarType);
                _context.SaveChanges();
            }
            return RedirectToAction("ListCarType");
        }

        [HttpGet]
        public IActionResult DeleteCarType(int id)
        {
            if (ModelState.IsValid)
            {
                var CarType = _context.CarTypes.Where(x => x.CarTypeId == id).FirstOrDefault();
                CarType.DeleteFlag = true;
                _context.CarTypes.Update(CarType);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCarType");
        }

        [HttpGet]
        public ActionResult UpdateCarType(int id)
        {
            if (ModelState.IsValid)
            {
                var CarType = _context.CarTypes.Where(x => x.CarTypeId == id && x.DeleteFlag == false).FirstOrDefault();

                if (CarType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(CarType);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        public ActionResult UpdateCarType(CarType model)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var CarTypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
                var CarType = CarTypes.Where(x => x.CarTypeId == model.CarTypeId).FirstOrDefault();

                if (CarType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                if (CarTypes.Any(x => x.TypeName.Equals(model.TypeName) && x.CarTypeId != model.CarTypeId))
                {
                    ModelState.AddModelError("TypeName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                CarType.TypeName = model.TypeName.ReplaceSpace().Trim();
                CarType.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                CarType.ModifiedOn = DateTime.Now;
                _context.CarTypes.Update(CarType);
                _context.SaveChanges();
            }
            return RedirectToAction("ListCarType");
        }

        [HttpPost]
        public IActionResult DeleteListCarType(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                foreach (var id in selectedIds)
                {
                    var carType = _context.CarTypes.Find(id);
                    if (carType != null)
                    {
                        carType.DeleteFlag = true;
                        carType.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        carType.ModifiedOn = DateTime.Now;
                        _context.CarTypes.Update(carType);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCarType");
        }
    }
}
