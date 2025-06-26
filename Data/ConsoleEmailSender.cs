using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WlodCar.Data
{
    public class ConsoleEmailSender : IEmailSender<ApplicationUser>
    {
        public Task SendConfirmationLinkAsync(
            ApplicationUser user, string email, string confirmationLink) =>
            Log("CONFIRM", email, confirmationLink);

        public Task SendPasswordResetLinkAsync(
            ApplicationUser user, string email, string resetLink) =>
            Log("RESET", email, resetLink);

        public Task SendPasswordResetCodeAsync(
            ApplicationUser user, string email, string resetCode) =>
            Log("RESET-CODE", email, resetCode);

        private Task Log(string type, string email, string content)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{type}] {email} ⇒ {content}");
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
