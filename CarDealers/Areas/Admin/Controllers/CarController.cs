using CarDealers.Areas.Admin.Models.CarViewModel;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Drawing.Drawing2D;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class CarController : CustomController
    {
        private readonly CarDealersContext _context;

        public CarController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListCar(int? page, int? pageSize, string Keyword, string brandFilter, string carTypeFilter, string engineFilter, string fuelFilter)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 10; // Set a default number of records per page
            int pageNumber = page ?? 1;
            int recordsPerPage = pageSize ?? defaultPageSize;
            var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();
            var brandSelectList = brands.Select(brand => new SelectListItem
            {
                Value = brand.BrandId.ToString(),
                Text = brand.BrandName,
                Selected = brand.BrandId.ToString().Equals(brandFilter)
            }).ToList();
            ViewBag.BrandOptions = brandSelectList;
            var cartypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
            var carTypeSelectList = cartypes.Select(cartype => new SelectListItem
            {
                Value = cartype.CarTypeId.ToString(),
                Text = cartype.TypeName,
                Selected = cartype.CarTypeId.ToString().Equals(carTypeFilter)
            }).ToList();
            ViewBag.CarTypeOptions = carTypeSelectList;
            var enginetypes = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
            var engineTypeSelectList = enginetypes.Select(enginetype => new SelectListItem
            {
                Value = enginetype.EngineTypeId.ToString(),
                Text = enginetype.EngineTypeName,
                Selected = enginetype.EngineTypeId.ToString().Equals(engineFilter)
            }).ToList();
            ViewBag.EngineType = engineTypeSelectList;
            var fueltypes = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
            var fuelTypeSelectList = fueltypes.Select(fueltype => new SelectListItem
            {
                Value = fueltype.FuelTypeId.ToString(),
                Text = fueltype.FuelTypeName,
                 Selected = fueltype.FuelTypeId.ToString().Equals(fuelFilter)
            }).ToList();
            ViewBag.FuelType = fuelTypeSelectList;
            ViewBag.Keyword = Keyword;
            //fetch data from database table orders
            var query1 = _context.Cars.Where(x => x.DeleteFlag == false)
                                        .Include(x => x.Brand)
                                        .Include(x => x.CarType)
                                        .Include(x => x.EngineType)
                                        .Include(x => x.FuelType)
                                        .OrderBy(p => p.CarId).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.Model.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.Model.ToLower()) || keywords.Any(e => u.Model.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            if (!string.IsNullOrEmpty(brandFilter))
            {
                query = query.Where(u => u.BrandId == int.Parse(brandFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(engineFilter))
            {
                query = query.Where(u => u.EngineTypeId == int.Parse(engineFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(fuelFilter))
            {
                query = query.Where(u => u.FuelTypeId == int.Parse(fuelFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(carTypeFilter))
            {
                query = query.Where(u => u.CarTypeId == int.Parse(carTypeFilter.Trim().ToLower()));
            }

            var cars = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();
            ViewBag.Brand = brandFilter;
            ViewBag.CarType = carTypeFilter;
            ViewBag.Engine = engineFilter;
            ViewBag.Fuel = fuelFilter;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed

            return View("ListCar", cars);
        }

        [HttpGet]
        public IActionResult CreateCar()
        {
            var viewModel = new CreateCarViewModel();
            var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();
            var cartypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
            var enginetypes = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
            var fueltypes = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();

            viewModel.BrandList = brands.Select(brand => new SelectListItem
            {
                Value = brand.BrandId.ToString(),
                Text = brand.BrandName
            });
            viewModel.CarTypeList = cartypes.Select(cartype => new SelectListItem
            {
                Value = cartype.CarTypeId.ToString(),
                Text = cartype.TypeName
            });
            viewModel.EngineTypeList = enginetypes.Select(enginetype => new SelectListItem
            {
                Value = enginetype.EngineTypeId.ToString(),
                Text = enginetype.EngineTypeName
            });
            viewModel.FuelTypeList = fueltypes.Select(fueltype => new SelectListItem
            {
                Value = fueltype.FuelTypeId.ToString(),
                Text = fueltype.FuelTypeName
            });
            viewModel.Colors = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName
            });

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCar(CreateCarViewModel car, IFormFile Image)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();
            var cartypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
            var enginetypes = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
            var fueltypes = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
            car.BrandList = brands.Select(brand => new SelectListItem
            {
                Value = brand.BrandId.ToString(),
                Text = brand.BrandName
            });
            car.CarTypeList = cartypes.Select(cartype => new SelectListItem
            {
                Value = cartype.CarTypeId.ToString(),
                Text = cartype.TypeName
            });
            car.EngineTypeList = enginetypes.Select(enginetype => new SelectListItem
            {
                Value = enginetype.EngineTypeId.ToString(),
                Text = enginetype.EngineTypeName
            });
            car.FuelTypeList = fueltypes.Select(fueltype => new SelectListItem
            {
                Value = fueltype.FuelTypeId.ToString(),
                Text = fueltype.FuelTypeName
            });
            car.Colors = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName
            });
            if (brands == null || brands.Count == 0)
            {
                ModelState.AddModelError("BrandId", "The Brand Require");
                return View(car);
            }
            if (cartypes == null || cartypes.Count == 0)
            {
                ModelState.AddModelError("CarTypeId", "Car Type Require");
                return View(car);
            }
            if (enginetypes == null || enginetypes.Count == 0)
            {
                ModelState.AddModelError("EngineTypeId", "Engine Type Require");
                return View(car);
            }
            if (fueltypes == null || fueltypes.Count == 0)
            {
                ModelState.AddModelError("FuelTypeId", "Fuel Type Require");
                return View(car);
            }
            if (colors == null || colors.Count == 0 || car.SelectedColors == null)
            {
                ModelState.AddModelError("SelectedColors", "color Require");
                return View(car);
            }
            if (ModelState.IsValid)
            {
                var cars = _context.Cars.Where(x => x.DeleteFlag == false).ToList();

                if (cars.Any(x => x.Model.Equals(car.Model) && car.Year == x.Year))
                {
                    ModelState.AddModelError("Model", "The Model already exists. Please choose a different name.");
                    return View(car);
                }
                var newCar = new Car
                {
                    Model = car.Model.ReplaceSpace().Trim(),
                    Year = car.Year,
                    Quantity = int.Parse(car.Quantity.Replace(",", "")),
                    BrandId = car.BrandId,
                    CarTypeId = car.CarTypeId,
                    EngineTypeId = car.EngineTypeId,
                    FuelTypeId = car.FuelTypeId,
                    DepositPrice = car.DepositPrice,
                    ImportPrice = car.ImportPrice,
                    ExportPrice = car.ExportPrice,
                    Description = car.Description,
                    Transmission = car.Transmission,
                    Mileage = int.Parse(car.Mileage.Replace(",", "")),
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now,
                    Tax = car.Tax,
                    Status = car.Status != 0,
                    Content = car.Content
                };
                if (Image != null && Image.Length > 0)
                {
                    var allowedContentTypes = new List<string> { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                    if (!allowedContentTypes.Contains(Image.ContentType))
                    {
                        ModelState.AddModelError("Image", "Please upload a valid image. Only JPEG, PNG, GIF, and BMP formats are allowed.");
                        return View(car);
                    }
                    if (Image.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Image", "The uploaded image is too large. It should be 2MB or less.");
                        return View(car);
                    }
                    // Generate a unique file name for the image
                    var fileName = Guid.NewGuid().ToString() + "-" + Image.FileName;

                    // Combine the constant folder path and file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGECARPATH, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }

                    newCar.Image = Path.Combine(Constant.IMAGECARPATH, fileName).Replace('\\', '/'); // Store the file path in the database
                }
                await _context.Cars.AddAsync(newCar);
                await _context.SaveChangesAsync();
                var newCarColor = car.SelectedColors.Select(x => new ColorCarRefer
                {
                    CarId = newCar.CarId,
                    ColorId = x,
                    Image = newCar.Image
                });
                await _context.ColorCarRefers.AddRangeAsync(newCarColor);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListCar");
            }
            return View(car);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateCar(int id)
        {
            if (ModelState.IsValid)
            {
                var car = _context.Cars.Where(x => x.CarId == id && x.DeleteFlag == false).FirstOrDefault();

                if (car == null)
                {
                    return RedirectToAction("RecordNotFound");
                }

                var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();
                var cartypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
                var enginetypes = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
                var fueltypes = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
                var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
                var carcolorRefers = _context.ColorCarRefers.Where(x => x.CarId == car.CarId).ToList();
                var updateCarViewModel = new UpdateCarViewModel
                {
                    CarId = car.CarId,
                    Model = car.Model.ReplaceSpace(),
                    Year = car.Year.Value,
                    Quantity = car.Quantity.ToString("N0"),
                    BrandId = car.BrandId,
                    CarTypeId = car.CarTypeId,
                    EngineTypeId = car.EngineTypeId,
                    FuelTypeId = car.FuelTypeId,
                    DepositPrice = car.DepositPrice?.ToString("N0"),
                    ImportPrice = car.ImportPrice?.ToString("N0"),
                    ExportPrice = car.ExportPrice?.ToString("N0"),
                    Description = car.Description,
                    Transmission = car.Transmission,
                    Mileage = car.Mileage?.ToString("N0"),
                    Tax = car.Tax?.ToString("N0"),
                    Status = car.Status ? 1 : 0,
                    Image = car.Image,
                    Content = car.Content,
                    BrandList = brands.Select(brand => new SelectListItem
                    {
                        Value = brand.BrandId.ToString(),
                        Text = brand.BrandName,
                        Selected = brand.BrandId == car.BrandId
                    }),
                    CarTypeList = cartypes.Select(cartype => new SelectListItem
                    {
                        Value = cartype.CarTypeId.ToString(),
                        Text = cartype.TypeName,
                        Selected = cartype.CarTypeId == car.CarTypeId
                    }),
                    EngineTypeList = enginetypes.Select(enginetype => new SelectListItem
                    {
                        Value = enginetype.EngineTypeId.ToString(),
                        Text = enginetype.EngineTypeName,
                        Selected = enginetype.EngineTypeId == car.EngineTypeId
                    }),
                    FuelTypeList = fueltypes.Select(fueltype => new SelectListItem
                    {
                        Value = fueltype.FuelTypeId.ToString(),
                        Text = fueltype.FuelTypeName,
                        Selected = fueltype.FuelTypeId == car.FuelTypeId
                    }),
                    Colors = colors.Select(color => new SelectListItem
                    {
                        Value = color.ColorId.ToString(),
                        Text = color.ColorName,
                        Selected = carcolorRefers.Any(x => x.ColorId == color.ColorId)
                    }),
                    SelectedColors = carcolorRefers.Select(x => x.ColorId).AsEnumerable()

                };
                updateCarViewModel.StatusList = updateCarViewModel.StatusList.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text,
                    Selected = car.Status.ToString().Equals(x.Value)
                });
                return View(updateCarViewModel);
            }
            return RedirectToAction("RecordNotFound");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCar(UpdateCarViewModel updateCarViewModel, IFormFile? Image)
        {
            var car = _context.Cars.Where(x => x.CarId == updateCarViewModel.CarId && x.DeleteFlag == false).FirstOrDefault();

            if (car == null)
            {
                return RedirectToAction("RecordNotFound");
            }
            //updateCarViewModel.Image = car.Image;
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var brands = _context.Brands.Where(x => x.DeleteFlag == false).ToList();
            var cartypes = _context.CarTypes.Where(x => x.DeleteFlag == false).ToList();
            var enginetypes = _context.EngineTypes.Where(x => x.DeleteFlag == false).ToList();
            var fueltypes = _context.FuelTypes.Where(x => x.DeleteFlag == false).ToList();
            var colors = _context.Colors.Where(x => x.DeleteFlag == false).ToList();
            var carcolorRefers = _context.ColorCarRefers.Where(x => x.CarId == car.CarId).ToList();

            updateCarViewModel.BrandList = brands.Select(brand => new SelectListItem
            {
                Value = brand.BrandId.ToString(),
                Text = brand.BrandName,
                Selected = brand.BrandId == car.BrandId
            });
            updateCarViewModel.CarTypeList = cartypes.Select(cartype => new SelectListItem
            {
                Value = cartype.CarTypeId.ToString(),
                Text = cartype.TypeName,
                Selected = cartype.CarTypeId == car.CarTypeId
            });
            updateCarViewModel.EngineTypeList = enginetypes.Select(enginetype => new SelectListItem
            {
                Value = enginetype.EngineTypeId.ToString(),
                Text = enginetype.EngineTypeName,
                Selected = enginetype.EngineTypeId == car.EngineTypeId
            });
            updateCarViewModel.FuelTypeList = fueltypes.Select(fueltype => new SelectListItem
            {
                Value = fueltype.FuelTypeId.ToString(),
                Text = fueltype.FuelTypeName,
                Selected = fueltype.FuelTypeId == car.FuelTypeId
            });
            updateCarViewModel.Colors = colors.Select(color => new SelectListItem
            {
                Value = color.ColorId.ToString(),
                Text = color.ColorName,
                Selected = carcolorRefers.Any(x => x.ColorId == color.ColorId)
            });
            if (brands == null || brands.Count == 0)
            {
                ModelState.AddModelError("BrandId", "The Brand Require");
                return View(updateCarViewModel);
            }
            if (cartypes == null || cartypes.Count == 0)
            {
                ModelState.AddModelError("CarTypeId", "Car Type Require");
                return View(updateCarViewModel);
            }
            if (enginetypes == null || enginetypes.Count == 0)
            {
                ModelState.AddModelError("EngineTypeId", "Engine Type Require");
                return View(updateCarViewModel);
            }
            if (fueltypes == null || fueltypes.Count == 0)
            {
                ModelState.AddModelError("FuelTypeId", "Fuel Type Require");
                return View(updateCarViewModel);
            }
            if (colors == null || colors.Count == 0 || updateCarViewModel.SelectedColors.Count() == 0)
            {
                ModelState.AddModelError("SelectedColors", "Color Require");
                return View(updateCarViewModel);
            }
            if (ModelState.IsValid)
            {
                // Update car properties based on the view model
                car.Model = updateCarViewModel.Model;
                car.Year = updateCarViewModel.Year;
                car.Quantity = int.Parse(updateCarViewModel.Quantity.Replace(",", ""));
                car.BrandId = updateCarViewModel.BrandId;
                car.CarTypeId = updateCarViewModel.CarTypeId;
                car.EngineTypeId = updateCarViewModel.EngineTypeId;
                car.FuelTypeId = updateCarViewModel.FuelTypeId;
                car.DepositPrice = updateCarViewModel.DepositPrice.IsNullOrEmpty() ? 0 : decimal.Parse(updateCarViewModel.DepositPrice.Replace(",", ""));
                car.ImportPrice = updateCarViewModel.ImportPrice.IsNullOrEmpty() ? 0 : decimal.Parse(updateCarViewModel.ImportPrice.Replace(",", ""));
                car.ExportPrice = updateCarViewModel.ExportPrice.IsNullOrEmpty() ? 0 : decimal.Parse(updateCarViewModel.ExportPrice.Replace(",", ""));
                car.Description = updateCarViewModel.Description;
                car.Mileage = updateCarViewModel.Mileage.IsNullOrEmpty() ? 0 : int.Parse(updateCarViewModel.Mileage.Replace(",", ""));
                car.Tax = updateCarViewModel.Tax.IsNullOrEmpty() ? 0 : decimal.Parse(updateCarViewModel.Tax.Replace(",", ""));
                car.Status = updateCarViewModel.Status == 1;
                car.Content = updateCarViewModel.Content;
                car.Transmission = updateCarViewModel.Transmission;
                car.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                car.ModifiedOn = DateTime.Now;

                // Handle image upload
                if (Image != null && Image.Length > 0)
                {
                    var allowedContentTypes = new List<string> { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                    if (!allowedContentTypes.Contains(Image.ContentType))
                    {
                        ModelState.AddModelError("Image", "Please upload a valid image. Only JPEG, PNG, GIF, and BMP formats are allowed.");
                        return View(updateCarViewModel);
                    }
                    if (Image.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Image", "The uploaded image is too large. It should be 2MB or less.");
                        return View(updateCarViewModel);
                    }
                    // Delete the image file
                    if (!string.IsNullOrEmpty(car.Image))
                    {
                        var imagePath = Path.Combine(Constant.PATH, car.Image);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    // Generate a unique file name for the image
                    var fileName = Guid.NewGuid().ToString() + "-" + Image.FileName;

                    // Combine the constant folder path and file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGECARPATH, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }

                    car.Image = Path.Combine(Constant.IMAGECARPATH, fileName).Replace('\\', '/'); // Store the file path in the database
                }

                _context.Cars.Update(car);
                //remove Color
                var carColorReferDelete = carcolorRefers.Where(x => !updateCarViewModel.SelectedColors.Contains(x.ColorId)).ToList();
                // Save the changes to the database
                if (carColorReferDelete.Count != 0)
                {
                    _context.ColorCarRefers.RemoveRange(carColorReferDelete);
                }
                //add color
                var carColorReferAdd = updateCarViewModel.SelectedColors.Except(carcolorRefers.Select(x => x.ColorId)).ToList();
                if (carColorReferAdd.Count != 0)
                {
                    var newCarColors = carColorReferAdd.Select(x => new ColorCarRefer
                    {
                        CarId = car.CarId,
                        ColorId = x,
                        Image = car.Image
                    });
                    await _context.ColorCarRefers.AddRangeAsync(newCarColors);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("ListCar");
            }
            return View(updateCarViewModel);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteCar(int id)
        {
            if (ModelState.IsValid)
            {
                var car = _context.Cars.Where(x => x.CarId == id).FirstOrDefault();
                car.DeleteFlag = true;
                _context.Cars.Update(car);
                // Delete the image file
                if (!string.IsNullOrEmpty(car.Image))
                {
                    var imagePath = Path.Combine(Constant.PATH, car.Image);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCar");
        }

        [HttpPost]
        public IActionResult DeleteListCar(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var car = _context.Cars.Find(id);
                    if (car != null)
                    {
                        if (!string.IsNullOrEmpty(car.Image))
                        {
                            var imagePath = Path.Combine(Constant.PATH, car.Image);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                        car.DeleteFlag = true;
                        _context.Cars.Update(car);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCar");
        }
    }
}
