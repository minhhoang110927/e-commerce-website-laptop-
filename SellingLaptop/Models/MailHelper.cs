using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SellingLaptop.Models
{
    public class MailHelper
    {
        public void SendMail(string from, string pass, string to, string subject, string content)
        {
            var fromEmail = new MailAddress(from, "admin selling laptop");
            var toEmail = new MailAddress(to);
            var fromEmailPass = pass;
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            bool enableSs1 = true;
            string body = content;

            MailMessage message = new MailMessage(fromEmail, toEmail);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;
            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(from, fromEmailPass);
            client.Host = smtpHost;
            client.EnableSsl = enableSs1;
            client.Port = smtpPort;
            client.Send(message);
        }
    }
}
