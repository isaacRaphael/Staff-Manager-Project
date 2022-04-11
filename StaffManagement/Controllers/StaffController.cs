using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StaffManagement.Contracts;
using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace StaffManagement.Controllers
{
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly SignInManager<Staff> _signInManager;
        private readonly UserManager<Staff> _userManager;
        private readonly IEmailService _emailSender;
        private readonly IImageService _imageService;

        public StaffController(IStaffRepository staffRepository,
                                SignInManager<Staff> signInManager,
                                 UserManager<Staff> userManager,
                                 IEmailService emailSender,
                                 IImageService imageService
                                )
        {
            _staffRepository = staffRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _imageService = imageService;
        }
        //Login-Get
        public async  Task<IActionResult> Login()
        {
            await _signInManager.SignOutAsync();
            return View();
        }
        //Login-Get
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Index()
        {
            var allStaffs = _staffRepository.GetAllStaff();

            return View(allStaffs);
        }
        [Authorize(Roles ="Admin")]
        // Action for the registration page
        public IActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            string imgUrl = "";
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (model.Photo is not null)
            {
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.Photo.CopyToAsync(stream);
                }
                imgUrl = await  _imageService.AddImage(filePath);

            }
            


            var userWithEmail = _staffRepository.GetStaff(s => s.Email == model.Email);
            var userWithUserName = _staffRepository.GetStaff(s => s.UserName == model.UserName);

            if (userWithEmail != null || userWithUserName != null)
            {
                ModelState.AddModelError("", "User already exists");
                return View(model);
            }

            var newUser = new Staff
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                JobTitle = model.JobTitle,
                Department = model.Department,
                PhotoPath = imgUrl
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (model.UserRole == "Admin")
                await _userManager.AddToRoleAsync(newUser, model.UserRole);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Unable to create user");
                return View(model);
            }

            var content = new EmailModel() { To = model.Email, Subject = "Your account has been successfully created" };
            var sendEmail = await _emailSender.SendLoginCredential(content, model.UserName, model.Password);

            await _signInManager.SignInAsync(newUser, false);

            return RedirectToAction("Index");
        }
        public IActionResult SendPasswordResetLink()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendPasswordResetLink(string email)
        {
            return Ok();
            var user = _userManager.FindByEmailAsync(email).Result;

            if (user == null || !(_userManager.IsEmailConfirmedAsync(user).Result))
            {
                ViewBag.Message = "Error while reseting password";

            }
            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
            var resetLink = Url.Action("ResetPassword", "Account", new { token = token }, protocol: HttpContext.Request.Scheme);

            // code to send the email

            // code to send the email ends

            ViewBag.Message = $"a password reset link has been sent to your { email}";
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
