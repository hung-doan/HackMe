using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackMe01.Persistence.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HackMe01.Persistence
{
    using Microsoft.Extensions.DependencyInjection;
    public class HackMeDbContextSeed
    {
        string breadProductName = "Bread";
        decimal breadProductPrice = 1000000;

        public const string poorGuyUserName = "poorguy";
        string poorGuyPassword = "nomoney";
        decimal poorGuyBalance = 1;

        string richGuyUserName = "richguy";
        decimal richGuyBalance = 10000;
        public async Task SeedAsync(HackMeDbContext context, IServiceProvider services)
        {
            using (var tx = await context.Database.BeginTransactionAsync())
            {

                // Seed users
                if (!context.Users.Any())
                {
                    var userManager = services.GetService<UserManager<IdentityUser>>();
                    await userManager.CreateAsync(new IdentityUser
                    {
                        UserName = poorGuyUserName,
                        Email = $"{poorGuyUserName}@gmail.com",
                        PhoneNumber = $"{poorGuyUserName}.123456789",
                        EmailConfirmed = true
                    }, poorGuyPassword);

                    await userManager.CreateAsync(new IdentityUser
                    {
                        UserName = richGuyUserName,
                        Email = $"{richGuyUserName}@gmail.com",
                        PhoneNumber = $"{richGuyUserName}.123456789",
                        EmailConfirmed = true
                    }, $"rich.{Guid.NewGuid().ToString()}");

                    for (var i = 1; i <= 100; i++)
                    {
                        await userManager.CreateAsync(new IdentityUser
                        {
                            UserName = $"User{i}",
                            Email = $"user{i}@gmail.com",
                            PhoneNumber = $"user{i}.123456789",
                            EmailConfirmed = i <= 90
                        }, $"{i}.{Guid.NewGuid().ToString()}");
                    }
                }

                // Seed Product
                if (!context.Products.Any(p => p.Name == breadProductName))
                {
                    await context.Products.AddAsync(new Product
                    {
                        Name = breadProductName,
                        Price = breadProductPrice
                    });


                    for (var i = 1; i <= 5; i++)
                    {
                        await context.Products.AddAsync(new Product
                        {
                            Name = $"Ridiculous {i}",
                            Price = i * 1000000
                        });
                    }
                }

                // Seed Balance
                var poorGuyId = context.Users.First(p => p.UserName == poorGuyUserName).Id;
                var richGuyId = context.Users.First(p => p.UserName == richGuyUserName).Id;

                if (!context.DebitAccounts.Any(p => p.UserId == poorGuyId))
                {
                    await context.DebitAccounts.AddAsync(new DebitAccount
                    {
                        UserId = poorGuyId,
                        Balance = poorGuyBalance
                    });

                }

                if (!context.DebitAccounts.Any(p => p.UserId == richGuyId))
                {
                    await context.DebitAccounts.AddAsync(new DebitAccount
                    {
                        UserId = richGuyId,
                        Balance = richGuyBalance
                    });
                }

                await context.SaveChangesAsync();
                tx.Commit();
            }

        }
    }
}
