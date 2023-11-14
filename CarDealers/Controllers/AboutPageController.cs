using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealers.Controllers
{
    public class AboutPageController : CustomController
    {
        private readonly CarDealersContext _context;

        public AboutPageController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.CUSTOMER_ROLE, Constant.GUEST_ROLE };
        }

        public IActionResult Index()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            ViewBag.FooterText = GetDefaultFooterText();
            ViewBag.Menu = GetDefaultMenu();
            return View();
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
