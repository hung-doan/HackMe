using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HackMe01.Models;
using HackMe01.Persistence;
using HackMe01.Persistence.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HackMe01.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly HackMeDbContext _db;
        public OrderController(HackMeDbContext db)
        {
            _db = db;
        }

        public IActionResult AddToCard(int id)
        {
            var productRequest = _db.Products.Where(p => p.Id == id).Select(p => new AddToCardRequest
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Price = p.Price
            }).FirstOrDefault();

            if (productRequest == null)
            {
                ViewBag.ErrorMessage = "Requested product doesn't exist.";
            }
            else
            {

                ViewBag.ErrorMessage = productRequest.Validate(GetMyAccountBalance());
            }
            return View(productRequest);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCard(AddToCardRequest request)
        {
            string flagId;
            using (var tx = _db.Database.BeginTransaction())
            {
                var userId = GetUserId();
                var userName = GetUserName();
                var productRequest = await _db.Products.Where(p => p.Id == request.ProductId).Select(p => new AddToCardRequest
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Price = p.Price
                }).FirstOrDefaultAsync();

                var userDebitAccount = await _db.DebitAccounts.FirstAsync(p => p.UserId == userId);

                if (productRequest == null)
                {
                    ViewBag.ErrorMessage = "Requested product doesn't exist.";
                }
                else
                {

                    ViewBag.ErrorMessage = productRequest.Validate(GetMyAccountBalance());
                }

                if (!string.IsNullOrEmpty(ViewBag.ErrorMessage))
                {
                    return View(productRequest);
                }

                productRequest.Note = request.Note;

                userDebitAccount.Balance = userDebitAccount.Balance - productRequest.Price;
                await _db.Orders.AddAsync(new Order
                {
                    ProductId = productRequest.ProductId,
                    Price = productRequest.Price,
                    UserId = userId,
                    CreatedDate = DateTimeOffset.UtcNow,
                    Note = productRequest.Note
                });

                var flag = new FlagTrack
                {
                    Source = $"order:{productRequest.ProductName} with price = {productRequest.Price}",
                    Flag = Guid.NewGuid().ToString(),
                    CreatedByUserName = userName,
                    CreatedDate = DateTimeOffset.UtcNow
                };

                flagId = flag.Flag;
                await _db.FlagTracks.AddAsync(flag);

                await _db.SaveChangesAsync();

                tx.Commit();

            }

            return View("_OrderSuccessWithFlag", flagId);

        }


        private decimal GetMyAccountBalance()
        {
            string userId = GetUserId();
            return _db.DebitAccounts.First(p => p.UserId == userId).Balance;
        }

        private string GetUserId()
        {
            return User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
        }
        private string GetUserName()
        {
            return User.Claims.First(p => p.Type == ClaimTypes.Name).Value;
        }
    }
}