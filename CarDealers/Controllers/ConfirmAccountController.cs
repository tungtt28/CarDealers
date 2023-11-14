using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace CarDealers.Controllers
{
    public class ConfirmAccountController : Controller
    {
        private readonly CarDealersContext _context;

        public ConfirmAccountController(CarDealersContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult ConfirmCode()
        {
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
                return View();
            }

            DateTime expiryTime = DateTime.Parse(storedOtpExpiry);
            if (DateTime.Now > expiryTime)
            {
                TempData["ErrorMessage"] = "The OTP has expired";
                return View();
            }

            if (otp == storedOtp)
            {                
                return Redirect(Constant.DEFAULT_CUSTOMER_PAGE);
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
            string email = HttpContext.Session.GetString("EmailVerify");
            var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (customer != null)
            {
                string confirmationCode = GenerateOTP();
                HttpContext.Session.SetString("ConfirmationCode", confirmationCode);
                HttpContext.Session.SetString("ConfirmationCodeExpiry", DateTime.Now.AddMinutes(1).ToString("o"));

                SendConfirmationCodeEmail(email, confirmationCode);
                return View("ConfirmCode");
            }
            return Redirect("/ConfirmCode/Index");
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
                HttpContext.Session.SetString("EmailVerify", email);

                SendConfirmationCodeEmail(email, confirmationCode);
                return RedirectToAction("ConfirmCode");
            }
            else
            {
                TempData["ErrorMessage"] = "Email not found in the system.";
                return View();
            }
        }

        private void SendConfirmationCodeEmail(string email, string confirmationCode)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("quochuycode0209@gmail.com", "mfus xank wtah rywh"),
                    EnableSsl = true,
                };
                using (var dbContext = new CarDealersContext())
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("quochuycode0209@gmail.com"),
                        Subject = "Verify email",
                        Body = "Your verify code is: " + confirmationCode,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(email);

                    smtpClient.Send(mailMessage);
                }
                ViewBag.Message = "Verify code sent successfully";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to send verify code: {ex.Message}";
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
    }
}
