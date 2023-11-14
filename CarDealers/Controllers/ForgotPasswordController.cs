using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace CarDealers.Controllers
{
    public class ForgotPasswordController : CustomController
    {
        private readonly CarDealersContext _context;

        public ForgotPasswordController(CarDealersContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendConfirmationCode(string email)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer != null)
            {
                string confirmationCode = GenerateOTP();
                HttpContext.Session.SetString("ConfirmationCode", confirmationCode);
                HttpContext.Session.SetString("ConfirmationCodeExpiry", DateTime.Now.AddMinutes(1).ToString("o"));
                HttpContext.Session.SetString("EmailChangePass", email);

                SendConfirmationCodeEmail(email, confirmationCode);
                return RedirectToAction("ConfirmCode");
            }
            else
            {
                TempData["ErrorMessage"] = "Email not found in the system.";
                return View("Index");
            }
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            int otpLength = 6;
            string otp = "";
            for (int i = 0; i < otpLength; i++)
            {
                otp += random.Next(0, 10);
            }

            return otp;
        }

        [HttpGet]
        public IActionResult ConfirmCode()
        {
            string email = HttpContext.Session.GetString("EmailChangePass");
            if (email == null)
            {
                return Redirect("/ForgotPassword");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmCode(string otp)
        {
            string storedOtp = HttpContext.Session.GetString("ConfirmationCode");
            string storedOtpExpiry = HttpContext.Session.GetString("ConfirmationCodeExpiry");
            if (string.IsNullOrEmpty(storedOtp) || string.IsNullOrEmpty(storedOtpExpiry))
            {
                TempData["ErrorMessage"] = "OTP not exist";
                return RedirectToAction("SendConfirmationCode");
            }

            DateTime expiryTime = DateTime.Parse(storedOtpExpiry);
            if (DateTime.Now > expiryTime)
            {
                TempData["ErrorMessage"] = "The OTP has expired";
                return View();
            }

            if (otp == storedOtp)
            {
                string hasAccess = "true";
                HttpContext.Session.SetString("CheckAccessResetPassword", hasAccess);
                return RedirectToAction("ResetPassword");
            }
            else
            {
                TempData["ErrorMessage"] = "OTP incorrect. Please try again";
                return View();
            }
        }

        [HttpPost]
        public IActionResult ResendCode()
        {
            string email = HttpContext.Session.GetString("EmailChangePass");
            var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer != null)
            {
                string confirmationCode = GenerateOTP();
                HttpContext.Session.SetString("ConfirmationCode", confirmationCode);
                HttpContext.Session.SetString("ConfirmationCodeExpiry", DateTime.Now.AddMinutes(1).ToString("o"));

                SendConfirmationCodeEmail(email, confirmationCode);
                return View("ConfirmCode");
            }
            return Redirect("/ForgotPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            string email = HttpContext.Session.GetString("EmailChangePass");
            string hasAccess = HttpContext.Session.GetString("CheckAccessResetPassword");
            string otp = HttpContext.Session.GetString("ConfirmationCode");
            if (email == null)
            {
                return Redirect("/ForgotPassword");
            }
            if (otp != null && hasAccess != "true")
            {
                return Redirect("/ForgotPassword/ConfirmCode");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            string email = HttpContext.Session.GetString("EmailChangePass");
            if (email == null)
            {
                return RedirectToAction("SendConfirmationCode");
            }
            var customer = _context.Customers.Where(c => c.Email == email).FirstOrDefault();
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Please fill all fields.";
                return View();
            }
            if (!Validation.CheckPassword(newPassword))
            {
                TempData["ErrorMessageNewPassword"] = "Password must have at least 6 character, contains special characters, uppercase letters, lowercase letters and numbers ";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessageConfirmPassword"] = "New passwords do not match.";
                return View();
            }

            customer.Password = HashPassword.GetMD5(newPassword);
            _context.SaveChanges();
            HttpContext.Session.Remove("EmailChangePass");
            HttpContext.Session.Remove("ConfirmationCode");
            HttpContext.Session.Remove("ConfirmationCodeExpiry");
            HttpContext.Session.Remove("CheckAccessResetPassword");
            TempData["SuccessMessage"] = "Password reset successfully!";
            return Redirect(Constant.LOGIN_PAGE);
        }      

        private void SendConfirmationCodeEmail(string email, string confirmationCode)
        {
            string body = "Your confirmation code is: " + confirmationCode;
            if(SendMail.SendEmail(email, "Password Reset Confirmation", body))
            {
                ViewBag.Message = "Confirmation code sent successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to send confirmation code";
            }
        }
    }
}

