// Services/ReservationService.cs
using Microsoft.EntityFrameworkCore;
using WlodCar.Data;
using WlodCar.Data.Entities;

namespace WlodCar.Services;

public interface IReservationService
{
    Task<bool> IsCarAvailable(string carId, DateTime from, DateTime to);
    Task<Reservation> CreateAsync(Reservation reservation, int pointsToRedeem = 0);
    Task<List<Reservation>> GetForUserAsync(Guid userId);
    Task CancelAsync(int reservationId, Guid userId);
}

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _db;
    private readonly ILoyaltyService _loyalty;
    private readonly IEmailService _emailService;

    public ReservationService(ApplicationDbContext db, ILoyaltyService loyalty, IEmailService emailService)
    {
        _db = db;
        _loyalty = loyalty;
        _emailService = emailService;
    }

    public Task<bool> IsCarAvailable(string carId, DateTime from, DateTime to)
        => _db.Reservations
              .Where(r => r.CarId == carId && r.Status == ReservationStatus.Active)
              .AllAsync(r => r.DateTo <= from || r.DateFrom >= to);

    public async Task<Reservation> CreateAsync(Reservation r, int pointsToRedeem = 0)
    {
        var discountPercent = pointsToRedeem / 10;
        var discountFactor = 1m - (discountPercent / 100m);
        r.TotalPrice = Math.Round(r.TotalPrice * discountFactor, 2);

        _db.Reservations.Add(r);
        await _db.SaveChangesAsync();

        if (pointsToRedeem > 0)
            await _loyalty.RedeemAsync(r.UserId, pointsToRedeem,
                                       $"Rabat za rezerwację #{r.Id}");

        await _loyalty.AwardForPriceAsync(r.UserId, r.TotalPrice,
                                          $"Rezerwacja #{r.Id}");

        // Wysyłanie maila z potwierdzeniem
        try
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == r.UserId.ToString());
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                var car = await _db.Cars.FirstOrDefaultAsync(c => c.Id.ToString() == r.CarId);
                if (car != null)
                {
                    await _emailService.SendReservationConfirmationAsync(
                        user.Email,
                        car.Name,
                        r.DateFrom,
                        r.DateTo);
                }
            }
        }
        catch (Exception ex)
        {
            // Nie przerywaj procesu rezerwacji jeśli mail się nie wyśle
            Console.WriteLine($"Błąd wysyłania potwierdzenia: {ex.Message}");
        }

        return r;
    }

    public Task<List<Reservation>> GetForUserAsync(Guid userId)
        => _db.Reservations
              .Where(r => r.UserId == userId)
              .OrderByDescending(r => r.DateFrom)
              .ToListAsync();

    public async Task CancelAsync(int id, Guid userId)
    {
        var r = await _db.Reservations
                         .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (r is not null && r.Status == ReservationStatus.Active)
        {
            r.Status = ReservationStatus.Cancelled;
            await _db.SaveChangesAsync();
        }
    }
}