using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Drawing.Drawing2D;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class ServiceTypeController : CustomController
    {
        private readonly CarDealersContext _context;

        public ServiceTypeController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        [HttpGet]
        public IActionResult CreateServiceType()
        {
            return View();
        }

        public IActionResult ListServiceType(int? page, string Keyword, int? pageSize)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.ServiceTypes.Where(x => x.DeleteFlag == false).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.ServiceTypeName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.ServiceTypeName.ToLower()) || keywords.Any(e => u.ServiceTypeName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var ServiceTypes = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query1.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListServiceType", ServiceTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateServiceType(ServiceType serviceType)
        {
            if (ModelState.IsValid)
            {
                var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).ToList();
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                if (serviceTypes.Any(x => x.ServiceTypeName.Equals(serviceType.ServiceTypeName)))
                {
                    ModelState.AddModelError("ServiceTypeName", "The name already exists. Please choose a different name.");
                    return View(serviceType);
                }
                // Save the ServiceType details to the database
                var newServiceType = new ServiceType
                {
                    ServiceTypeName = serviceType.ServiceTypeName.ReplaceSpace().Trim(),
                    Description = serviceType.Description,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.ServiceTypes.AddAsync(newServiceType);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListServiceType");
            }
            return View(serviceType);
        }


        [HttpGet]
        public async Task<ActionResult> DeleteServiceType(int id)
        {
            if (ModelState.IsValid)
            {
                var serviceType = _context.ServiceTypes.Where(x => x.ServiceTypeId == id).FirstOrDefault();
                try
                {
                    serviceType.DeleteFlag = true;
                    _context.ServiceTypes.Update(serviceType);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ListServiceType");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error");
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListServiceType");
        }

        [HttpGet]
        public async Task<ActionResult> UpdateServiceType(int id)
        {
            if (ModelState.IsValid)
            {
                var serviceType = _context.ServiceTypes.Where(x => x.ServiceTypeId == id && x.DeleteFlag == false).FirstOrDefault();

                if (serviceType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(serviceType);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateServiceType(ServiceType model, IFormFile? logoImage)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var serviceTypes = _context.ServiceTypes.Where(x => x.DeleteFlag == false).ToList();
                var serviceType = serviceTypes.Where(x => x.ServiceTypeId == model.ServiceTypeId).FirstOrDefault();

                if (serviceType == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                // Find the existing ServiceType in the database by ID
                if (serviceTypes.Any(x => x.ServiceTypeName.Equals(model.ServiceTypeName) && x.ServiceTypeId != model.ServiceTypeId))
                {
                    ModelState.AddModelError("ServiceTypeName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                // Update the ServiceType's properties
                serviceType.ServiceTypeName = model.ServiceTypeName.ReplaceSpace().Trim();
                serviceType.Description = model.Description;
                serviceType.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                serviceType.ModifiedOn = DateTime.Now;
                _context.ServiceTypes.Update(serviceType);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListServiceType");
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult DeleteListServiceType(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                // Tiến hành xóa các bản ghi có ID nằm trong danh sách selectedIds
                foreach (var id in selectedIds)
                {
                    var serviceTypes = _context.ServiceTypes.Find(id);
                    if (serviceTypes != null)
                    {
                        serviceTypes.DeleteFlag = true;
                        serviceTypes.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        serviceTypes.ModifiedOn = DateTime.Now;
                        _context.ServiceTypes.Update(serviceTypes);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListServiceType");
        }
    }
}

