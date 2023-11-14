using CarDealers.Areas.Admin.Models.AccessoriesViewModel;
using CarDealers.Areas.Admin.Models.CarViewModel;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;


namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class AccessoriesController : CustomController
    {
        private readonly CarDealersContext _context;

        public AccessoriesController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListAccessories(int? page, int? pageSize, string Keyword)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.AccessoryName.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.AccessoryName.ToLower()) || keywords.Any(e => u.AccessoryName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var accessories = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            var listAccessories = accessories.Select(x => new ListAccessoriesViewModel
            {
                AccessoryId = x.AccessoryId,
                Quantity = x.Quantity.ToString("N0"),
                AccessoryName = x.AccessoryName,
                ImportPrice = x.ImportPrice?.ToString("N0"),
                ExportPrice = x.ExportPrice?.ToString("N0"),
                Description = x.Description,
                Image = x.Image,
                Status = x.Status
            }).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.Keyword = Keyword;

            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed

            return View("ListAccessories", listAccessories);
        }

        [HttpGet]
        public IActionResult CreateAccessories()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAccessoriesAsync(CreateAccessoriesViewModel model, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();

                //single 
                var accessory = new AutoAccessory();
                accessory.Image = "Empty";

                //check the same
                //check if quantity is not integer, null or <0
                if (!int.TryParse(model.Quantity.ToString(), out int quantityValue) || model.Quantity.ToString() == null || int.Parse(model.Quantity) < 0)
                {
                    ModelState.AddModelError("Quantity", "Quantity cannot empty or below 0. Please choose again.");
                    return View(model);
                }

                //check if name null
                if (model.AccessoryName == null)
                {
                    ModelState.AddModelError("AccessoryName", "Accessory name cannot empty. Please choose again.");
                    return View(model);
                }

                //check if name is existed
                if (accessories.Any(x => x.AccessoryName.Equals(model.AccessoryName)))
                {
                    ModelState.AddModelError("AccessoryName", "The name of accessory already exists. Please choose a different name.");
                    return View(model);
                }

                // Handle logo image upload
                if (image == null || image.Length == 0)
                {
                    ModelState.AddModelError("Image", "Image is neccesery. Please choose again.");
                    return View(model);
                }
                else if (image != null && image.Length > 0)
                {
                    if (image.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Image", "The uploaded image is too large. It should be 2MB or less.");
                        return View(model);
                    }

                    // Kiểm tra định dạng tệp ảnh
                    string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                    string fileExtension = Path.GetExtension(image.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("Image", "Only .png, .jpeg, .jpg files are allowed.");
                        return View(model);
                    }

                    // Generate a unique file name for the image
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    // Combine the constant folder path and file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGEACCESSORIESPATH, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    model.Image = Path.Combine(Constant.IMAGEACCESSORIESPATH, fileName).Replace('\\', '/'); // Store the file path in the database
                }

                accessory.Quantity = int.Parse(model.Quantity.Replace(",", ""));
                accessory.AccessoryName = model.AccessoryName;
                accessory.ImportPrice = model.ImportPrice;
                accessory.ExportPrice = model.ExportPrice;
                accessory.Description = model.Description;
                accessory.Image = model.Image;
                accessory.Status = model.Status;
                accessory.CreatedBy = customerId != null ? int.Parse(customerId) : null;
                accessory.CreatedOn = DateTime.Now;

                await _context.AutoAccessories.AddAsync(accessory);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListAccessories");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateAccessories(int id)
        {
            var accessory = _context.AutoAccessories.Where(x => x.AccessoryId == id && x.DeleteFlag == false).FirstOrDefault();

            if (accessory == null)
            {
                return RedirectToAction("RecordNotFound");
            }
            var updateAccessoriesViewModel = new UpdateAccessoriesViewModel
            {
                AccessoryId = accessory.AccessoryId,
                Quantity = accessory.Quantity.ToString("N0"),
                AccessoryName = accessory.AccessoryName,
                ImportPrice = accessory.ImportPrice?.ToString("N0"),
                ExportPrice = accessory.ExportPrice?.ToString("N0"),
                Description = accessory.Description,
                Image = accessory.Image,
                Status = accessory.Status
            };

            return View(updateAccessoriesViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAccessories(UpdateAccessoriesViewModel model, IFormFile? image)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var accessories = _context.AutoAccessories.Where(x => x.DeleteFlag == false).ToList();
            var accessory = accessories.Where(x => x.AccessoryId == model.AccessoryId).FirstOrDefault();

            //check if name null
            if (model.AccessoryName == null)
            {
                ModelState.AddModelError("AccessoryName", "Accessory name cannot empty. Please choose again.");
                return View(model);
            }
            //name existed
            if (!accessory.AccessoryName.Equals(model.AccessoryName))
            {
                //check if name is existed except the one you chose
                if (accessories.Any(x => x.AccessoryName.Equals(model.AccessoryName)))
                {
                    ModelState.AddModelError("AccessoryName", "The name of accessory already exists. Please choose a different name.");
                    return View(model);
                }
            }

            if (model.Quantity == null)
            {
                ModelState.AddModelError("Quantity", "Quantity cannot empty or below 0. Please choose again.");
                return View(model);
            }

            // Handle logo image upload
            if (image.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("Image", "The uploaded image is too large. It should be 2MB or less.");
                return View(model);
            }
            // Kiểm tra định dạng tệp ảnh
            string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
            string fileExtension = Path.GetExtension(image.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("Image", "Only .png, .jpeg, .jpg files are allowed.");
                return View(model);
            }
            if (image != null && image.Length > 0)
            {
                if (!string.IsNullOrEmpty(accessory.Image))
                {
                    var imagePath = Path.Combine(Constant.PATH, accessory.Image);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Generate a unique file name for the image
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                // Combine the constant folder path and file name
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGEACCESSORIESPATH, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                model.Image = Path.Combine(Constant.IMAGEACCESSORIESPATH, fileName).Replace('\\', '/'); // Store the file path in the database
            }

            if (ModelState.IsValid)
            {
                accessory.Quantity = int.Parse(model.Quantity.Replace(",", ""));
                accessory.AccessoryName = model.AccessoryName;
                if (model.ImportPrice.IsNullOrEmpty())
                {
                    accessory.ImportPrice = null;
                }
                else
                {
                    accessory.ImportPrice = decimal.Parse(model.ImportPrice.Replace(",", ""));

                }
                if (model.ExportPrice.IsNullOrEmpty())
                {
                    accessory.ExportPrice = null;
                }
                else
                {
                    accessory.ExportPrice = decimal.Parse(model.ExportPrice.Replace(",", ""));

                }
                accessory.Description = model.Description;
                accessory.Image = model.Image;
                accessory.Status = model.Status;
                accessory.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                accessory.ModifiedOn = DateTime.Now;

                await _context.SaveChangesAsync();
                return RedirectToAction("ListAccessories");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> DeleteAccessories(int id)
        {
            if (ModelState.IsValid)
            {
                var accessory = _context.AutoAccessories.Where(x => x.AccessoryId == id).FirstOrDefault();
                try
                {
                    // Delete the image file
                    if (!string.IsNullOrEmpty(accessory.Image))
                    {
                        var imagePath = Path.Combine(Constant.PATH, accessory.Image);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    accessory.DeleteFlag = true;
                    _context.AutoAccessories.Update(accessory);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ListAccessories");
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
            return RedirectToAction("ListAccessories");
        }

        [HttpPost]
        public IActionResult DeleteListAccessories(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                // Tiến hành xóa các bản ghi có ID nằm trong danh sách selectedIds
                foreach (var id in selectedIds)
                {
                    var accessory = _context.AutoAccessories.Find(id);
                    if (accessory != null)
                    {
                        accessory.DeleteFlag = true;
                        accessory.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        accessory.ModifiedOn = DateTime.Now;
                        _context.AutoAccessories.Update(accessory);
                        if (!string.IsNullOrEmpty(accessory.Image))//delete image
                        {
                            var imagePath = Path.Combine(Constant.PATH, accessory.Image);
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
            return RedirectToAction("ListAccessories");
        }

        [HttpGet]
        public ActionResult ViewDetails(int id)
        {
            if (ModelState.IsValid)
            {
                var accessories = _context.AutoAccessories.Where(x => x.AccessoryId == id && x.DeleteFlag == false)
            .FirstOrDefault();

                if (accessories == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(accessories);
            }
            return RedirectToAction("RecordNotFound");
        }
    }
}
