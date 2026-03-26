using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineMobileServices.Models;  

namespace OnlineMobileServices.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<RechargePlan> RechargePlans { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ServiceActivation> ServiceActivations { get; set; }
        public DbSet<ActivityHistory> Activities { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Decimal precision fix
            builder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            builder.Entity<RechargePlan>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Entity<ActivityHistory>()
    .Property(a => a.Amount)
    .HasPrecision(18, 2);


            // Default Recharge Plans
            builder.Entity<RechargePlan>().HasData(

   
    new RechargePlan
    {
        PlanId = 1,
        PlanType = "TopUp",
        Amount = 100,
        Description = "Talktime Rs 100"
    },
    new RechargePlan
    {
        PlanId = 2,
        PlanType = "TopUp",
        Amount = 300,
        Description = "Talktime Rs 300"
    },
    new RechargePlan
    {
        PlanId = 3,
        PlanType = "TopUp",
        Amount = 500,
        Description = "Talktime Rs 500"
    },

    // 🟣 SPECIAL
    new RechargePlan
    {
        PlanId = 4,
        PlanType = "Special",
        Amount = 299,
        Description = "2GB/Day + Unlimited Calls"
    },
    new RechargePlan
    {
        PlanId = 5,
        PlanType = "Special",
        Amount = 499,
        Description = "3GB/Day + OTT Access"
    },
    new RechargePlan
    {
        PlanId = 6,
        PlanType = "Special",
        Amount = 799,
        Description = "Unlimited 5G + Premium"
    }
);
        }

    }
}
