using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RapidPay.DAL.Models;

namespace RapidPay.DAL
{
    public class ContextDB : IdentityDbContext<User>
    {
        public ContextDB(DbContextOptions<ContextDB> options) : base(options) { }
        // Parameterless constructor for EF Core tools
        public ContextDB() : base(new DbContextOptionsBuilder<ContextDB>()
            .UseSqlServer("RapidPayConnection")
            .Options)
        { }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Additional model configurations can be added here
        }

    }
}
