using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class FuelController : CustomController
    {
        private readonly CarDealersContext _context;

        public FuelController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListFuel(int? page, int? pageSize, string Keyword)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set a default number of records per page
            int pageNumber = page ?? 1;
            int recordsPerPage = pageSize ?? defaultPageSize;
            var query1 = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.FuelTypeName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.FuelTypeName.ToLower()) || keywords.Any(e => u.FuelTypeName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }

            var query = query1.AsQueryable();
            var fuels = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.Keyword = Keyword;
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed

            return View(fuels);
		}
            
        public ActionResult CreateFuel() {
            return View();
        }

        [HttpPost]
        public IActionResult CreateFuel(FuelType model) {
            if(ModelState.IsValid)
            {
                var fuels = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
                if (fuels.Any(x => x.FuelTypeName.Equals(model.FuelTypeName))){

                    ModelState.AddModelError("FuelTypeName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                var fuel = new FuelType { FuelTypeName = model.FuelTypeName };
                _context.FuelTypes.Add(fuel);
                _context.SaveChanges();
            }
            return RedirectToAction("ListFuel");
        }

        [HttpGet]
        public IActionResult DeleteFuel(int id)
        {
            if (ModelState.IsValid)
            {
                var fuel = _context.FuelTypes.Where(x => x.FuelTypeId == id).FirstOrDefault();
                fuel.DeleteFlag = true;
                _context.FuelTypes.Update(fuel);
                _context.SaveChanges();
            }
            else {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListFuel");
        }

        [HttpGet]
        public ActionResult UpdateFuel(int id) {
            if (ModelState.IsValid) {
                var fuel = _context.FuelTypes.Where(x => x.FuelTypeId == id && x.DeleteFlag == false).FirstOrDefault();
                
                if (fuel == null) {
                    return RedirectToAction("RecordNotFound");
                }
                return View(fuel);
            }
            return RedirectToAction("RecordNotFound");
        }
        [HttpPost]
        public ActionResult UpdateFuel(FuelType model) {
            if (ModelState.IsValid) {
                var fuels = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
                var fuel = fuels.Where(x => x.FuelTypeId == model.FuelTypeId).FirstOrDefault();

                if(fuel == null) 
                {
                    return RedirectToAction("RecordNotFound");
                }

                if (fuels.Any(x => x.FuelTypeName.Equals(model.FuelTypeName) && x.FuelTypeId != model.FuelTypeId))
                {
                    ModelState.AddModelError("FuelName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                fuel.FuelTypeName = model.FuelTypeName;
                _context.FuelTypes.Update(fuel);
                _context.SaveChanges();
                
            }
            return RedirectToAction("ListFuel");
        }

        [HttpPost]
        public IActionResult DeleteListFuelType(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var fuelType = _context.FuelTypes.Find(id);
                    if (fuelType != null)
                    {
                        fuelType.DeleteFlag = true;
                        _context.FuelTypes.Update(fuelType);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListFuel");
        }
        
    }
}
