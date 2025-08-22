using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WlodCar.Data;
using WlodCar.Data.Entities;

namespace WlodCar.Services;

public class ReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(IServiceProvider serviceProvider, ILogger<ReminderService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndSendReminders();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wysyłania przypomnień");
            }

            // Sprawdzaj co 6 godzin
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }

    private async Task CheckAndSendReminders()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var tomorrow = DateTime.Today.AddDays(1);

        // Znajdź rezerwacje kończące się jutro
        var reservations = await db.Reservations
            .Where(r => r.Status == ReservationStatus.Active &&
                       r.DateTo.Date == tomorrow)
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            try
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == reservation.UserId.ToString());
                var car = await db.Cars.FirstOrDefaultAsync(c => c.Id.ToString() == reservation.CarId);

                if (user != null && car != null && !string.IsNullOrEmpty(user.Email))
                {
                    await emailService.SendReturnReminderAsync(
                        user.Email,
                        car.Name,
                        reservation.DateTo);

                    _logger.LogInformation($"Wysłano przypomnienie do {user.Email} o zwrocie {car.Name}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Błąd wysyłania przypomnienia dla rezerwacji {reservation.Id}");
            }
        }
    }
}