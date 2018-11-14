using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackMe01.Persistence.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HackMe01.Persistence
{
    public class HackMeDbContext : IdentityDbContext
    {
        public HackMeDbContext(DbContextOptions<HackMeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<DebitAccount> DebitAccounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<FlagTrack> FlagTracks { get; set; }
    }
}
