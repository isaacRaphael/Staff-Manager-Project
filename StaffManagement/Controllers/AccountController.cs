using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffManagement.Contracts;
using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Staff> _userManager;
        private readonly IEmailService _emailSender;
        public AccountController(UserManager<Staff> userManager, IEmailService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
               
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel obj)
        {
            var user = _userManager.FindByEmailAsync(obj.UserName).Result;

            IdentityResult result = _userManager.ResetPasswordAsync(user, obj.Token, obj.Password).Result;

            if (result.Succeeded)
            {
                ViewBag.Message = "Password reset successful";
                return View("Login");
            }
            else
            {
                ViewBag.Message = "Error while resetting the password";
                return View("Login");
            }
        }
    }
}
