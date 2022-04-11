using Microsoft.AspNetCore.Authorization;
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
    public class StaffController : Controller
    {
        private readonly IStaffRepository _staffRepository;
        private readonly SignInManager<Staff> _signInManager;
        private readonly UserManager<Staff> _userManager;
        private readonly IEmailService _emailSender;

        public StaffController(IStaffRepository staffRepository,
                                SignInManager<Staff> signInManager,
                                 UserManager<Staff> userManager,
                                 IEmailService emailSender
                                )
        {
            _staffRepository = staffRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        //Login-Get
        public IActionResult Login()
        {
            return View();
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
            if (!ModelState.IsValid)
            {
                return View();
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
                Department = model.Department
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

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
    }
}
