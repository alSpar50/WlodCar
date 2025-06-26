using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WlodCar.Data;

namespace WlodCar.Services;

public class DomainPasswordValidator : IPasswordValidator<ApplicationUser>
{
    public Task<IdentityResult> ValidateAsync(
        UserManager<ApplicationUser> manager,
        ApplicationUser user,
        string? password)                // ← string?  (nullable)
    {
        if (user.Email?.Equals("admin@wlodcar.pl",
                StringComparison.OrdinalIgnoreCase) == true &&
            (password?.Length ?? 0) < 12)
        {
            return Task.FromResult(
                IdentityResult.Failed(new IdentityError
                {
                    Code = "PasswordTooShortForAdmin",
                    Description = "Hasło kierownika musi mieć co najmniej 12 znaków."
                }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}

