using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WlodCar.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. role (masz je już w migracji, ale dla pewności…)
            foreach (var r in new[] { "Guest", "Client", "Staff", "Admin" })
                if (!await roles.RoleExistsAsync(r))
                    await roles.CreateAsync(new IdentityRole(r));

            // 2. admin
            const string adminMail = "admin@wlodcar.pl";
            var admin = await users.FindByEmailAsync(adminMail);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminMail,
                    Email = adminMail,
                    EmailConfirmed = true,
                    FirstName = "Adam",
                    LastName = "Admin"
                };

                await users.CreateAsync(admin, "Admin123!");          // hasło DEV
                await users.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
