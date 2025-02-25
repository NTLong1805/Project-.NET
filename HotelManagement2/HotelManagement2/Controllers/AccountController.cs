using HotelManagement2.Models;
using HotelManagement2.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> Auth(string? returnUrl)
        {
            AuthViewModel model = new AuthViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };
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
                var result = await _userManager.CreateAsync(account, model.RegisterPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(account, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Register Attempt");
                return View("Auth", model);
            }
            return View("Auth", model);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(AuthViewModel model, string? ReturnUrl)
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
                if (!string.IsNullOrEmpty(ReturnUrl) || Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View("Auth", model);
            }
            return View("Auth", model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action(
                 action: "ExternalLoginCallback",
                 controller: "Account",
                 values: new { ReturnUrl = returnUrl }
            );

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? ReturnUrl, string? remoteError)
        {
            ReturnUrl = ReturnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                return Content($"<script>alert('Error from external provider: {remoteError}'); window.close(); </script>", "text/html");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return Content($"<script>alert('Error loading external login information.'); window.close();</script>", "text/html");
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true
            );

            if (signInResult.Succeeded)
            {
                return Content($"<script>window.opener.location.href = '{ReturnUrl}'; window.close();</script>", "text/html");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);

            if(email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    user = new Account
                    {
                        UserName = info.Principal.FindFirstValue(ClaimTypes.Surname) + info.Principal.FindFirstValue(ClaimTypes.GivenName),
                        Email = email
                    };
                    await _userManager.CreateAsync(user);
                }    
                await _userManager.AddLoginAsync(user, info);

                await _signInManager.SignInAsync(user, isPersistent: false);
                
                return Content($"<script>window.opener.location.href = '{ReturnUrl}'; window.close();</script>", "text/html");
            }
            return Content($"<script>alert('Email claim not received. Please contact support.'); window.close();</script>", "text/html");
        }
    }
}
