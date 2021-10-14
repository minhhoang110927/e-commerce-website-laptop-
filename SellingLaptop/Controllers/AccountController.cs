using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SellingLaptop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SellingLaptop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly DataContext dataContext;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.dataContext = dataContext;

        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationModel userRegistrationModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userRegistrationModel);
            }
            var cart = new Cart() {Total = 0};
            dataContext.Add(cart);
            await dataContext.SaveChangesAsync();
            var user = new User() { FullName = userRegistrationModel.FullName, UserName = userRegistrationModel.Username, Email = userRegistrationModel.Email, CartId = cart.CartId};
            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
            var listresult = result.Errors;
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError("Register", error.Description);
                }
                return View(userRegistrationModel);
            }
            //Change role Visitor
            //await userManager.AddToRoleAsync(user, "Visitor");
            await userManager.AddToRoleAsync(user, "Administrator");
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.url = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel userLoginModel, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(userLoginModel);
            }
            var result = await signInManager.PasswordSignInAsync(userLoginModel.Username, userLoginModel.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectTo(returnUrl);
            }
            else
            {
                ModelState.AddModelError("Login", "Sai tên đăng nhập hoặc mật khẩu");
                return View();
            }
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }
        public void SendEmail(string email, string subject, string message)
        {
            var sender = new MailAddress("adminSellingLaptop@gmail.com", "Admin Selling Laptop");
            var receiver = new MailAddress(email);
            

        }

        private IActionResult RedirectTo(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
