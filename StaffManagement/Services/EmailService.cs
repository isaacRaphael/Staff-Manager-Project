using StaffManagement.Contracts;
using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StaffManagement.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _host;


        //  uzor.nwachukwu@thebulbafrica.institute
        // B%Mw-mc8S_++iJ)
        public EmailService(string email, string password, string host)
        {
            _email = email;
            _password = password;
            _host = host;
        }
        

        public async Task<bool> SendEmail(EmailModel content, string body)
        {
            content.Body = body;
            var mail = new MailMessage();
            mail.To.Add(new MailAddress(content.To));
            mail.From = new MailAddress(_email);
            mail.Subject = content.Subject;
            mail.Body = content.Body;
            mail.IsBodyHtml = true;

            try
            {
                using (var smtp = new SmtpClient())
                {
                    var credentials = new NetworkCredential(_email, _password);
                    smtp.Credentials = credentials;
                    smtp.Host = _host;
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> SendResetToKen(EmailModel content, string link)
        {
            string body = $"<a href={link}>{link}</a>";

            return await SendEmail(content, body);
        }
    }
}
