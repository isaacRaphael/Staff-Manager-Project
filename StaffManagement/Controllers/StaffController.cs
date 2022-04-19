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
using System.Net;
using System.Net.Mail;

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
        public IActionResult Login()
        {
            return View();
        }
        //Login-Get
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Profile(string name)
        {
            var currentStaff = _staffRepository.GetStaff(x => x.UserName == name);

            var model = new RegisterViewModel
            {
                FirstName = currentStaff.FirstName,
                LastName = currentStaff.LastName,
                UserName = currentStaff.UserName,
                JobTitle = currentStaff.PhotoPath
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> PicChange(RegisterViewModel model)
        {
            var staff = _staffRepository.GetStaff(x => x.UserName == model.UserName);
            var imgUrl = "";
            if (model.Photo is not null)
            {
                var filePath = Path.GetTempFileName();

                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.Photo.CopyToAsync(stream);
                }
                var uploadResult = await _imageService.AddImage(filePath);
                imgUrl = uploadResult;
                await _staffRepository.ChangeStaffImage(staff, imgUrl);

            }
            return RedirectToAction("Index");

            
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

        [Authorize(Roles ="Admin, User")]
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
                var uploadResult = await  _imageService.AddImage(filePath);
                imgUrl = uploadResult;

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

            string body = $"<div>" +
                 $"<h4>WELCOME TO OUR STAFF MANAGEMENT SYSTEM</h4>" +
                 $"<h5>Your login credentials are as follow:</h5>" +
                 $"</div>" +
                 $"<div><strong>Username:</strong> {model.UserName}</div>" +
                 $"<div><strong>password:</strong> {model.Password}</div>";

            var sendEmail = await _emailSender.SendEmail(content, body);

            await _signInManager.SignInAsync(newUser, false);

            return RedirectToAction("Index");
        }
        public IActionResult SendPasswordResetLink()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendPasswordResetLink(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            Console.WriteLine(user.FirstName);

            if (user == null)
            {
                ViewBag.Message = "Error while reseting password";

            } else
            {
                var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                var resetLink = Url.Action("ResetPassword", "Account", new { token = token }, protocol: HttpContext.Request.Scheme);

                // code to send the email
                var content = new EmailModel() { To = email, Subject = "Your password resent link from staff management" };
                var sendEmail = await _emailSender.SendResetToKen(content, resetLink);



                // code to send the email ends

                ViewBag.Message = $"a password reset link has been sent to your { email}";
            }
            
            return View();
        }
       
    }

}
