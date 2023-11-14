using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealers.Controllers
{
    public class NewsPageController : CustomController
    {
        private readonly CarDealersContext _context;
        public NewsPageController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE, Constant.GUEST_ROLE };
        }
        public IActionResult News(int? page)
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            int pageSize = 6; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1

            //fetch data from database table orders
            var newsList = _context.News.Where(x => x.DeleteFlag == false && !x.NewsType.NewsTypeName.Equals(Constant.FOOTER) && !x.NewsType.NewsTypeName.Equals(Constant.MENU)).Include(x => x.NewsType).Include(x => x.Author).Include(x => x.Reviews)
            .OrderBy(p => p.NewsId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = _context.News.Count();
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View(newsList);
        }
        


        [HttpGet]
        public ActionResult ViewDetails(int id)
        {
            if (ModelState.IsValid)
            {
                var news = _context.News.Where(x => x.NewsId == id && x.DeleteFlag == false).Include(x => x.NewsType).Include(x => x.Author).Include(x => x.Reviews)
            .OrderBy(p => p.NewsId).FirstOrDefault();


                

                var customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);



                var reviews = _context.Reviews.Where(x => x.NewsId == news.NewsId && x.DeleteFlag == false).Include(x => x.Customer).OrderBy(p => p.PublishDate).ToList();
                if(customerId != null)
                {
                    ViewBag.CustomerID = int.Parse(customerId);
                }
               
                ViewBag.Reviews = reviews;






                var latestNews = GetLatestNews();
                ViewBag.LatestNews = latestNews;
                
                if (news == null)
                {
                    return RedirectToAction("RecordNotFound");
                }
                ViewBag.FooterText = GetDefaultFooterText();
                ViewBag.Menu = GetDefaultMenu();
                return View(news);
            }
            return RedirectToAction("RecordNotFound");
        }



        public IActionResult SearchNewsPage(int? page, string Keyword)
        {
            int pageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1

            //fetch data from database table orders
            var query1 = _context.News.Where(x => x.DeleteFlag == false).Include(x => x.NewsType).Include(x => x.Author).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.Title.ToLower().Contains(Keyword.Trim().ToLower()) || Keyword.Trim().ToLower().Contains(u.Title.ToLower()) || keywords.Any(e => u.Title.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var titles = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = _context.News.Where(x => x.DeleteFlag == false).Count();
            ViewBag.Keyword = Keyword;
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View("News", titles);
        }

        public List<News> GetLatestNews()
        {

            var latestNews = _context.News.Where(x => x.DeleteFlag == false && !x.NewsType.NewsTypeName.Equals(Constant.FOOTER) && !x.NewsType.NewsTypeName.Equals(Constant.MENU)).OrderByDescending(n => n.PublishDate).Take(3).ToList();
            return latestNews;
        }       




        [HttpPost]
        public IActionResult AddComment(Review model)
        {
            var review = _context.Reviews.Where(x => x.DeleteFlag == false).Include(x => x.Customer).ToList();
            var customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            if (string.IsNullOrEmpty(customerId))
            {

                return RedirectToAction("Login", "Account");
            }
            var reviews = new Review
            {
                NewsId = model.NewsId,
                CustomerId = int.Parse(customerId),
                Rating = model.Rating,
                Comment = model.Comment,
                 PublishDate = DateTime.Now,
                DeleteFlag = false,
                CreatedBy = customerId != null ? int.Parse(customerId) : null,
                CreatedOn = DateTime.Now,
            };
            _context.Reviews.Add(reviews);
            _context.SaveChanges();
            return Redirect("/NewsPage/ViewDetails?id=" + model.NewsId);
        }



        [HttpGet]
        public IActionResult Delete(int id)
        {
            var review = _context.Reviews.Find(id);

            if (review == null)
            {
                return RedirectToAction("RecordNotFound");
            }


            review.DeleteFlag = true;
            _context.Reviews.Update(review);
            _context.SaveChanges();

            return Redirect("/NewsPage/ViewDetails?id=" + review.NewsId);
        }



        [HttpGet]
        public IActionResult EditComment(int NewsId)
        {

            if (ModelState.IsValid)
            {
                Review rv = _context.Reviews.Where(p => p.ReviewId == NewsId).FirstOrDefault();
                
            if (rv == null)
            {
                return RedirectToAction("RecordNotFound");
            }
            return View(rv);
            }
            return RedirectToAction("RecordNotFound");

        }
        [HttpPost]
        public IActionResult EditComment(Review model)
        {
            var customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var reviews = new Review
            {
                ReviewId = model.ReviewId,
                NewsId = model.NewsId,
                CustomerId = int.Parse(customerId),
                Rating = model.Rating,
                Comment = model.Comment,
                PublishDate = DateTime.Now,
                DeleteFlag = false,
                ModifiedBy = customerId != null ? int.Parse(customerId) : null,
                ModifiedOn = DateTime.Now,
            };
           
            _context.Reviews.Update(reviews);
            _context.SaveChanges();
            return Redirect("/NewsPage/ViewDetails?id=" + model.NewsId);
        }







        protected List<News> GetDefaultFooterText()
        {
            var footers = _context.News.Include(e => e.NewsType).Where(e => e.DeleteFlag == false && e.NewsType.NewsTypeName.Equals(Constant.FOOTER)).OrderBy(e => e.Order).ToList();
            return footers;
        }

        protected List<News> GetDefaultMenu()
        {
            var menus = _context.News.Include(e => e.NewsType).Where(e => e.DeleteFlag == false && e.NewsType.NewsTypeName.Equals(Constant.MENU)).OrderBy(e => e.Order).ToList();
            return menus;
        }







    }
}
