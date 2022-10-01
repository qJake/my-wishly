using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyWishly.App.Models;
using MyWishly.App.Models.Exceptions;
using MyWishly.App.Services;
using MyWishly.App.ViewModels;
using System.Security.Claims;

namespace MyWishly.App.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        public IAuthService AuthenticationService { get; }
        public ICryptographyService CryptographyService { get; }

        public HomeController(IAuthService authenticationService, ICryptographyService cryptographyService)
        {
            AuthenticationService = authenticationService;
            CryptographyService = cryptographyService;
        }

        public IActionResult Index()
        {
            if (TempData["UserRegistered"] is bool b && b)
            {
                ViewBag.ShowRegisterMessage = true;
            }
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (login is null)
            {
                ModelState.AddModelError(nameof(LoginViewModel.Username), "Login failed, please try again.");
                return View(login);
            }

            User user;
            try
            {
                // Attempt to get an existing user account - will NOT throw an exception if we found someone.
                // TODO: Fix this to be the inverse - throw a "user found" exception, or add a "UserExists" function.
                user = await AuthenticationService.GetUser(login.Username!.Trim().ToLower());
                if (user is null)
                {
                    throw new LoginException($"Username '{login.Username}' was not found.");
                }

                if (!await CryptographyService.CheckPassword(user.PasswordHash!, login.Password!, user.CreatedUtc))
                {
                    throw new LoginException($"Invalid password for '{login.Username}'.");
                }
            }
            catch 
            {
                ModelState.AddModelError(nameof(LoginViewModel.Username), "Login failed, please try again.");
                return View(login);
            }

            // Otherwise, sign in!
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, user.Email!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.Name!)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = login.RememberMe ? DateTimeOffset.UtcNow.AddDays(180) : DateTimeOffset.UtcNow.AddMinutes(20),
                IsPersistent = login.RememberMe,
                IssuedUtc = DateTimeOffset.Now.Subtract(TimeSpan.FromSeconds(1))
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Attempt to get an existing user account - will NOT throw an exception if we found someone.
                    // TODO: Fix this to be the inverse - throw a "user found" exception, or add a "UserExists" function.
                    await AuthenticationService.GetUser(user.Email!.Trim().ToLower());

                    ModelState.AddModelError(nameof(RegisterViewModel.Email), "An account with that e-mail address already exists.");
                    return View(user);
                }
                catch (UserNotFoundException)
                {
                    // No duplicate user found
                }

                var created = DateTimeOffset.Now;

                await AuthenticationService.RegisterUser(new User
                {
                     Email = user.Email,
                     Name = user.Name,
                     PasswordHash = await CryptographyService.HashPassword(user.Password!, created),
                     CreatedUtc = created,
                     CreatedIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                TempData["UserRegistered"] = true;
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }
    }
}
