using CarDealers.Areas.Admin.Models.CustomerViewModel;
using CarDealers.Entity;
using CarDealers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using static CarDealers.Areas.Admin.Controllers.SendEmailController;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class CustomerController : CustomController
    {
        private readonly CarDealersContext _context;

        public CustomerController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GeneratePassword(int length)
        {
            var random = new Random();
            var password = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(AllowedChars.Length);
                password.Append(AllowedChars[index]);
            }

            return password.ToString();
        }

        public IActionResult ListCustomer(int? page, int? pageSize, string Keyword)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.Customers.Where(x => x.DeleteFlag == false).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.FullName.ToLower().Contains(Keyword.Trim().ToLower())
                || Keyword.Trim().ToLower().Contains(u.FullName.ToLower())
                || keywords.Any(e => u.FullName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var customers = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            var listCustomers = customers.Select(e => new ListCustomerViewModel
            {
                CustomerId = e.CustomerId,
                FullName = e.FullName,
                PhoneNumber = e.PhoneNumber,
                Email = e.Email
            }).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.Keyword = Keyword;

            ViewBag.PageSizeList = new List<int> { 5, 10, 20, 50 }; // Add other values as needed

            return View("ListCustomer", listCustomers);
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCustomer(CreateCustomerViewModel model, string formaction)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var customers = _context.Customers.Where(x => x.DeleteFlag == false).ToList();
            var customersAll = _context.Customers.ToList();
            string originPassword = "";
            Boolean gender = true;

            //check full name
            if (model.FullName == null)
            {
                ModelState.AddModelError("FullName", "Name cannot null.");
                return View(model);
            }
            //check email
            if (model.Email == null)
            {
                ModelState.AddModelError("Email", "Email cannot null.");
                return View(model);
            }
            else if (customersAll.Any(x => x.Email.Equals(model.Email)))
            {
                ModelState.AddModelError("Email", "The Email already exists. Please choose another.");
                return View(model);
            }
            //check phone number
            if (model.PhoneNumber == null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number cannot null");
                return View(model);
            }
            //check gender
            if (model.Gender.IsNullOrEmpty())
            {
                ModelState.AddModelError("Gender", "The Gender cannot empty. Please choose a Gender.");
                return View(model);
            }
            //check status
            if (model.Status == null)
            {
                ModelState.AddModelError("Status", "The Status cannot empty. Please choose a Status.");
                return View(model);
            }
            //valid gender
            if (model.Gender.Equals("male"))
            {
                gender = true;
            }
            else
            {
                gender = false;
            }

            //create account
            if (formaction == "CreateCustomerAccount")
            {
                if (customersAll.Where(x => x.CustomerType == 2).Any(x => x.PhoneNumber.Equals(model.PhoneNumber)))
                {
                    ModelState.AddModelError("PhoneNumber", "The Phone Number already exists. Please choose a different PhoneNumber.");
                    return View(model);
                }
                model.CustomerType = 2;
                model.Username = model.Email;
                originPassword = GeneratePassword(7);
                model.Password = HashPassword.GetMD5(originPassword);//Generate Password
            }
            else if (formaction == "SaveInformation")
            {
                model.CustomerType = 1;
            }

            //valid and input model to db
            if (ModelState.IsValid)
            {

                var customer = new Customer
                {
                    Dob = DateTime.ParseExact(model.Dob, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    Gender = gender,
                    Status = model.Status,
                    Username = model.Username,
                    Password = model.Password,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    CarId = model.CarId,
                    Kilometerage = model.Kilometerage,
                    PlateNumber = model.PlateNumber,
                    CustomerType = model.CustomerType,
                    CreatedBy = customerId != null ? int.Parse(customerId) : null,
                    CreatedOn = DateTime.Now
                };
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();


                if (customer.CustomerType == 2)
                {
                    var customerEmail = new EmailModel()
                    {
                        To = customer.Email,
                        Body = originPassword,
                        Subject = "password for our car dealer website"
                    };

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
                                Subject = customerEmail.Subject,
                                Body = customerEmail.Body,
                            };
                            mailMessage.To.Add(customer.Email);

                            smtpClient.Send(mailMessage);
                        }

                        ViewBag.Message = "Email sent successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = $"Failed to send email: {ex.Message}";
                    }
                }

                return RedirectToAction("ListCustomer");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            if (ModelState.IsValid)
            {
                var customer = _context.Customers.Where(x => x.CustomerId == id).FirstOrDefault();
                customer.DeleteFlag = true;
                customer.Status = 0;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCustomer");
        }

        [HttpPost]
        public IActionResult DeleteListCustomer(List<int> selectedIds)
        {
            if (ModelState.IsValid)
            {
                string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
                foreach (var id in selectedIds)
                {
                    var customer = _context.Customers.Find(id);
                    if (customer != null)
                    {
                        customer.DeleteFlag = true;
                        customer.Status = 0;
                        customer.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        customer.ModifiedOn = DateTime.Now;

                        _context.Customers.Update(customer);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListCustomer");
        }

        [HttpGet]
        public async Task<ActionResult> UpdateCustomer(int id)
        {
            var customer = _context.Customers.Where(x => x.CustomerId == id && x.DeleteFlag == false).FirstOrDefault();

            if (customer == null)
            {
                return RedirectToAction("RecordNotFound");
            }

            var updateCustomerViewModel = new UpdateCustomerViewModel
            {
                CustomerId = customer.CustomerId,
                Dob = customer.Dob?.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                Gender = customer.Gender,
                Status = customer.Status,
                CarId = customer.CarId,
                Username = customer.Username,
                Password = customer.Password,
                Email = customer.Email,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
                Kilometerage = customer.Kilometerage,
                PlateNumber = customer.PlateNumber,
                CustomerType = customer.CustomerType
            };

            return View(updateCustomerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCustomer(UpdateCustomerViewModel model, string formaction)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var customers = _context.Customers.Where(x => x.DeleteFlag == false).ToList();
            var customersAll = _context.Customers.ToList();
            var customer = customers.Where(x => x.CustomerId == model.CustomerId).FirstOrDefault();
            string originPassword = "";

            //name
            if (model.FullName == null)
            {
                ModelState.AddModelError("FullName", "Name cannot null.");
                return View(model);
            }
            //email
            if (model.Email == null)
            {
                ModelState.AddModelError("Email", "Email cannot null.");
                return View(model);
            }
            if (customersAll.Where(x => x.CustomerId != customer.CustomerId).Any(x => x.Email.Equals(model.Email)))//any delete flag
            {
                ModelState.AddModelError("Email", "The Email already exists. Please choose another.");
                return View(model);
            }
            //phone number
            if (model.PhoneNumber == null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number cannot null.");
                return View(model);
            }
            //guest turn to customer
            if (formaction == "CreateCustomerAccount")//not create account
            {
                if (customer.CustomerType == 1)
                {
                    //check sdt guest to guest
                    if (customersAll.Where(x => x.CustomerType == 2).Any(x => x.PhoneNumber.Equals(model.PhoneNumber)))//require phone number
                    {
                        ModelState.AddModelError("PhoneNumber", "The Phone Number already existed. Please choose a different Phone Number.");
                        customer.CustomerType = 1;//set to guest
                        return View(model);
                    }
                    customer.CustomerType = 2;//become customer
                    model.Username = model.Email;
                    originPassword = GeneratePassword(7);
                    model.Password = HashPassword.GetMD5(originPassword);//Generate Password

                    // change booking
                    var bookings = _context.BookingServices.Include(x => x.Customer).Where(e => e.DeleteFlag == false && (e.CustomerId == customer.CustomerId || e.Customer.PhoneNumber.Equals(customer.PhoneNumber)));
                    var booking = bookings.FirstOrDefault(e => e.CustomerId == customer.CustomerId);
                    if (booking?.CustomerParentId != null)
                    {
                        if (!booking.CustomerParentId.HasValue)
                        {
                            var updateBookings = bookings.Select(e => new BookingService
                            {
                                BookingId = e.BookingId,
                                DateBooking = e.DateBooking,
                                Status = e.Status,
                                Note = e.Note,
                                CustomerParentId = customer.CustomerId,
                                CustomerId = e.CustomerId
                            });
                            _context.BookingServices.UpdateRange(updateBookings);
                            await _context.SaveChangesAsync();
                        }
                    }

                    //send email
                    var customerEmail = new EmailModel()
                    {
                        To = customer.Email,
                        Body = originPassword,
                        Subject = "Password: "
                    };

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
                                Subject = customerEmail.Subject,
                                Body = customerEmail.Body,
                            };
                            mailMessage.To.Add(customer.Email);

                            smtpClient.Send(mailMessage);
                        }

                        ViewBag.Message = "Email sent successfully";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Error = $"Failed to send email: {ex.Message}";
                    }
                }
                else
                {
                    ModelState.AddModelError("Username", "Account already created");
                    return View(model);
                }
            }
            else if (formaction == "SaveInformation")//customer save info
            {
                if (customer.CustomerType == 2)
                {
                    //check sdt guest to guest
                    if (customersAll.Where(x => x.CustomerType == 2 && x.CustomerId != customer.CustomerId).Any(x => x.PhoneNumber.Equals(model.PhoneNumber)))//require phone number
                    {
                        ModelState.AddModelError("PhoneNumber", "The Phone Number already existed. Please choose a different Phone Number.");
                        return View(model);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                customer.Dob = DateTime.ParseExact(model.Dob, "dd-MM-yyyy", CultureInfo.InvariantCulture); ;
                customer.Gender = model.Gender;
                customer.Status = model.Status;
                customer.Username = model.Username;
                customer.Password = model.Password;
                customer.Email = model.Email;
                customer.FullName = model.FullName;
                customer.PhoneNumber = model.PhoneNumber;
                customer.Address = model.Address;
                customer.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                customer.ModifiedOn = DateTime.Now;

                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListCustomer");
            }
            return View(model);
        }

    }
}
