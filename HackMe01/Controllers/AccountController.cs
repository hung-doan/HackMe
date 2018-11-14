using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HackMe01.Models;
using HackMe01.Persistence;
using HackMe01.Persistence.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HackMe01.Controllers
{
    public class AccountController : Controller
    {
        private readonly HackMeDbContext _db;
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;
        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, HackMeDbContext db)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        ViewBag.Message = error.ErrorMessage;
                        break;
                    }
                }

                return View();
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (signInResult.Succeeded)
                {
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    //await _signInManager.SignInAsync(user, false);
                    if (user.UserName != HackMeDbContextSeed.poorGuyUserName)
                    {
                        var flag = new FlagTrack
                        {
                            Source = $"login:{user.UserName} and password = {request.Password}",
                            Flag = Guid.NewGuid().ToString(),
                            CreatedByUserName = user.UserName,
                            CreatedDate = DateTimeOffset.UtcNow
                        };
                        await _db.FlagTracks.AddAsync(flag);
                        await _db.SaveChangesAsync();

                        return View("_LoginSuccessWithFlag", flag.Flag);
                    }
                    return Redirect("/Profile");
                }
            }


            ViewBag.Message = "User name or password does not match";
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout(LoginRequest request)
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}