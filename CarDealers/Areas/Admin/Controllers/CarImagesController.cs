
using Azure.Core;
using CarDealers.Areas.Admin.Models.CarViewModel;
using CarDealers.Entity;
using CarDealers.Models;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Threading;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class CarImagesController : CustomController
    {
        int carid;
        //create dbContext variable
        private readonly CarDealersContext _context;

        public int Carid { get => carid; set => carid = value; }

        public CarImagesController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public IActionResult ListCarImages(int? page,int id)
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }

            int pageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1

            //fetch data from database table orders
            var listCarImages = _context.ImageCars.Where(x => x.DeleteFlag == false&&x.CarId==id).Include(x => x.Car)
            .OrderBy(p => p.ImageId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();
            Carid = id;
            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = _context.ImageCars.Where(x => x.DeleteFlag == false).Count();
            ViewBag.IdCar=id;
            return View(listCarImages);
        }



        public ActionResult CreateCarImages(int id)
        {
            Carid = id;
            ViewBag.CarId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCarImages(int carId, List<IFormFile> ImageCars)
        {
            
            var imageCars = _context.ImageCars.Where(x => x.DeleteFlag == false).ToList();

            foreach (var imageFile in ImageCars)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Generate a unique file name for the image
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Combine the constant folder path and file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGELISTCAR, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    var imageCar = new ImageCar
                    {
                        CarId = carId,
                        Image = Path.Combine(Constant.IMAGELISTCAR, fileName).Replace('\\', '/') // Store the file path in the database
                    };

                    await _context.ImageCars.AddAsync(imageCar);
                }
            }

            await _context.SaveChangesAsync();
            Carid = carId;
            return RedirectToAction("ListCarImages", new { page = 1, id = carId });
            //return RedirectToAction("ListCarImages");
        }



        [HttpGet]
        public IActionResult DeleteCarImages(int id)
        {
            if (ModelState.IsValid)
            {
                var carImages = _context.ImageCars.Where(x => x.ImageId == id).FirstOrDefault();
                carImages.DeleteFlag = true;
                _context.ImageCars.Update(carImages);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCarImages", new { page = 1, id = Carid });
        }


        [HttpGet]
        public ActionResult UpdateCarImages(int id)
        {
            if (ModelState.IsValid)
            {
                var carImages = _context.ImageCars.Where(x => x.ImageId == id && x.DeleteFlag == false).FirstOrDefault();

                if (carImages == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(carImages);
            }
            return RedirectToAction("RecordNotFound");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCarImages(ImageCar model, IFormFile? ImageCars)
        {

            var imageCars = _context.ImageCars.Where(x => x.DeleteFlag == false).ToList();
            var CarImages = imageCars.Where(x => x.ImageId == model.ImageId).FirstOrDefault();

            if (CarImages == null)
            {
                return RedirectToAction("RecordNotFound");
            }

            CarImages.CarId = model.CarId;
            if (ImageCars != null && ImageCars.Length > 0)
            {
                // Delete the image file
                if (!string.IsNullOrEmpty(CarImages.Image))
                {
                    var imagePath = Path.Combine(Constant.PATH, CarImages.Image);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                // Generate a unique file name for the image
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageCars.FileName);

                // Combine the constant folder path and file name
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGELISTCAR, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageCars.CopyToAsync(fileStream);
                }

                CarImages.Image = Path.Combine(Constant.IMAGELISTCAR, fileName).Replace('\\', '/'); // Store the file path in the database
            }

            _context.ImageCars.Update(CarImages);
            _context.SaveChanges();

            return RedirectToAction("ListCarImages", new { page = 1, id = model.CarId });
        }

        [HttpPost]
        public IActionResult DeleteCarImages(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var carImages = _context.ImageCars.Find(id);
                    if (carImages != null)
                    {
                        carImages.DeleteFlag = true;
                        _context.ImageCars.Update(carImages);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCarImages", new { page = 1, id = Carid });
        }


        [HttpPost]
        public IActionResult DeleteListCarImages(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                // Tiến hành xóa các bản ghi có ID nằm trong danh sách selectedIds
                foreach (var id in selectedIds)
                {
                    var carImages = _context.ImageCars.Find(id);
                    if (carImages != null)
                    {
                        carImages.DeleteFlag = true;
                        _context.ImageCars.Update(carImages);
                        if (!string.IsNullOrEmpty(carImages.Image))
                        {
                            var imagePath = Path.Combine(Constant.PATH, carImages.Image);
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
            return RedirectToAction("ListCarImages", new { page = 1, id = Carid });
        }


    }

}


