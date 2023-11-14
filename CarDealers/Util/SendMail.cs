using CarDealers.Entity;
using System.Net.Mail;
using System.Net;
using Microsoft.VisualBasic;

namespace CarDealers.Util
{
    public class SendMail
    {
        public static bool SendEmail(string receiver, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Constant.SENDER_EMAIL, Constant.SENDER_PASSWORD),
                    EnableSsl = true,
                };
                using (var dbContext = new CarDealersContext())
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(Constant.SENDER_EMAIL),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(receiver);

                    smtpClient.Send(mailMessage);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SendEmails(List<string> receivers, string subject, string body, IFormFile attachment = null)
        {
            try
            {
                //setup stmlClient
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Constant.SENDER_EMAIL, Constant.SENDER_PASSWORD),
                    EnableSsl = true,
                };

                //Create message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Constant.SENDER_EMAIL),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                if (attachment != null && attachment.Length > 0)
                {
                    var attachmentStream = attachment.OpenReadStream();
                    var attachmentName = attachment.FileName;
                    mailMessage.Attachments.Add(new Attachment(attachmentStream, attachmentName));
                }

                //add email list
                foreach (string receiver in receivers)
                {
                    mailMessage.Bcc.Add(receiver);
                    smtpClient.Send(mailMessage);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
