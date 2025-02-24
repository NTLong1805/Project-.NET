using HotelManagement2.Models;
using HotelManagement2.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement2.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;

        public AccountController(UserManager<Account> userManager, SignInManager<Account> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Auth()
        {
            return View(new AuthViewModel());
        }

        
        [HttpPost]
        public async Task<IActionResult> SignUp(AuthViewModel model)
        {
            model.FormType = "SignUp";
            ModelState.Remove("LoginEmail");
            ModelState.Remove("LoginPassword");
            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    UserName = model.UserName,
                    Email = model.RegisterEmail,
                };
                var result = await _userManager.CreateAsync(account,model.RegisterPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(account, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Register Attempt");
                return View("Auth", model);
            }
            return View("Auth",model);
        }
       
        [HttpPost]
        public async Task<IActionResult> SignIn(AuthViewModel model)
        {
            model.FormType = "SignIn";
            ModelState.Remove("RegisterEmail");
            ModelState.Remove("UserName");
            ModelState.Remove("RegisterPassword");
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid Login Attempt");
                return View("Auth", model);
            }
            var result = await _signInManager.PasswordSignInAsync(model.LoginEmail, model.LoginPassword, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return View("Auth", model);
            }
            return View("Auth", model);
        }
    }
}
