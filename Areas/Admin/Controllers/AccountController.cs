using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AkinTeknik.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class AccountController : Controller
{
    private readonly IConfiguration _configuration;

    public AccountController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string Username, string Password, bool RememberMe = false, string? returnUrl = null)
    {
        var configUsername = _configuration["AdminAuth:Username"];
        var configPassword = _configuration["AdminAuth:Password"];
        var passwordHash = _configuration["AdminAuth:PasswordHash"];
        var configEmail = _configuration["AdminAuth:Email"];

        if (string.IsNullOrEmpty(configUsername) || (string.IsNullOrEmpty(configPassword) && string.IsNullOrEmpty(passwordHash)))
        {
            ViewBag.Error = "Yönetici hesabı yapılandırılmamış. Lütfen sistem yöneticisi ile iletişime geçin.";
            return View();
        }

        var inputUser = Username?.Trim() ?? "";
        bool userMatches = (!string.IsNullOrEmpty(configUsername) && string.Equals(inputUser, configUsername, StringComparison.OrdinalIgnoreCase)) ||
                           (!string.IsNullOrEmpty(configEmail) && string.Equals(inputUser, configEmail, StringComparison.OrdinalIgnoreCase));

        bool passwordMatches = false;
        if (userMatches && !string.IsNullOrEmpty(Password))
        {
            if (!string.IsNullOrEmpty(passwordHash))
            {
                var hasher = new PasswordHasher<string>();
                var verifyResult = hasher.VerifyHashedPassword(configUsername, passwordHash, Password);
                passwordMatches = verifyResult != PasswordVerificationResult.Failed;
            }
            else if (!string.IsNullOrEmpty(configPassword))
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(Password);
                var configBytes = System.Text.Encoding.UTF8.GetBytes(configPassword);
                passwordMatches = System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(inputBytes, configBytes);
            }
        }

        if (!userMatches || !passwordMatches)
        {
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, configUsername),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("DisplayName", "Akın Teknik Yöneticisi")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = RememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(RememberMe ? 14 : 1)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home", new { area = "Admin" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> LogoutGet()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account", new { area = "Admin" });
    }
}
