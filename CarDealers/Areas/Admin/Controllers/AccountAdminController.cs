using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    public class AccountAdminController : CustomController
    {
        private readonly CarDealersContext _context;

        public AccountAdminController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.GUEST_ROLE };
        }
        [Route("login")]
        public IActionResult LoginAdmin()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }

            //Logic 
            return View();
        }

        [Route("login")]
        [HttpPost]
        public IActionResult LoginAdmin(string username, string password)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill username and password.";
                return View();
            }
            else
            {
                var customer = _context.Users.Where(x => x.Username == username && x.DeleteFlag == false).FirstOrDefault();
                if (customer != null && HashPassword.GetMD5(password) == customer.Password && customer.Status == 1)
                {
                    HttpContext.Session.SetString(Constant.LOGIN_USERID_SESSION_NAME, customer.UserId.ToString());
                    HttpContext.Session.SetString(Constant.LOGIN_USERROLE_SESSION_NAME, Constant.ADMIN_ROLE);
                    HttpContext.Session.SetString(Constant.LOGIN_USERNAME_SESSION_NAME, customer.Username.ToString());                                       
                    return Redirect(Constant.DEFAULT_ADMIN_PAGE);
                }
                else
                {
                    TempData["ErrorMessage"] = "Incorrect username or password.";
                    return View();
                }
            }
        }

        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Redirect(Constant.LOGIN_ADMIN_PAGE);
        }
    }
}
