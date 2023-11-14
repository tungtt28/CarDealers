using CarDealers.Entity;
using CarDealers.Models.TrialDrivingModel;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarDealers.Controllers
{
    public class TrialDrivingController : CustomController
    {
        private readonly CarDealersContext _context;

        public TrialDrivingController(CarDealersContext context)
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
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == Converter.ParseInt(customerId));
            var cars = _context.Cars.ToList();
            ViewBag.Cars = cars;
            ViewBag.Customer = customer;
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

        [HttpPost]
        public IActionResult RegisterTrial(TrialDrivingViewModel trialDriving)
        {
            using (CarDealersContext context = new CarDealersContext())
            {
                try
                {
                    bool isError = false;
                    //check errror
                    if (string.IsNullOrEmpty(trialDriving.FullName) || string.IsNullOrEmpty(trialDriving.DriverLicense) ||
                        string.IsNullOrEmpty(trialDriving.Email) || string.IsNullOrEmpty(trialDriving.PhoneNumber))
                    {
                        TempData["ErrorMessage"] = "Please fill all information.";
                        isError = true;
                    }
                    else
                    {
                        if (!Validation.CheckPhone(trialDriving.PhoneNumber))
                        {
                            TempData["ErrorMessagePhoneNumber"] = "Please check phonenumber again";
                            isError = true;
                        }
                        if (!Validation.CheckEmail(trialDriving.Email))
                        {
                            TempData["ErrorMessageEmail"] = "Email must correct with format";
                            isError = true;
                        }
                        if (!Validation.CheckDriverLicense(trialDriving.DriverLicense))
                        {
                            TempData["ErrorMessageDriverLicense"] = "Please check driver license again";
                            isError = true;
                        }
                        if (!Validation.CheckBookingDate(trialDriving.DateBooking))
                        {
                            TempData["ErrorMessageDateBooking"] = "Please check date booking again";
                            isError = true;
                        }
                    }
                    if (!isError)
                    {
                        var listCarTrial = _context.TrialDrivings.Include(x => x.CarTrail)
                            .Where(x => x.DateBooking == trialDriving.DateBooking && x.CarTrail.CarId == trialDriving.CarId)
                            .Select(x => x.CarTrailId).ToList();
                        var carTrial = new CarTrial();
                        if (listCarTrial == null)
                        {
                            carTrial = _context.CarTrials.FirstOrDefault(carTrial => carTrial.Status == 1 && carTrial.CarId == trialDriving.CarId);
                        }
                        else
                        {
                            carTrial = _context.CarTrials.FirstOrDefault(carTrial => !listCarTrial.Any(x => x == carTrial.CarTrialId) && carTrial.Status == 1 && carTrial.CarId == trialDriving.CarId);
                        }
                        if (carTrial != null)
                        {
                            var newTrialDriving = new TrialDriving()
                            {
                                Email = trialDriving.Email,
                                FullName = trialDriving.FullName,
                                PhoneNumber = trialDriving.PhoneNumber,
                                CarTrailId = carTrial.CarTrialId,
                                DateBooking = trialDriving.DateBooking,
                                DriverLicense = trialDriving.DriverLicense,
                                Request = trialDriving.Request,
                                Status = 0,
                                DeleteFlag = false,
                                CreatedOn = DateTime.Now
                            };
                            _context.TrialDrivings.Add(newTrialDriving);
                            _context.SaveChanges();
                            TempData["SuccessMessage"] = "Submited successfully!";

                            string receiver = trialDriving.Email;
                            string subject = "Schedule a trial driving";
                            var carName = _context.Cars.FirstOrDefault(x => x.CarId == carTrial.CarId);
                            string body = $"You have scheduled a test drive for {carName.Model}<br />Time: {trialDriving.DateBooking.ToString("dd/MM/yyyy")}<br />Please wait for a confirmation email from us."; SendMail.SendEmail(receiver, subject, body);
                        }
                        else
                        {
                            TempData["ErrorFullMessage"] = "This car model is fully booked on the day you booked";
                            isError = true;
                        }
                    }
                    // if has error
                    if (isError)
                    {
                        var cars = _context.Cars.ToList();
                        ViewBag.Cars = cars;
                        return View("Index", trialDriving);
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return Redirect(Constant.TRIAL_PAGE);
        }

    }
}
