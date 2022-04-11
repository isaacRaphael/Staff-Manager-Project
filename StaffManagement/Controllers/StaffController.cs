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

        public StaffController(IStaffRepository staffRepository,
                                SignInManager<Staff> signInManager,
                                 UserManager<Staff> userManager

                                )
        {
            _staffRepository = staffRepository;
            _signInManager = signInManager;
            _userManager = userManager;
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


                var myAccount = new Account { ApiKey = "848314188379545", ApiSecret = "L9STxKQsLvitNYrdxBAhltnPLLk", Cloud = "dhfao0jm7" };
                Cloudinary _cloudinary = new Cloudinary(myAccount);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath)
                };

                var uploadResult = await  _cloudinary.UploadAsync(uploadParams);
                imgUrl = uploadResult.Url.AbsoluteUri;

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

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Unable to create user");
                return View(model);
            }

            await _signInManager.SignInAsync(newUser, false);

            return RedirectToAction("Index");
        }
    }
}
