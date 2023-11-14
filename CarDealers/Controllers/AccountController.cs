using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;

namespace CarDealers.Controllers
{
    public class AccountController : CustomController
    {
        private readonly CarDealersContext _context;

        public AccountController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.GUEST_ROLE };
        }

        public IActionResult Login()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            //Logic 
            return View();

        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill username and password.";
                return View();
            }
            else
            {
                var c = _context.Customers.Where(x => x.Username == username && x.DeleteFlag == false).FirstOrDefault();
                //successs
                if (c != null && HashPassword.GetMD5(password) == c.Password && c.Status == 1)
                {
                    HttpContext.Session.SetString(Constant.LOGIN_USERID_SESSION_NAME, c.CustomerId.ToString());
                    HttpContext.Session.SetString(Constant.LOGIN_USERROLE_SESSION_NAME, Constant.CUSTOMER_ROLE);
                    HttpContext.Session.SetString(Constant.LOGIN_USERNAME_SESSION_NAME, c.Username.ToString());
                    return Redirect(Constant.DEFAULT_CUSTOMER_PAGE);
                }
                else
                {
                    TempData["ErrorMessage"] = "Incorrect username or password.";
                    return View();
                }
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}
