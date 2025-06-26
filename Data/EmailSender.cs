namespace WlodCar.Data
{
    public interface IAppEmailSender
    {
        Task SendAsync(string to, string subject, string html);
    }
    public class EmailSender : IAppEmailSender
    {
        private readonly IWebHostEnvironment _env;
        public EmailSender(IWebHostEnvironment env) => _env = env;
        public Task SendAsync(string to, string subject, string html)
        {
            var path = Path.Combine(_env.ContentRootPath, "_mails");
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, $"{Guid.NewGuid()}.html"), html);
            return Task.CompletedTask;
        }
    }
}
