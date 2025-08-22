using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace WlodCar.Services;

public interface IEmailService
{
    Task SendReservationConfirmationAsync(string recipientEmail, string carName, DateTime fromDate, DateTime toDate);
    Task SendReturnReminderAsync(string recipientEmail, string carName, DateTime returnDate);
    Task SendTestEmailAsync(string recipientEmail);
    Task SendCustomMessageAsync(string recipientEmail, string subject, string message); // NOWA METODA
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _fromEmail = "wlodekcar@gmail.com";
    private readonly string _fromPassword;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _fromPassword = _configuration["EmailSettings:Password"] ?? "";

        // Loguj konfigurację (bez hasła!)
        _logger.LogInformation($"EmailService initialized: From={_fromEmail}, HasPassword={!string.IsNullOrEmpty(_fromPassword)}");
    }

    public async Task SendTestEmailAsync(string recipientEmail)
    {
        var subject = "Test Email - WlodCar";
        var body = @"
            <html>
            <body>
                <h2>Test Email z WlodCar</h2>
                <p>Jeśli widzisz tę wiadomość, system mailowy działa poprawnie!</p>
                <p>Data wysłania: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
            </body>
            </html>";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendCustomMessageAsync(string recipientEmail, string subject, string message)
    {
        var fullSubject = $"WlodCar - {subject}";
        var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; margin: 0; padding: 0; }}
                    .container {{ max-width: 600px; margin: 0 auto; }}
                    .header {{ background: #1e3a8a; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; background: #f9fafb; }}
                    .footer {{ background: #e5e7eb; padding: 15px; text-align: center; font-size: 12px; color: #6b7280; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2 style='margin: 0;'>WlodCar</h2>
                        <p style='margin: 5px 0; font-size: 14px;'>Wypożyczalnia samochodów</p>
                    </div>
                    <div class='content'>
                        {message}
                    </div>
                    <div class='footer'>
                        <p style='margin: 5px 0;'>
                            <strong>WlodCar Sp. z o.o.</strong><br>
                            Al. Kilińskiego 12, 09-402 Płock<br>
                            Tel: +48 661 121 122 | Email: bok@wlodcar.pl
                        </p>
                        <hr style='border: none; border-top: 1px solid #d1d5db; margin: 10px 0;'>
                        <p style='margin: 5px 0; font-size: 11px;'>
                            Ta wiadomość została wysłana z systemu WlodCar.<br>
                            Jeśli nie jesteś adresatem tej wiadomości, prosimy o jej usunięcie.
                        </p>
                    </div>
                </div>
            </body>
            </html>";

        await SendEmailAsync(recipientEmail, fullSubject, body);
    }

    public async Task SendReservationConfirmationAsync(string recipientEmail, string carName, DateTime fromDate, DateTime toDate)
    {
        var subject = "Potwierdzenie rezerwacji - WlodCar";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #1e3a8a;'>Potwierdzenie rezerwacji</h2>
                <p>Dziękujemy za dokonanie rezerwacji w WlodCar!</p>
                <div style='background: #f3f4f6; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Samochód:</strong> {carName}</p>
                    <p><strong>Data odbioru:</strong> {fromDate:dd.MM.yyyy}</p>
                    <p><strong>Data zwrotu:</strong> {toDate:dd.MM.yyyy}</p>
                </div>
                <p>W razie pytań prosimy o kontakt: bok@wlodcar.pl lub +48 661 121 122</p>
                <hr style='margin-top: 30px;'>
                <p style='color: #6b7280; font-size: 12px;'>
                    WlodCar Sp. z o.o.<br>
                    Al. Kilińskiego 12, 09-402 Płock
                </p>
            </body>
            </html>";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    public async Task SendReturnReminderAsync(string recipientEmail, string carName, DateTime returnDate)
    {
        var subject = "Przypomnienie o zwrocie samochodu - WlodCar";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #dc2626;'>Przypomnienie o zwrocie</h2>
                <p>Przypominamy o zbliżającym się terminie zwrotu samochodu.</p>
                <div style='background: #fef2f2; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc2626;'>
                    <p><strong>Samochód:</strong> {carName}</p>
                    <p><strong>Termin zwrotu:</strong> {returnDate:dd.MM.yyyy}</p>
                </div>
                <p>Prosimy o punktualny zwrot pojazdu.</p>
                <p>W razie pytań prosimy o kontakt: bok@wlodcar.pl lub +48 661 121 122</p>
                <hr style='margin-top: 30px;'>
                <p style='color: #6b7280; font-size: 12px;'>
                    WlodCar Sp. z o.o.<br>
                    Al. Kilińskiego 12, 09-402 Płock
                </p>
            </body>
            </html>";

        await SendEmailAsync(recipientEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            _logger.LogInformation($"Wysyłanie maila do: {toEmail}, temat: {subject}");

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_fromEmail, _fromPassword),
                Timeout = 30000 // 30 sekund timeout
            };

            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, "WlodCar"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            // Dodaj nagłówki dla lepszej dostarczalności
            message.Headers.Add("X-Priority", "1");
            message.Headers.Add("X-MSMail-Priority", "High");
            message.Headers.Add("Importance", "High");

            await client.SendMailAsync(message);
            _logger.LogInformation($"Mail wysłany pomyślnie do: {toEmail}");
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, $"SMTP Error podczas wysyłania maila do: {toEmail}. Status: {smtpEx.StatusCode}");
            throw new Exception($"Błąd SMTP: {smtpEx.Message}", smtpEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Błąd wysyłania maila do: {toEmail}");
            throw;
        }
    }
}