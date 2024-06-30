using DMS.WebApp.MVC.Models;
using DMS.WebApp.MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DMS.WebApp.MVC.Controllers;

public class IdentityController : MainController
{
    private readonly IIdentityAuthenticationService _identityAuthenticationService;

    public IdentityController(IIdentityAuthenticationService identityAuthenticationService)
    {
        _identityAuthenticationService = identityAuthenticationService;
    }

    [HttpGet]
    [Route("nova-conta")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [Route("nova-conta")]
    public async Task<IActionResult> Register(UserRegistration userRegistration)
    {
        if (!ModelState.IsValid) return View(userRegistration);

        var response = await _identityAuthenticationService.Register(userRegistration);

        if (ResponseHasErrors(response.ResponseResult)) return View(userRegistration);

        await RunLogin(response);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("login")]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(UserLogin userLogin, string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(userLogin);

        var response = await _identityAuthenticationService.Login(userLogin);

        if (ResponseHasErrors(response.ResponseResult)) return View(userLogin);

        await RunLogin(response);

        if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "Home");

        return RedirectToAction(returnUrl);
    }

    [HttpGet]
    [Route("sair")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private async Task RunLogin(UserResponseLogin responseLogin)
    {
        var token = GetTokenFormatted(responseLogin.AccessToken);

        var claims = new List<Claim>();
        claims.Add(new Claim("JWT", responseLogin.AccessToken));
        claims.AddRange(token.Claims);

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
            IsPersistent = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    private static JwtSecurityToken GetTokenFormatted(string jwtToken)
    {
        return new JwtSecurityTokenHandler().ReadToken(jwtToken) as JwtSecurityToken;
    }
}
