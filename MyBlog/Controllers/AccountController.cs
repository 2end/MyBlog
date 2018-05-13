using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyBlog.Models;
using MyBlog.Models.Account;
using MyBlog.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlog.Controllers
{
    public class AccountController: Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
				User user = new User
				{
                    UserName = model.Name,
                    Email = model.Email,
                };

                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
					await userManager.AddToRoleAsync(user, "user");
					string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
					string callbackUrl = Url.Action
						(
							"ConfirmEmail",
							"Account",
							new { userId = user.Id, code = code },
							protocol: HttpContext.Request.Scheme
						);
					EmailService emailService = new EmailService();
					await emailService.SendEmailAsync(model.Email, "Confirm your account",
						$"To submit registration, go to <a href='{callbackUrl}'>link</a>");
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

		[HttpGet]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return RedirectToAction("Index", "Home");
			}
			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return View("Error");
			}
			var result = await userManager.ConfirmEmailAsync(user, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
				User user = await userManager.FindByEmailAsync(model.Login);
				if (user == null)
				{
					user = await userManager.FindByNameAsync(model.Login);
				}
				if (user != null)
				{
					var result = await signInManager.PasswordSignInAsync(user, model.Password, true, false);
					if (result.Succeeded)
					{
						return RedirectToAction("Index", "Home");
					}
					else
					{
						ModelState.AddModelError("", "Wrong name and (or) password...");
					}
				}
                
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

		public IActionResult ForgotPassword()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				User user = await userManager.FindByEmailAsync(model.Email);
				if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
				{
					return View("ForgotPasswordConfirmation");
				}

				string code = await userManager.GeneratePasswordResetTokenAsync(user);
				string callbackUrl = Url.Action
					(
					"ResetPassword",
					"Account",
					new { userId = user.Id, code = code },
					protocol: HttpContext.Request.Scheme
					);
				EmailService emailService = new EmailService();
				await emailService.SendEmailAsync(model.Email, "Reset Password",
					$"To reset password go to <a href='{callbackUrl}'>link</a>");
				return View("ForgotPasswordConfirmation");
			}

			return View(model);
		}

		public IActionResult ResetPassword(string code = null)
		{
			return code == null ? View("Error") : View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				User user = await userManager.FindByEmailAsync(model.Email);
				if (user != null)
				{
					IdentityResult result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);
					if (result.Succeeded)
					{
						return RedirectToAction("ResetPasswordConfirmation", "Account");
					}
				}
			}

			return View(model);
		}

		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}

	}
}
