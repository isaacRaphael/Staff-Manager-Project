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
        public async Task<bool> SendLoginCredential(EmailModel content, string username, string password)
        {
            content.Body = $"<div>" +
                $"<h4>WELCOME TO OUR STAFF MANAGEMENT SYSTEM</h4>" +
                $"<h5>Your login credentials are as follow:</h5>" +
                $"</div>" +
                $"<div><strong>Username:</strong> {username}</div>" +
                $"<div><strong>password:</strong> {password}</div>";
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
            } catch
            {
                return false;
            }

            return true;

        }
    }
}
