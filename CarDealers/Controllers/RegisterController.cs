using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace CarDealers.Controllers
{
    public class RegisterController : CustomController
    {
        private readonly CarDealersContext _context;

        public RegisterController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.GUEST_ROLE };
        }

        public IActionResult Register()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }

            //Logic 
            return View(new Customer());
        }

        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            using (CarDealersContext context = new CarDealersContext())
                try
                {
                    bool isError = true;
                    //check errror
                    if (string.IsNullOrEmpty(customer.FullName) || string.IsNullOrEmpty(customer.Username) || string.IsNullOrEmpty(customer.Password) ||
                        string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.Address) || string.IsNullOrEmpty(customer.PhoneNumber))
                    {
                        TempData["ErrorMessage"] = "Please fill all information.";
                        isError = false;
                    }
                    else
                    {
                        if (!Validation.CheckPassword(customer.Password))
                        {
                            TempData["ErrorMessageUsername"] = "Password must have at least 6 character, contains special characters, uppercase letters, lowercase letters and numbers ";
                            isError = false;
                        }
                        if (!Validation.CheckEmail(customer.Email))
                        {
                            TempData["ErrorMessageEmail"] = "Email must correct with format";
                            isError = false;
                        }
                        if (context.Customers.Any(c => c.Email == customer.Email))
                        {
                            TempData["ErrorMessageEmail"] = "Email is already in use. Please choose a different email.";
                            isError = false;
                        }
                        if (context.Customers.Any(c => c.Username == customer.Username))
                        {
                            TempData["ErrorMessageUsername"] = "Username is already in use. Please choose a different username.";
                            isError = false;
                        }
                        if (!Validation.CheckPhone(customer.PhoneNumber))
                        {
                            TempData["ErrorMessagePhoneNumber"] = "Please check phonenumber again";
                            isError = false;
                        }
                    }
                    // if has error
                    if (!isError)
                    {
                        return View(customer);
                    }

                    //if no error 
                    customer.Status = 1;
                    customer.CustomerType = 2;
                    customer.DeleteFlag = false;
                    customer.Password = HashPassword.GetMD5(customer.Password);
                    customer.CreatedOn = DateTime.Today;
                    customer.Ads = true;
                    _context.Customers.Add(customer);
                    _context.SaveChanges();


                    //set session
                    HttpContext.Session.SetString(Constant.LOGIN_USERID_SESSION_NAME, customer.CustomerId.ToString());
                    HttpContext.Session.SetString(Constant.LOGIN_USERROLE_SESSION_NAME, Constant.CUSTOMER_ROLE);
                    HttpContext.Session.SetString(Constant.LOGIN_USERNAME_SESSION_NAME, customer.Username.ToString());

                }
                catch (Exception)
                {

                    throw;
                }
            return Redirect(Constant.DEFAULT_CUSTOMER_PAGE);
        }
    }
}
