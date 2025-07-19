using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RootsApp.Models;
using RootsApp.Services.Interfaces;

namespace RootsApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            var isCaptchaValid = await VerifyCaptchaAsync(gRecaptchaResponse);
            if (!isCaptchaValid)
            {
                ViewBag.Error = "Captcha validation failed.";
                return View();
            }

            var user = _userService.Login(username, password);
            if (user == null)
            {
                ViewBag.Error = "Invalid login.";
                return View();
            }

            if (user.IsEmailConfirmed != true)
            {
                TempData["Email"] = user.Email;
                TempData["Message"] = "Please confirm your email first.";
                return RedirectToAction("VerifyEmail");
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Index", "Invoice");
        }

        private async Task<bool> VerifyCaptchaAsync(string token)
        {
            var secretKey = _configuration["GoogleReCaptcha:SecretKey"];
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
                    null
                );
                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonString);
                return result.success == "true" || result.success == true;
            }
        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!await _userService.RegisterAsync(user))
            {
                ViewBag.Error = "Username or email already exists.";
                return View();
            }

            TempData["Email"] = user.Email;
            TempData["Message"] = "A verification code has been sent to your email.";
            return RedirectToAction("VerifyEmail");
        }

        public IActionResult VerifyEmail()
        {
            ViewBag.Email = TempData["Email"];
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public IActionResult VerifyEmail(string email, string code)
        {
            if (_userService.VerifyEmailCode(email, code))
            {
                TempData["Success"] = "Email successfully confirmed. You can now log in.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Invalid or expired verification code.";
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendVerificationCode(string email)
        {
            if (await _userService.ResendVerificationCodeAsync(email))
            {
                ViewBag.Message = "A new verification code has been sent.";
            }
            else
            {
                ViewBag.Error = "An error occurred while sending the code.";
            }

            ViewBag.Email = email;
            return View("VerifyEmail");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [HttpPost]
        public IActionResult ChangeLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
