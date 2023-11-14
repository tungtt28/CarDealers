using CarDealers.Areas.Admin.Models.BrandViewModel;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class BrandController : CustomController
    {
        private readonly CarDealersContext _context;

        public BrandController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }
        [HttpGet]
        public IActionResult CreateBrand()
        {
            return View();
        }

        public IActionResult ListBrand(int? page, int? pageSize, string Keyword)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 2; // Set a default number of records per page
            int pageNumber = page ?? 1;
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.Brands.Where(x => x.DeleteFlag == false).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.BrandName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.BrandName.ToLower()) || keywords.Any(e => u.BrandName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var brands = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListBrand", brands);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBrand(CreateBrandViewModel brand, IFormFile logoImage)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();

                if (brands.Any(x => x.BrandName.Equals(brand.BrandName)))
                {
                    ModelState.AddModelError("BrandName", "The name already exists. Please choose a different name.");
                    return View(brand);
                }
                // Save the brand details to the database
                var newBrand = new Brand
                {
                    BrandName = brand.BrandName.ReplaceSpace().Trim(),
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                // Handle logo image upload
                if (logoImage != null && logoImage.Length > 0)
                {
                    var allowedContentTypes = new List<string> { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                    if (!allowedContentTypes.Contains(logoImage.ContentType))
                    {
                        ModelState.AddModelError("LogoImage", "Please upload a valid image. Only JPEG, PNG, GIF, and BMP formats are allowed.");
                        return View(brand);
                    }
                    if (logoImage.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("LogoImage", "The uploaded image is too large. It should be 2MB or less.");
                        return View(brand);
                    }
                    // Generate a unique file name for the image
                    var fileName = Guid.NewGuid().ToString() + "-" + logoImage.FileName;

                    // Combine the constant folder path and file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGELOGOPATH, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoImage.CopyToAsync(fileStream);
                    }

                    newBrand.LogoImage = Path.Combine(Constant.IMAGELOGOPATH, fileName).Replace('\\', '/'); // Store the file path in the database
                }
                await _context.Brands.AddAsync(newBrand);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListBrand");
            }

            return View(brand);
        }


        [HttpGet]
        public async Task<ActionResult> DeleteBrand(int id)
        {
            if (ModelState.IsValid)
            {
                var brand = _context.Brands.Where(x => x.BrandId == id).FirstOrDefault();
                try
                {
                    // Delete the image file
                    if (!string.IsNullOrEmpty(brand.LogoImage))
                    {
                        var imagePath = Path.Combine(Constant.PATH, brand.LogoImage);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    brand.DeleteFlag = true;
                    _context.Brands.Update(brand);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ListBrand");
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
            return RedirectToAction("ListBrand");
        }

        [HttpGet]
        public async Task<ActionResult> UpdateBrand(int id)
        {
            if (ModelState.IsValid)
            {
                var brand = _context.Brands.Where(x => x.BrandId == id && x.DeleteFlag == false).FirstOrDefault();
                var viewBrandModel = new UpdateBrandViewModel
                {
                    BrandId = brand.BrandId,
                    BrandName = brand.BrandName,
                    LogoImage = brand.LogoImage,
                };
                if (brand == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(viewBrandModel);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateBrand(UpdateBrandViewModel model, IFormFile? logoImage)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();
                var brand = brands.Where(x => x.BrandId == model.BrandId).FirstOrDefault();

                if (brand == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                // Find the existing brand in the database by ID
                if (brands.Any(x => x.BrandName.Equals(model.BrandName) && x.BrandId != model.BrandId))
                {
                    ModelState.AddModelError("BrandName", "The name already exists. Please choose a different name.");
                    return View(model);
                }

                // Update the brand's properties
                brand.BrandName = model.BrandName.ReplaceSpace().Trim();
                brand.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                brand.ModifiedOn = DateTime.Now;
                // Handle logo image upload
                if (logoImage != null && logoImage.Length > 0)
                {
                    var allowedContentTypes = new List<string> { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                    if (!allowedContentTypes.Contains(logoImage.ContentType))
                    {
                        ModelState.AddModelError("LogoImage", "Please upload a valid image. Only JPEG, PNG, GIF, and BMP formats are allowed.");
                        return View(model);
                    }
                    if (logoImage.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("LogoImage", "The uploaded image is too large. It should be 2MB or less.");
                        return View(model);
                    }
                    if (!string.IsNullOrEmpty(brand.LogoImage))
                    {
                        var imagePath = Path.Combine(Constant.PATH, brand.LogoImage);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    // Generate a unique file name for the image
                    var fileName = Guid.NewGuid().ToString() + "-" + logoImage.FileName;

                    // Combine the constant folder path and file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGELOGOPATH, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoImage.CopyToAsync(fileStream);
                    }

                    brand.LogoImage = Path.Combine(Constant.IMAGELOGOPATH, fileName).Replace('\\', '/'); // Store the file path in the database
                }
                 _context.Brands.Update(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListBrand");
            }

            return View(model);
        }


        [HttpPost]
        public IActionResult DeleteListBrand(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                // Tiến hành xóa các bản ghi có ID nằm trong danh sách selectedIds
                foreach (var id in selectedIds)
                {
                    var brand = _context.Brands.Find(id);
                    if (brand != null)
                    {
                        brand.DeleteFlag = true;
                        brand.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        brand.ModifiedOn = DateTime.Now;
                        _context.Brands.Update(brand);
                        if (!string.IsNullOrEmpty(brand.LogoImage))
                        {
                            var imagePath = Path.Combine(Constant.PATH, brand.LogoImage);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListBrand");
        }
    }

}
