
using Azure.Core;
using CarDealers.Areas.Admin.Models.CarViewModel;
using CarDealers.Entity;
using CarDealers.Models;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using System.Reflection.Emit;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class NewsController : CustomController
    {
        //create dbContext variable
        private readonly CarDealersContext _context;

        public NewsController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        public ActionResult CreateNews()
        {
            var newTypes = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();
            var newTypesSelectList = newTypes.Select(newTypes => new SelectListItem
            {
                Value = newTypes.NewsTypeId.ToString(),
                Text = newTypes.NewsTypeName
            }).ToList();
            ViewBag.NewsTypeOptions = newTypesSelectList;
            return View();
        }


        public IActionResult ListNews(int? page, string Keyword, string typeFilter, string authorFilter, int? pageSize)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            var newsType = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();

            var newsTypeSelectList = newsType.Select(newsType => new SelectListItem
            {
                Value = newsType.NewsTypeId.ToString(),
                Text = newsType.NewsTypeName,
                Selected = newsType.NewsTypeId.ToString().Equals(typeFilter),
            }).ToList();
            ViewBag.NewsTypeOptions = newsTypeSelectList;

            var authord = _context.Users.Where(x => x.DeleteFlag == false).ToList();

            var authordSelectList = authord.Select(authord => new SelectListItem
            {
                Value = authord.UserId.ToString(),
                Text = authord.FullName,
                Selected = authord.UserId.ToString().Equals(authorFilter),
            }).ToList();
            ViewBag.AuthordOptions = authordSelectList;

            var query1 = _context.News.Where(x => x.DeleteFlag == false)
                                        .Include(x => x.NewsType)
                                        .Include(x => x.Author)
                                        .OrderBy(p => p.NewsId).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.Title.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.Title.ToLower()) || keywords.Any(e => u.Title.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            if (!string.IsNullOrEmpty(typeFilter))
            {
                query = query.Where(u => u.NewsTypeId == int.Parse(typeFilter.Trim().ToLower()));
            }
            if (!string.IsNullOrEmpty(authorFilter))
            {
                query = query.Where(u => u.AuthorId == int.Parse(authorFilter.Trim().ToLower()));
            }


            //fetch data from database table orders



            var news = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            // Pass the records and page information to the view
            ViewBag.Type = typeFilter;
            ViewBag.Author = authorFilter;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed
            ViewBag.Keyword = Keyword;

            return View("ListNews", news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNews(News model, IFormFile image, string newsType)
        {
            var newTypes = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();
            var news = _context.News.Include(e => e.NewsType).Where(x => x.DeleteFlag == false).ToList();
            var newsTypeSelectList = newTypes.Select(newsType => new SelectListItem
            {
                Value = newsType.NewsTypeId.ToString(),
                Text = newsType.NewsTypeName,
                Selected = newsType.NewsTypeId.ToString().Equals(newsType)
            }).ToList();
            ViewBag.NewsTypeOptions = newsTypeSelectList;
            if (string.IsNullOrEmpty(newsType))
            {
                ModelState.AddModelError("Image", "The News Type Require");
                return View(model);
            }

            var type = newTypes.FirstOrDefault(e => e.NewsTypeId == int.Parse(newsType));
            if (type.NewsTypeName.Equals(Constant.FOOTER))
            {
                if(!model.Order.HasValue)
                {
                    ModelState.AddModelError("Order", "The Order Require");
                    return View(model);
                }    
                else if(news.Any(e => e.NewsType.NewsTypeName.Equals(Constant.FOOTER) && (model.Order.HasValue ? e.Order == model.Order : false)))
                {
                    ModelState.AddModelError("Order", "The Order Duplicate");
                    return View(model);
                }    
            }

            else if (type.NewsTypeName.Equals(Constant.MENU))
            {
                if (!model.Order.HasValue)
                {
                    ModelState.AddModelError("Order", "The Order Require");
                    return View(model);
                }
                else if (news.Any(e => e.NewsType.NewsTypeName.Equals(Constant.MENU) && (model.Order.HasValue ? e.Order == model.Order : false)))
                {
                    ModelState.AddModelError("Order", "The Order Duplicate");
                    return View(model);
                }
            }

            var authorId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

                var newss = new News
                {
                    AuthorId = int.Parse(authorId),
                    NewsTypeId = int.Parse(newsType), // Chú ý: Bạn cần đảm bảo newsType có giá trị hợp lệ
                    Title = model.Title.Trim(),
                    Content = model.Content,
                    PublishDate = model.PublishDate,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now,
                    Order = model.Order,
                };
            
                if (image != null && image.Length > 0)
                {
                    if (image.Length > 2 * 1024 * 1024)
                    {
                    TempData["ErrorMessageImage"] = "The uploaded image is too large. It should be 2MB or less.";
                    return RedirectToAction("CreateNews");
                    }

                    string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                    string fileExtension = Path.GetExtension(image.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                    TempData["ErrorMessageImage"] = "Only .png, .jpeg, .jpg files are allowed.";
                    return RedirectToAction("CreateNews");
                    }



                // Generate a unique file name for the image
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                // Combine the constant folder path and file name
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGENEWS, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                newss.Image = Path.Combine(Constant.IMAGENEWS, fileName).Replace('\\', '/'); // Store the file path in the database
            }
            await _context.News.AddAsync(newss);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListNews");
        }



        [HttpGet]
        public IActionResult DeleteNews(int id)
        {
            if (ModelState.IsValid)
            {
                var news = _context.News.Where(x => x.NewsId == id).FirstOrDefault();
                news.DeleteFlag = true;
                _context.News.Update(news);
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListNews");
        }


        [HttpGet]
        public ActionResult ViewDetails(int id)
        {
            if (ModelState.IsValid)
            {
                var news = _context.News.Where(x => x.NewsId == id && x.DeleteFlag == false).Include(x => x.NewsType).Include(x => x.Author)
            .OrderBy(p => p.NewsId).FirstOrDefault();

                if (news == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                return View(news);
            }
            return RedirectToAction("RecordNotFound");
        }








        [HttpGet]
        public ActionResult UpdateNews(int id)
        {
            var news = _context.News.Where(x => x.NewsId == id && x.DeleteFlag == false).FirstOrDefault();

            if (news == null)
            {
                return RedirectToAction("RecordNotFound");
            }

            var newTypes = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();
            var newTypesSelectList = newTypes.Select(newTypes => new SelectListItem
            {
                Value = newTypes.NewsTypeId.ToString(),
                Text = newTypes.NewsTypeName,
                Selected = newTypes.NewsTypeId == news.NewsTypeId
            }).ToList();
            ViewBag.NewsTypeOptions = newTypesSelectList;




            if (ModelState.IsValid)
            {
                return View(news);
            }
            return RedirectToAction("RecordNotFound");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateNews(News model, IFormFile? NewsImage)
        {

            var newsDB = _context.News.Where(x => x.DeleteFlag == false).ToList();
            var news = newsDB.Where(x => x.NewsId == model.NewsId).FirstOrDefault();
            var newTypes = _context.NewsTypes.Where(x => x.DeleteFlag == false).ToList();
            if (news == null)
            {
                return RedirectToAction("RecordNotFound");
            }

            var authorId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);

            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var type = newTypes.FirstOrDefault(e => e.NewsTypeId == model.NewsTypeId);
            if (type.NewsTypeName.Equals(Constant.FOOTER))
            {
                if (!model.Order.HasValue)
                {
                    ModelState.AddModelError("Order", "The Order Require");
                    return View(model);
                }
                if (newsDB.Any(e => e.NewsType.NewsTypeName.Equals(Constant.FOOTER) && (model.Order.HasValue? e.Order == model.Order : false) && e.NewsId != news.NewsId))
                {
                    ModelState.AddModelError("Order", "The Order Duplicate");
                    return View(model);
                }
            }
            else if (type.NewsTypeName.Equals(Constant.MENU))
            {
                if (!model.Order.HasValue)
                {
                    ModelState.AddModelError("Order", "The Order Require");
                    return View(model);
                }
                if (newsDB.Any(e => e.NewsType.NewsTypeName.Equals(Constant.MENU) && (model.Order.HasValue ? e.Order == model.Order : false) && e.NewsId != news.NewsId))
                {
                    ModelState.AddModelError("Order", "The Order Duplicate");
                    return View(model);
                }
            }


            news.AuthorId = int.Parse(authorId);
            news.NewsTypeId = model.NewsTypeId;
            news.Title = model.Title.Trim();
            news.Content = model.Content;
            news.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
            news.ModifiedOn = DateTime.Now;
            news.Order = model.Order;
            if (NewsImage != null && NewsImage.Length > 0)
            {

                if (NewsImage.Length > 2 * 1024 * 1024)
                {
                    TempData["ErrorMessageImage"] = "The uploaded image is too large. It should be 2MB or less.";
                    return RedirectToAction("CreateNews");
                }

                string[] allowedExtensions = { ".png", ".jpeg", ".jpg" };
                string fileExtension = Path.GetExtension(NewsImage.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessageImage"] = "Only .png, .jpeg, .jpg files are allowed.";
                    return RedirectToAction("CreateNews");
                }


                // Delete the image file
                if (!string.IsNullOrEmpty(news.Image))
                {
                    var imagePath = Path.Combine(Constant.PATH, news.Image);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                // Generate a unique file name for the image
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NewsImage.FileName);

                // Combine the constant folder path and file name
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), Constant.PATH + Constant.IMAGENEWS, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await NewsImage.CopyToAsync(fileStream);
                }

                news.Image = Path.Combine(Constant.IMAGENEWS, fileName).Replace('\\', '/'); // Store the file path in the database
            }

            _context.News.Update(news);
            _context.SaveChanges();

            return RedirectToAction("ListNews");
        }

        [HttpPost]
        public IActionResult DeleteListNews(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var newsType = _context.News.Find(id);
                    if (newsType != null)
                    {
                        newsType.DeleteFlag = true;
                        _context.News.Update(newsType);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListNews");
        }


    }

}


