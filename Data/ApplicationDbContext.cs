// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WlodCar.Data.Entities;

namespace WlodCar.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /* ---------- DbSety ------------------------------------------- */
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<LoyaltyTransaction> LoyaltyTransactions => Set<LoyaltyTransaction>();

        /* ---------- konfiguracja ------------------------------------- */
        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            /* unikalny numer rejestracyjny */
            b.Entity<Car>()
             .HasIndex(c => c.RegistrationNumber)
             .IsUnique();

            /* indeks po UserId – pod zapytania historii punktów */
            b.Entity<LoyaltyTransaction>()
             .HasIndex(t => t.UserId);

            /* seeding ról (zosta³o bez zmian) */
            string[] roles = ["Guest", "Client", "Staff", "Admin"];
            foreach (var r in roles)
                b.Entity<IdentityRole>()
                 .HasData(new IdentityRole { Id = r, Name = r, NormalizedName = r.ToUpper() });
        }
    }
}