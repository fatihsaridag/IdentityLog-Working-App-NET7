using IdentityLog.Entities;
using IdentityLog.Models;
using IdentityLog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityLog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager , SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            _logger.LogInformation("Index Sayfasına Giriş Yapıldı.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {

            if (ModelState.IsValid)
            {


                AppUser appUser = new AppUser();
                appUser.FirstName = registerViewModel.FirstName;
                appUser.LastName = registerViewModel.LastName;
                appUser.Email = registerViewModel.Email;
                appUser.UserName = registerViewModel.UserName;

                if (registerViewModel.Password == registerViewModel.Repassword)
                {
                    IdentityResult result = await _userManager.CreateAsync(appUser,registerViewModel.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login");

                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                return View(registerViewModel);
            }
            return View(registerViewModel);

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async  Task<IActionResult> Login(LoginViewModel loginViewModel)
        {

            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(loginViewModel.Email);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(loginViewModel.Email), "Geçersiz email veya şifresi");
                }

            }

            return View(loginViewModel);
        }


    }
}