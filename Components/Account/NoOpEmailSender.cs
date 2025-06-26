using Microsoft.AspNetCore.Identity;

namespace WlodCar.Components.Account
{
    /// <summary>
    /// Pusta implementacja IEmailSender – nic nie wysyła.
    /// </summary>
    public class NoOpEmailSender<TUser> : IEmailSender<TUser> where TUser : class
    {
        public Task SendConfirmationLinkAsync(
            TUser user, string email, string confirmationLink) => Task.CompletedTask;

        public Task SendPasswordResetLinkAsync(
            TUser user, string email, string resetLink) => Task.CompletedTask;

        public Task SendPasswordResetCodeAsync(
            TUser user, string email, string resetCode) => Task.CompletedTask;
    }
}
