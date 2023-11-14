using CarDealers.Entity;
using CarDealers.Areas.Admin.Models.EmployeeViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Data;
using System.Text;
using System.Globalization;
using static CarDealers.Areas.Admin.Controllers.SendEmailController;
using CarDealers.Util;
using System.Net.Mail;
using System.Net;
using System.Drawing.Drawing2D;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class EmployeeController : CustomController
    {
        private readonly CarDealersContext _context;

        public EmployeeController(CarDealersContext context)
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

        public IActionResult ListEmployee(int? page, int? pageSize, string Keyword)
        {
			if (!HasAuthorized())
			{
				return RedirectBaseOnPage();
			}
			int defaultPageSize = 5; // Set the number of records per page
            int pageNumber = (page ?? 1); // If no page number is specified, default to 1
            int recordsPerPage = pageSize ?? defaultPageSize;

            //fetch data from database table orders
            var query1 = _context.Users.Where(x => x.DeleteFlag == false).ToList();

            if (!string.IsNullOrEmpty(Keyword))
            {
                var keywords = Keyword.Trim().ToLower().Split(" ").ToList();
                query1 = query1.Where(u => u.FullName.ToLower().Contains(Keyword.Trim().ToLower())
                || Keyword.Trim().ToLower().Contains(u.FullName.ToLower())
                || keywords.Any(e => u.FullName.ToLower().Contains(e.Trim().ToLower()))).ToList();
            }
            var query = query1.AsQueryable();
            var employees = query.Skip((pageNumber - 1) * (pageSize.HasValue ? pageSize.Value : defaultPageSize)).Take((pageSize.HasValue ? pageSize.Value : defaultPageSize)).ToList();

            var listEmployees = employees.Select(e => new ListEmployeeViewModel
            {
                EmployeeId = e.UserId,
                Email = e.Email,
                FullName = e.FullName,
                PhoneNumber = e.PhoneNumber
            }).ToList();

            // Pass the records and page information to the view
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = recordsPerPage;
            ViewBag.TotalRecords = query.Count();
            ViewBag.Keyword = Keyword;

            ViewBag.PageSizeList = new List<int> { 2, 5, 10, 20, 50 }; // Add other values as needed

            return View("ListEmployee", listEmployees);
        }

        public ActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEmployee(CreateEmployeeViewModel model)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var employees = _context.Users.Where(x => x.DeleteFlag == false).ToList();
            var employeesAll = _context.Users.ToList();
            var roleEmployee = _context.UserRoles.Where(r => r.RoleName.ToLower().Equals("employee")).FirstOrDefault();
            int roleEmployeeId = roleEmployee.UserRoleId;
            string originPassword = "";

            //Name
            if (model.FullName == null)
            {
                ModelState.AddModelError("FullName", "Name cannot null.");
                return View(model);
            }
            //Email
            if (model.Email == null)
            {
                ModelState.AddModelError("Email", "Email cannot null.");
                return View(model);
            }
            if (employeesAll.Any(x => x.Email.Equals(model.Email)))
            {
                ModelState.AddModelError("Email", "The Email already exists. Please choose another.");
                return View(model);
            }
            //Phone
            if (model.PhoneNumber == null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number cannot null.");
                return View(model);
            }
            if (employeesAll.Any(x => x.PhoneNumber.Equals(model.PhoneNumber)))
            {
                ModelState.AddModelError("PhoneNumber", "The Phone Number already exists. Please choose a different PhoneNumber.");
                return View(model);
            }
            //Username
            model.Username = model.Email;
            //Password
            originPassword = GeneratePassword(7);
            model.Password = HashPassword.GetMD5(originPassword);//Generate Password
            //Address
            if (model.Address == null)
            {
                ModelState.AddModelError("Address", "Address cannot null.");
                return View(model);
            }

            var employee = new User();
            if (ModelState.IsValid)
            {
                employee.Dob = DateTime.ParseExact(model.Dob, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                employee.UserRoleId = roleEmployeeId;
                employee.Gender = model.Gender;
                employee.Status = model.Status;
                employee.Username = model.Username;
                employee.Password = model.Password;
                employee.Email = model.Email;
                employee.FullName = model.FullName;
                employee.PhoneNumber = model.PhoneNumber;
                employee.Address = model.Address;
                employee.CreatedBy = customerId != null ? int.Parse(customerId) : null;
                employee.CreatedOn = DateTime.Now;

                await _context.Users.AddAsync(employee);
                await _context.SaveChangesAsync();

                //send email
                var customerEmail = new EmailModel()
                {
                    To = employee.Email,
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
                        mailMessage.To.Add(employee.Email);

                        smtpClient.Send(mailMessage);
                    }

                    ViewBag.Message = "Email sent successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Failed to send email: {ex.Message}";
                }

                return RedirectToAction("ListEmployee");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> UpdateEmployee(int id)
        {
            var roleEmployee = _context.UserRoles.Where(r => r.RoleName.ToLower().Equals("employee")).FirstOrDefault();
            int roleEmployeeId = roleEmployee.UserRoleId;
            var employee = _context.Users.Where(x => x.UserId == id && x.DeleteFlag == false).FirstOrDefault();

            if (employee == null)
            {
                return RedirectToAction("RecordNotFound");
            }

            var updateEmployeeViewModel = new UpdateEmployeeViewModel
            {
                EmployeeId = employee.UserId,
                UserRoleId = employee.UserRoleId,
                Dob = employee.Dob?.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                Gender = employee.Gender,
                Status = employee.Status,
                Username = employee.Username,
                Password = employee.Password,
                Email = employee.Email,
                FullName = employee.FullName,
                PhoneNumber = employee.PhoneNumber,
                Address = employee.Address
            };

            ViewBag.RoleEmployeeId = roleEmployeeId;

            return View(updateEmployeeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateEmployee(UpdateEmployeeViewModel model)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            var employees = _context.Users.Where(x => x.DeleteFlag == false).ToList();
            var employeesAll = _context.Users.ToList();
            var employee = employees.Where(x => x.UserId == model.EmployeeId).FirstOrDefault();
            string originPassword = "";

            //Name
            if (model.FullName == null)
            {
                ModelState.AddModelError("FullName", "Name cannot null.");
                return View(model);
            }
            //Email
            if (model.Email == null)
            {
                ModelState.AddModelError("Email", "Email cannot null.");
                return View(model);
            }
            if (employeesAll.Where(x => x.UserId != employee.UserId).Any(x => x.Email.Equals(model.Email)))
            {
                ModelState.AddModelError("Email", "The Email already exists. Please choose another.");
                return View(model);
            }
            //Phone
            if (model.PhoneNumber == null)
            {
                ModelState.AddModelError("PhoneNumber", "Phone Number cannot null.");
                return View(model);
            }
            if (employeesAll.Where(x => x.UserId != employee.UserId).Any(x => x.PhoneNumber.Equals(model.PhoneNumber)))
            {
                ModelState.AddModelError("PhoneNumber", "The Phone Number already exists. Please choose a different PhoneNumber.");
                return View(model);
            }
            //Address
            if (model.Address == null)
            {
                ModelState.AddModelError("Address", "Address cannot null.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                employee.FullName = model.FullName;
                employee.Email = model.Email;
                employee.PhoneNumber = model.PhoneNumber;
                employee.Gender = model.Gender;
                employee.Address = model.Address;
                employee.Status = model.Status;
                employee.Username = model.Username;
                employee.Password = model.Password;
                employee.Dob = DateTime.ParseExact(model.Dob, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                employee.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                employee.ModifiedOn = DateTime.Now;

                _context.Users.Update(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListEmployee");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteEmployee(int id)
        {
            if (ModelState.IsValid)
            {
                var employee = _context.Users.Where(x => x.UserId == id).FirstOrDefault();
                employee.DeleteFlag = true;
                employee.Status = 0;
                _context.Users.Update(employee);
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListEmployee");
        }

        [HttpPost]
        public IActionResult DeleteListEmployee(List<int> selectedIds)
        {
            string customerId = HttpContext.Session.GetString(Constant.LOGIN_USERID_SESSION_NAME);
            if (ModelState.IsValid)
            {
                foreach (var id in selectedIds)
                {
                    var employee = _context.Users.Find(id);
                    if (employee != null)
                    {
                        employee.DeleteFlag = true;
                        employee.Status = 0;
                        employee.ModifiedBy = customerId != null ? int.Parse(customerId) : null;
                        employee.ModifiedOn = DateTime.Now;
                        _context.Users.Update(employee);
                    }
                }
                _context.SaveChanges();
            }
            else
            {
                return RedirectToAction("RecordNotFound");
            }
            return RedirectToAction("ListEmployee");
        }

    }
}