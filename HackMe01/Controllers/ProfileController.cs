using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HackMe01.Models;
using HackMe01.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HackMe01.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private HackMeDbContext _db;
        public ProfileController(HackMeDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            string userId = User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
            var profile = (from u in _db.Users
                join b in _db.DebitAccounts on u.Id equals b.UserId
                where u.Id == userId
                select new UserProfileResponse
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Phone = u.PhoneNumber,
                    Balance = b.Balance
                }).First();

            return View(profile);
        }
    }
}