using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using CarDealers.Util;
using CarDealers.Entity;

namespace CarDealers.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/[controller]/[action]")]
    public class SendEmailController : CustomController
    {
        private readonly CarDealersContext _context;

        public SendEmailController(CarDealersContext context)
        {
            _context = context;
            authorizedRoles = new string[] { Constant.ADMIN_ROLE };
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!HasAuthorized())
            {
                return RedirectBaseOnPage();
            }
            return View();
        }

        /*[HttpPost]
        public IActionResult SendEmailAction(EmailModel emailModel, string SendToAll, IFormFile attachment)
        {
            try
            {
                if (string.IsNullOrEmpty(emailModel.Subject) || string.IsNullOrEmpty(emailModel.Body))
                {
                    ViewBag.Error = "Subject and Body are required fields.";
                    return View("Index");
                }

                if (attachment != null && attachment.Length > 0)
                {
                    const long maxFileSize = 10 * 1024 * 1024;
                    if (attachment.Length > maxFileSize)
                    {
                        ViewBag.Error = "Attachment size exceeds the 10MB limit.";
                        return View("Index");
                    }

                    // Kiểm tra định dạng tệp đính kèm
                    string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".rar" };
                    string fileExtension = Path.GetExtension(attachment.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ViewBag.Error = "Only .pdf, .doc, .docx, .jpg, .jpeg, .png, .rar files are allowed for attachment.";
                        return View("Index");
                    }
                }               
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("quochuycode0209@gmail.com", "mfus xank wtah rywh"),
                    EnableSsl = true,
                };

                using (var dbContext = new CarDealersContext())
                {
                    var customers = dbContext.Customers.ToList();

                    if (SendToAll != "on")
                    {
                        customers = customers.Where(customer => customer.Ads).ToList();
                    }

                    foreach (var customer in customers)
                    {
                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress("quochuycode0209@gmail.com"),
                            Subject = emailModel.Subject,
                            Body = emailModel.Body,
                        };

                        mailMessage.To.Add(customer.Email);

                        if (attachment != null && attachment.Length > 0)
                        {
                            var attachmentStream = attachment.OpenReadStream();
                            var attachmentName = attachment.FileName;
                            mailMessage.Attachments.Add(new Attachment(attachmentStream, attachmentName));
                        }

                        smtpClient.Send(mailMessage);
                    }
                }

                ViewBag.Message = "Email sent successfully";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to send email: {ex.Message}";
            }

            return View("Index");
        }*/

        [HttpPost]
        public IActionResult SendEmailAction(EmailModel emailModel, string SendToAll, IFormFile attachment)
        {
            if (string.IsNullOrEmpty(emailModel.Subject) || string.IsNullOrEmpty(emailModel.Body))
            {
                ViewBag.Error = "Subject and Body are required fields.";
                return View("Index");
            }

            if (attachment != null && attachment.Length > 0)
            {
                const long maxFileSize = 10 * 1024 * 1024;
                if (attachment.Length > maxFileSize)
                {
                    ViewBag.Error = "Attachment size exceeds the 10MB limit.";
                    return View("Index");
                }

                // Kiểm tra định dạng tệp đính kèm
                string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".rar" };
                string fileExtension = Path.GetExtension(attachment.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ViewBag.Error = "Only .pdf, .doc, .docx, .jpg, .jpeg, .png, .rar files are allowed for attachment.";
                    return View("Index");
                }
            }

            //send mail
            List<string> receivers = new List<string>();
            using (var dbContext = new CarDealersContext())
            {
                var customers = dbContext.Customers.ToList();
                if (SendToAll != "on")
                {
                    customers = customers.Where(customer => customer.Ads).ToList();
                }
                foreach(var customer in customers)
                {
                    receivers.Add(customer.Email);
                }
            }

            if(SendMail.SendEmails(receivers, emailModel.Subject, emailModel.Body, attachment))
            {
                ViewBag.Message = "Email sent successfully";
            }
            else
            {
                ViewBag.Error = $"Failed to send email";
            }                      
            return View("Index");
        }


        public class EmailModel
        {
            public string To { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }
    }
}
