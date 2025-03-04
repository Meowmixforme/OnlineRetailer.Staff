﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using ThAmCo.Staff.Models;

namespace ThAmCo.Staff.Controllers
{
    public class AccountController : Controller
    {
        public async Task Login(string returnUrl = "/")
        {
            var authenticationProperties = new
                LoginAuthenticationPropertiesBuilder()
                    .WithRedirectUri(returnUrl)
                    .Build();

            await HttpContext.ChallengeAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);
        }
        // Logout
        [Authorize]
        public async Task Logout()
        {
            var authenticationProperties = new
                LogoutAuthenticationPropertiesBuilder()
                    .WithRedirectUri(Url.Action("Index", "Home"))
                    .Build();

            await HttpContext.SignOutAsync(
                Auth0Constants.AuthenticationScheme, authenticationProperties);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
        // Profile
        [Authorize]
        public IActionResult Profile()
        {
            return View(new UserProfileViewModel()
            {
                Name = User.Identity.Name,
                EmailAddress = User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                ProfileImage = User.Claims
                    .FirstOrDefault(c => c.Type == "picture")?.Value
            });
        }
        // Claims
        [Authorize]
        public IActionResult Claims()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}